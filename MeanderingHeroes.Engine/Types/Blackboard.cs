using LaYumba.Functional;
using System.Numerics;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Engine.Types
{
    public readonly record struct BlackboardKey<T>(string Key) where T : struct
    {
        public static implicit operator string(BlackboardKey<T> key) => key.Key;
    }

    public class Blackboard
    {
        private readonly Dictionary<string, object> _data = [];

        public void Set<T>(BlackboardKey<T> key, T value) where T : struct
        {
            _data.AddOrUpdate(key, value);
        }

        public Option<T> Get<T>(BlackboardKey<T> key) where T : struct
            => _data.TryGetValue(key, out var value) && value is T typedValue ? Some(typedValue) : None;
        public void Clear() => _data.Clear();
    }

    public static partial class BlackboardKeys
    {
        public static readonly BlackboardKey<int> PlayerHealth = new("Player.Health");
        public static readonly BlackboardKey<Vector3> PlayerPosition = new("Player.Position");
    }
}
