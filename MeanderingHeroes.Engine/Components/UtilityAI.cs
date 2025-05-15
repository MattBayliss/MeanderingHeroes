using LaYumba.Functional;
using MeanderingHeroes.Engine.Types;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Engine.Components
{

    public class UtilityAIComponent
    {
        private record BehaviourScore(Dse Dse, float Score);
        private record DseAndScoreFunc(Dse Dse, Func<float> ScoreFunc);
        private ConsiderationContext _considerationContext;
        private Dictionary<int, BehaviourDelegate> _behaviourFuncs;
        public UtilityAIComponent(ConsiderationContext context)
        {
            _considerationContext = context;
            _behaviourFuncs = new Dictionary<int, BehaviourDelegate>();
        }

        public GameState Update(Game game, GameState state)
        {
            (GameState State, IEnumerable<Entity> UpdatedEntities) initialState = (state, []);

            var updated = state
                .Entities
                .Aggregate(
                    seed: initialState,
                    func: (runningState, entity) => {
                        (var rs, var eee) = runningState;
                        var update = entity switch
                        {
                            SmartEntity pawn => UpdateAgent(rs, pawn),
                            _ => None
                        };
                        var updatedEntities = update
                            .Bind(br => br.EntityChange)
                            .Match(
                                    None: () => eee,
                                    Some: ue => eee.Append(ue)
                                );
                        var updatedState = update
                            .Bind(br => br.StateChange)
                            .Match(
                                    None: () => rs,
                                    Some: newState => newState
                                );

                        return (updatedState, updatedEntities);
                    }
                );

            return updated.State.ModifyEntities(updated.UpdatedEntities);
        }
        private Option<BehaviourResult> UpdateAgent(GameState state, SmartEntity agent)
        {
            Func<Decision, Utility> getConsideration = 
                decision => _considerationContext.GetConsideration(decision)(agent);

            var behaviours = state.Behaviours
                .Where(b => b.EntityId == agent.Id);

            var dses = behaviours
            .Select(b => b.DseId)
            .Bind(state.DseById.Lookup);

            var winningBehaviour = GetWinningBehaviour(getConsideration, dses);

            var bFunc = winningBehaviour
                .Bind(wb => behaviours.Where(b => b.DseId == wb.Dse.Id).Head())
                .Map(b => b.Run);

            return bFunc.Map(fn => fn(agent, state));
        }

        private static Option<BehaviourScore> GetWinningBehaviour(Func<Decision, Utility> getConsideration, IEnumerable<Dse> decisionEvaluators)
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
                                    scores: dse.Decisions.Select(d =>
                                        (
                                            Input: getConsideration(d),
                                            Curve: d.Curve.ToFunc()
                                        )
                                    ).Select(ic => ic.Curve(ic.Input) * dse.Weight) // TODO: add an inertia component here? dse.Inertia
                                )
                            )
                        )
                    )
                );

            // now the evalation begins
            IEnumerable<BehaviourScore> scores = [];
            foreach (var weightDseScoreFuncs in scoreFuncsByDseByWeight)
            {
                if (weightDseScoreFuncs.Weight < threshold)
                {
                    // if the DSE can't possibly meet the threshold, don't bother anymore
                    break;
                }

                // this is where the calculations are run
                var scoresBatch = weightDseScoreFuncs.DseScoreFunc
                    .Select(dsf => new BehaviourScore(dsf.Dse, dsf.ScoreFunc()))
                    .Where(bs => bs.Score > 0f)
                    // we only care about the top 3 for each weight tier (for now?)
                    .OrderByDescending(bs => bs.Score)
                    .Take(3);

                threshold = scoresBatch.Select(bs => bs.Score).Min();

                scores = scores.Concat(scoresBatch);
            }

            return scores.OrderByDescending(ds => ds.Score).Head();
        }

        private static float CalculateScore(IEnumerable<float> scores)
            => scores.Any(s => s == 0f) ? 0f : scores.Aggregate((total, next) => total *= next);
    }

}
