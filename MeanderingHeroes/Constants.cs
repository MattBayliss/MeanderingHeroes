using HexCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeanderingHeroes
{
    public static class Constants
    {
        public static readonly TerrainType GrassTerrain;
        public static readonly TerrainType ForestTerrain;
        public static readonly MovementType Walking;
        public static readonly MovementType Running;
        public static readonly MovementType Swimming;
        public static readonly IImmutableList<TerrainType> TerrainTypes;
        public static readonly IImmutableList<MovementType> MovementTypes;
        public static readonly MovementTypes MovementAndTerrainTypes;

        static Constants()
        {
            GrassTerrain = new TerrainType(1, "Grass");
            ForestTerrain = new TerrainType(2, "Forest");

            TerrainTypes = [GrassTerrain, ForestTerrain];

            Walking = new MovementType(1, "Walking");
            Running = new MovementType(2, "Running");
            Swimming = new MovementType(3, "Swimming");

            MovementTypes = [Walking, Running, Swimming];

            MovementAndTerrainTypes = new MovementTypes(
                [.. TerrainTypes],
                new Dictionary<MovementType, Dictionary<TerrainType, int>>
                {
                    [Walking] = new Dictionary<TerrainType, int>
                    {
                        [GrassTerrain] = 3,
                        [ForestTerrain] = 2
                    },
                    [Running] = new Dictionary<TerrainType, int>
                    {
                        [GrassTerrain] = 4,
                        [ForestTerrain] = 2
                    }
                });
        }
    }
}
