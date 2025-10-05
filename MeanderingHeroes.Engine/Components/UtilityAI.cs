using LaYumba.Functional;
using MeanderingHeroes.Engine.Types;
using Microsoft.Extensions.Logging;
using static LaYumba.Functional.F;
using static MeanderingHeroes.Engine.Functions;

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

            (IEnumerable<StateChange> StateChanges, IEnumerable<int> CompletedDSEs) initialState = ([], []);

            var updated = state
                .Entities
                .Aggregate(
                    seed: initialState,
                    func: (runningState, entity) => UpdateAgent(state, entity).Match(
                            None: () => runningState,
                            Some: scoreResult => runningState with
                            {
                                StateChanges = runningState.StateChanges.Concat(scoreResult.Result.StateChanges),
                                CompletedDSEs = scoreResult.Result.Status.HasFlag(DseStatus.Completed)
                                    ? runningState.CompletedDSEs.Append([scoreResult.Score.DseId])
                                    : runningState.CompletedDSEs
                            }
                        )
                );

            updated.StateChanges.ForEach(change => Logger.LogDebug(change.ToString()));

            return state.UpdateState(updated.StateChanges, updated.CompletedDSEs);
        }
        private Option<(BehaviourScore Score, AiResult Result)> UpdateAgent(GameState state, Entity agent)
        {
            Func<Decision, Option<Utility>> getConsideration =
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

        private record DsesByWeight(float Weight, float Threshold, IEnumerable<Dse> Dses);
        private record RunningScores(float Threshold, IEnumerable<BehaviourScore> Scores);

        private Option<BehaviourScore> GetWinningBehaviour(Func<Decision, Option<Utility>> getConsideration, IEnumerable<Dse> decisionEvaluators)
        {
            // process highest weights first (so we can stop calculating once the threshold can no longer be met)
            var dsesByWeights = decisionEvaluators
                .GroupBy(dse => dse.Weight)
                .Select(group => new DsesByWeight(group.Key, 0f, group))
                .OrderByDescending(wd => wd.Weight);

            Func<Dse, BehaviourScore> calculateScoreForDse = dse => CalculateScore(Logger, dse, getConsideration);

            // group Decision Score Evaluators by Weight,
            // and as long as the DSE can conceivably beat the threshold
            // (because the weight is greater than the threshold), keep
            // evaluating DSEs.
            // For now, return the DSE with the highest score, but in future
            // maybe make it choose randomly between the top 3 results?

            (_, var validScores) = dsesByWeights.Aggregate(
                seed: new RunningScores(0f, []),
                func: (agg, dbw) =>
                {
                    if (dbw.Weight < agg.Threshold)
                    {
                        return agg;
                    }
                    else
                    {
                        var scores = dbw.Dses.Select(calculateScoreForDse);
                        return agg with
                        {
                            Threshold = float.Max(agg.Threshold, scores.Max(s => s.Score)),
                            Scores = scores
                        };
                    }
                });
                // Func
            
            

            return validScores
                .OrderByDescending(ds => ds.Score).Head();
        }

        private static BehaviourScore CalculateScore(ILogger logger, Dse dse, Func<Decision, Option<Utility>> getConsideration)
        {
            var scores = dse.Decisions.Select
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
                            Result: dic.Input.Match
                            (
                                // for unknown/None considerations, always return 0 - don't run through
                                // curve function
                                None: () => Utility(0f), 
                                Some: i => dic.Curve(i)
                            ) // TODO: add inertia back in when basics are working
                        )
                    )
                    .Select(ddd => new DecisionResult(
                        $"{ddd.Decision.ConsiderationType}:{ddd.Input} => {ddd.CurveDescription} => {ddd.Result}",
                        ddd.Result.Value));

            scores.ForEach(score => logger.LogTrace(score.Description));

            return scores.Aggregate<DecisionResult, (IEnumerable<string> desc, float totalScore)>(
                seed: ([], 1f),
                func: (acc, score) => acc with
                {
                    desc = acc.desc.Append($"[{score.Description}]"),
                    totalScore = acc.totalScore *= score.Score,
                })
            .Pipe(ddd => new BehaviourScore(
                        DseId: dse.Id,
                        Name: $"{dse.Name}::{string.Join(",", ddd.desc)}",
                        Score: ddd.totalScore * dse.Weight));
        }
    }
}
