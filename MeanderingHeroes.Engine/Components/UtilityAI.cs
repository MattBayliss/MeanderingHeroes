using LaYumba.Functional;
using MeanderingHeroes.Engine.Types;

namespace MeanderingHeroes.Engine.Components
{
    public class UtilityAIComponent : IComponent
    {
        public GameState Update2(Game game, GameState state)
        {
            var updatedEntities = state.Entities.Select(entity => entity switch
            {
                SmartEntity smartEntity => Update(game, smartEntity).Match(() => smartEntity, updatedEntity => updatedEntity),
                _ => entity
            });

            // TODO: test to see if this creates a copy of gamestate if nothing has changed
            return state with { Entities = updatedEntities.ToImmutableList() };
        }
        private Option<SmartEntity> Update(Game game, SmartEntity entity)
        {
            // TODO: filter advertisers to only those nearby entity?
            var offers = game
                .Entities
                .OfType<Advertiser>()
                .SelectMany(ad => ad.Offers);



            // you should take the top 3 and randomly choose from those - depending
            // on the deviation of results

            // BUT this early on, just return the highest result every time
            var calculatedUtilities = entity.Behaviours
                .Select(b =>
                    (
                        Utility: b.Interaction.CalculateUtility(game, entity),
                        Behaviour: b
                    )
                ).OrderByDescending(cc => cc.Utility);

            var winner = calculatedUtilities.Head();

            return winner.Match(
                None: () => entity,
                Some: bhr => bhr.Behaviour.Update(game, entity)
            ).Pipe(updated => updated with 
                { Behaviours = updated.Behaviours.Where(i9n => !i9n.ToRemove).ToImmutableList() }
            );
        }
    }
}
