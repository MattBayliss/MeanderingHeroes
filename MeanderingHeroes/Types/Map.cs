using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Types
{
    public record Location(float X, float Y)
    {
        public static implicit operator Vector2(Location location) => new Vector2(location.X, location.Y);
        public static implicit operator Location(Vector2 vector) => new Location(vector.X, vector.Y);
    }
}
