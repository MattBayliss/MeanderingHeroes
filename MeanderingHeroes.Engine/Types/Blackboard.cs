using LaYumba.Functional;
using System.Numerics;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Engine.Types
{
    public readonly record struct BlackboardKey<T>(string Key, bool Persistent) where T : struct
    {
        public static implicit operator string(BlackboardKey<T> key) => key.Key;
        public static implicit operator bool(BlackboardKey<T> key) => key.Persistent;
    }

    public class Blackboard
    {
        private readonly Dictionary<string, object> _transientData = [];
        private readonly Dictionary<string, object> _persistentData = [];

        public void Set<T>(BlackboardKey<T> key, T value) where T : struct
        {
            DictionaryForKey(key)[key] = value;
        }

        private Dictionary<string, object> DictionaryForKey<T>(BlackboardKey<T> key) where T : struct => key.Persistent ? _persistentData : _transientData;

        public Option<T> Get<T>(BlackboardKey<T> key) where T : struct
            => DictionaryForKey(key).TryGetValue(key, out var value) && value is T typedValue ? Some(typedValue) : None;
        public void Clear() => _transientData.Clear();
    }

    public static partial class BlackboardKeys
    {
        public static readonly BlackboardKey<int> PlayerHealth = new("Player.Health", false);
        public static readonly BlackboardKey<Vector3> PlayerPosition = new("Player.Position", false);
    }
}
