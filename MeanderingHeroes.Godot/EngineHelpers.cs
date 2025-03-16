using Godot;
using MeanderingHeroes.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Godot
{
    public static class EngineHelpers
    {
        public static Hex ToHex(this Vector2I v) => 
            (
                v.X - (v.Y - (v.Y & 1)) / 2,    // q
                v.Y                             // r
            );
    }
}
