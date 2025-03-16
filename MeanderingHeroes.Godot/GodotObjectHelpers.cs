using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Godot
{
    /// <summary>
    /// In order to minimise interop & marshalling - cache core objects in C#
    /// </summary>
    public record GodotObject<T>(string Id, T Value);
    public static class GodotObjectHelpers
    {

    }
}
