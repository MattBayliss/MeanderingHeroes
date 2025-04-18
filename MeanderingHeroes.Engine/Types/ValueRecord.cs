using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    public record ValueRecord<T>
    {
        private T Value { get; init; }
        private ValueRecord(T value)
        {
            Value = value;
        }

        public static implicit operator T(ValueRecord<T> vr) => vr.Value;
        public static implicit operator ValueRecord<T>(T value) => new ValueRecord<T>(value);
    }
}
