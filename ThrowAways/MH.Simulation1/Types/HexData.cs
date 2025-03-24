using MeanderingHeroes.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MH.Simulation1.Types
{
    public record struct HexData(Hex Hex, Godot.Vector2 Offset, Terrain Terrain);
}
