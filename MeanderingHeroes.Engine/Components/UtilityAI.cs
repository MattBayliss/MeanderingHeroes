using LaYumba.Functional;
using MeanderingHeroes.Engine.Types;
using Microsoft.Extensions.Logging;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Engine.Components
{
    public class UtilityAIComponent
    {
        private readonly ILogger Logger;
        /// <summary>
        /// (DseId, Score)
        /// </summary>
        /// <param name="DseId"></param>
        /// <param name="Name">Name for debugging purposes</param>
        /// <param name="Score"></param>
        private record BehaviourScore(int DseId, string Name, float Score);
        private record DecisionResult(string Description, float Score);
        private record DseAndScoreFunc(Dse Dse, Func<DecisionResult> ScoreFunc);
        private ConsiderationContext _considerationContext;
        public UtilityAIComponent(ILogger<UtilityAIComponent> logger, ConsiderationContext context)
        {
            Logger = logger;
            _considerationContext = context;
        }
        public GameState Update(Game game, GameState state)
        {
            _considerationContext.SetStateSnapshot();

            (IEnumerable<Entity> UpdatedEntities, IEnumerable<int> CompletedDSEs) initialState = ([], []);

            var updated = state
                .Entities
                .Aggregate(
                    seed: initialState,
                    func: (runningState, entity) => UpdateAgent(state, entity).Match(
                            None: () => runningState,
                            Some: scoreResult => runningState with
                            {
                                UpdatedEntities = runningState.UpdatedEntities.Concat(scoreResult.Result.EntityChange.AsEnumerable()),
                                CompletedDSEs = scoreResult.Result.Status.HasFlag(DseStatus.Completed)
                                    ? runningState.CompletedDSEs.Append([scoreResult.Score.DseId])
                                    : runningState.CompletedDSEs
                            }
                        )
                );

            return state
                .ModifyEntities(updated.UpdatedEntities)
                .RemoveBehaviours(updated.CompletedDSEs);
        }
        private Option<(BehaviourScore Score, AiResult Result)> UpdateAgent(GameState state, Entity agent)
        {
            Func<Decision, Utility> getConsideration =
                decision => _considerationContext.GetConsideration(decision)(agent);

            var behaviours = state.Behaviours
                .Where(b => b.EntityId == agent.Id);

            Func<EntityBehaviour, Option<Dse>> lookupDseAndApplyInertia =
                b => state.DseById
                    .Lookup(b.DseId)
                    .Match(
                        None: () => None,
                        Some: dse => Some(dse with { Inertia = b.Inertia }));

            var dses = behaviours.Bind(lookupDseAndApplyInertia);

            Logger.LogTrace("Entity:{0}: DSEs:[{1}]", agent.Id, string.Join(",", dses.Select(dse => $"{dse.Id}:{dse.Name}")));

            var winningBehaviour = GetWinningBehaviour(getConsideration, dses);

            Logger.LogTrace($"Winning DSE: {winningBehaviour.ToString()}");

            // TODO: Decrease all losing inertia's - assign new inertia
            return winningBehaviour.Match(
                None: () => None,
                Some: score
                    => score
                        .Pipe(wb => behaviours.Where(b => b.DseId == wb.DseId).Head())
                        .Map(b => b.Run)
                        .Map(fn => fn(agent, state))
                        .Match(
                            None: () => None,
                            Some: result => Some((Score: score, Result: result))
                        )
            );
        }

        private Option<BehaviourScore> GetWinningBehaviour(Func<Decision, Utility> getConsideration, IEnumerable<Dse> decisionEvaluators)
        {
            // scores have to be able to generate values over the threshold to be considered
            float threshold = 0f;

            // process highest weights first (so we can stop calculating once the threshold can no longer be met)
            var dsesByWeight = decisionEvaluators.OrderByDescending(dse => dse.Weight).GroupBy(dse => dse.Weight);

            // setting up the linq selects - nothing is being evaluated.. yet
            IEnumerable<(float Weight, IEnumerable<DseAndScoreFunc> DseScoreFunc)> scoreFuncsByDseByWeight =
                dsesByWeight.Select(g => (
                    Weight: g.Key,
                    ScoreFuncsByDse: g.Select(
                        dse => new DseAndScoreFunc(
                            Dse: dse,
                            ScoreFunc: () => CalculateScore(
                                    scores: dse.Decisions.Select
                                        (d =>
                                        // TODO: remove extra properties for logging when we've got basic tests to pass
                                            (
                                                Decision: d,
                                                Input: getConsideration(d),
                                                Curve: d.Curve.ToFunc()
                                            )
                                        )
                                        .Select(dic =>
                                            (
                                                dic.Decision,
                                                dic.Input,
                                                CurveDescription: dic.Decision.Curve.Description,
                                                Result: dic.Curve(dic.Input) // TODO: add inertia back in when basics are working
                                            )
                                        )
                                        .Select(ddd => new DecisionResult(
                                            $"{ddd.Decision.ConsiderationType}:{ddd.Input} => {ddd.CurveDescription} => {ddd.Result}",
                                            (float)ddd.Result))
                                )
                            )
                        )
                    )
                );

            // now the evalation begins
            IEnumerable<BehaviourScore> scores = [];
            foreach (var weightDseScoreFuncs in scoreFuncsByDseByWeight)
            {
                //if (weightDseScoreFuncs.Weight < threshold)
                //{
                //    // if the DSE can't possibly meet the threshold, don't bother anymore
                //    break;
                //}

                // this is where the calculations are run
                var scoresBatch = weightDseScoreFuncs.DseScoreFunc
                    .Select(dsf => (
                            DseId: dsf.Dse.Id,
                            Name: dsf.Dse.Name,
                            Result: dsf.ScoreFunc()))
                    .Select(inr => new BehaviourScore(
                        DseId: inr.DseId,
                        Name: $"{inr.Name}::{inr.Result.Description}",
                        Score: inr.Result.Score))
                    .Where(bs => bs.Score > 0f)
                // we only care about the top 3 for each weight tier (for now?)
                    .OrderByDescending(bs => bs.Score)
                    .Do(bs => Logger.LogTrace($"{bs.Score:F3}::{bs.Name}"))
                    .Take(3);

                threshold = scoresBatch.Any() ? scoresBatch.Select(bs => bs.Score).Min() : threshold;

                scores = scores.Concat(scoresBatch);
            }

            return scores.OrderByDescending(ds => ds.Score).Head();
        }

        private static DecisionResult CalculateScore(IEnumerable<DecisionResult> scores)
            => scores.Aggregate<DecisionResult, (IEnumerable<string> desc, float totalScore)>(
                seed: ([], 1f),
                func: (acc, score) => acc with 
                    {
                        desc = acc.desc.Append($"[{score.Description}]"),
                        totalScore = acc.totalScore *= score.Score, 
                    })
            .Pipe(agg => new DecisionResult(string.Join(",", agg.desc), agg.totalScore));
    }
}
