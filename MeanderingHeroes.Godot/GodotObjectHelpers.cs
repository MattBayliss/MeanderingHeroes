using Godot;
using LaYumba.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Godot
{
    /// <summary>
    /// In order to minimise interop & marshalling - cache core objects in C#
    /// </summary>
    public record GodotObject<T>(string Id, T Value);
    public static class GodotObjectHelpers
    {
        public static Option<T> ToOption<T>(this T node) where T : Node => node == null ? None : Some(node);
        public static void Do<T>(this Option<T> @this, Action<T> action) => @this.ForEach(action);
    }

    public class OnReady<T> where T : Node
    {
        private T? _value = null;

        public static implicit operator T(OnReady<T> onready)
        {
            if (onready._value == null)
            {
                throw new NullReferenceException("Attempted to access an OnReady variable before the OnReady method");
            }

            return onready._value;
        }
    }
}
