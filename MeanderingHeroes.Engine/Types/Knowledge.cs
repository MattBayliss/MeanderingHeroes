using LaYumba.Functional;
using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace MeanderingHeroes.Engine.Types
{
    /// <summary>
    /// A common data structure for all tidbits - to try and make knowledge data
    /// memory efficient
    /// </summary>
    /// <param name="Consideration"></param>
    /// <param name="Hex"></param>
    /// <param name="A"></param>
    /// <param name="B"></param>
    public readonly record struct Tidbit(ConsiderationType Consideration, Hex Hex, float A, float B)
    {
        public long OnTick { get; init; } = Game.Tick;
        public override string ToString() => $"{OnTick}:{Consideration.ToString()}-{Hex.ToString()}:{A:F3},{B:F3}";
        public Utility UtilityValue => Utility(this.A);
        public FractionalHex CoordsValue => new FractionalHex(this.A, this.B);
        public Hex HexValue => this.CoordsValue.Round();
    }
    public record Knowledge
    {
        public ImmutableHashSet<Tidbit> Tidbits { get; init; }
        public Knowledge(IEnumerable<Tidbit> tidbits)
        {
            Tidbits = tidbits.ToImmutableHashSet();
        }
        public Option<Tidbit> GetTidbit(ConsiderationType consideration, Hex hex)
            => Tidbits
                .Where(td => td.Hex == hex)
                .Where(td => td.Consideration == consideration)
                .Head();


    }
    /// <summary>
    /// Holds all the knowledge of the world an entity is aware of - will have to see how feasible it
    /// is to track all this. Will have to be serialisable to be saved and loaded from save files.
    /// Knowledge is stored as Tidbits - currently storing the ConsiderationType and Hex - might creep
    /// into a bigger structure.
    /// </summary>
    public class KnowledgeBase
    {
        private ConcurrentDictionary<int, Knowledge> _knowledgeForEntity { get; init; }
        public Option<Knowledge> this[int entityId] => _knowledgeForEntity.Lookup(entityId);

        public KnowledgeBase()
        {
            _knowledgeForEntity = [];
        }

        public void AddTidbits(int entityId, IEnumerable<Tidbit> tidbits)
            => tidbits.ForEach(tidbit => _knowledgeForEntity.AddOrUpdate
                (
                    entityId,
                    _ => new Knowledge([tidbit]),
                    (_, oldKnowledge) => oldKnowledge with { Tidbits = oldKnowledge.Tidbits.Add(tidbit) }
                ));
        public Option<Tidbit> GetTidbit(int entityId, ConsiderationType consideration, Hex hex)
            => _knowledgeForEntity.TryGetValue(entityId, out var knowledge) ? knowledge.GetTidbit(consideration, hex) : None;
    }
}
