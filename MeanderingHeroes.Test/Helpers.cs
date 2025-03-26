using LaYumba.Functional;
using MeanderingHeroes.Engine.Types;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Test
{
    internal static class Helpers
    {
        internal static Grid MakeGrass10x10MapGrid()
        {
            var terrain = Range(0, 9)
                .SelectMany(q => Range(0, 9)
                    .Select(r => (Hex: new Hex(q, r), Terrain: (Terrain)(new LandTerrain("grass", 1)))));

            return new Grid(terrain);
        }
        internal static Grid GenerateMapFromAsciiMess(string asciiMap)
        {
            var mapCodes = asciiMap
            .Split("\r\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .ToArray();

            var width = mapCodes.Min(line => line.Length);
            var height = mapCodes.Count();

            var terrain = Range(0, width - 1)
                .SelectMany(q =>
                    Range(0, height - 1)
                        .Select(r => (
                            Hex: new Hex(q, r),
                            Terrain: (Terrain)(mapCodes[r][q] switch
                            {
                                "~" => new WaterTerrain("ocean", 10),
                                "_" => new LandTerrain("grass", 1),
                                "^" => new LandTerrain("hill", 2),
                                "M" => new LandTerrain("mountain", 10),
                                "v" => new LandTerrain("swamp", 5),
                                "T" => new LandTerrain("forest", 3),
                                _ => new LandTerrain("UNEXPECTED", 9999)
                            }))
                            )
                        );

            return new Grid(terrain);
        }
        internal static Grid MakeTestMap2Grid()
        {
            // legend: _ = grass, ^ = hill, T = forest, ~ = water, M = mountains v = swamp
            string testMap =
"""
~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ 
 ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~
~ ~ ~ ~ ~ _ _ _ ~ _ _ _ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ 
 ~ ~ ~ ~ - ^ _ _ _ ^ ^ ^ ^ _ _ ~ _ _ ^ _ _ ~ ~ ~ _ _ _ ~ ~ ~ ~ ~ ~
~ ~ ~ ~ _ _ _ _ _ T ^ ^ M _ _ _ T _ _ _ _ _ _ _ _ _ _ _ _ _ _ ~ ~ 
 ~ ~ _ _ _ _ _ _ _ _ _ M M ^ T T _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ ~ ~
_ _ _ _ _ _ _ _ _ _ _ _ ^ M ^ ^ T T T _ _ _ _ _ _ _ _ _ _ _ _ ~ ~ 
 _ _ _ _ _ _ _ _ _ _ _ ^ M ^ ^ T T T _ _ _ _ _ _ _ _ _ ~ ~ ~ ~ ~ ~
_ _ _ _ _ _ _ _ _ _ _ _ M M ^ ^ _ T T T T _ _ _ _ _ _ _ ~ ~ ~ ~ ~ 
 _ _ _ _ _ _ _ _ _ _ _ _ M M ^ _ _ _ _ _ _ _ _ _ _ _ _ _ _ ~ ~ ~ ~
_ _ _ _ _ _ _ _ _ _ _ _ M ^ M ^ ^ _ _ _ _ _ _ _ _ _ _ _ _ ~ ~ ~ ~ 
 _ _ _ _ _ _ _ _ _ _ ^ ^ _ _ M M _ _ _ _ _ _ _ _ _ _ _ _ _ _ ~ ~ ~
_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ M _ _ _ _ _ _ _ _ _ _ _ _ _ ~ ~ ~ 
 _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ ^ ^ _ _ _ _ _ _ _ _ _ _ _ _ _ ~ ~
_ _ _ _ _ _ _ _ _ _ _ _ v v _ _ _ _ ^ _ _ _ _ _ _ _ _ _ _ _ _ _ ~ 
 _ _ _ _ _ _ _ _ _ _ _ v v v v _ _ ^ _ _ _ _ _ _ _ _ _ _ _ _ ~ ~ ~
~ ~ _ _ _ _ _ _ _ _ _ v v v _ _ _ _ _ _ _ _ _ _ _ _ _ ~ ~ ~ ~ ~ ~ 
 ~ ~ ~ ~ ~ ~ ~ _ _ _ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ _ _ _ _ _ _ _ ~ ~ ~ ~ ~ ~
~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ 
 ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~
""";
            return GenerateMapFromAsciiMess(testMap);
        }

        internal static T AssertIsSome<T>(Option<T> value)
        {
            if (value == None)
            {
                throw new ArgumentException($"Expected value to be Some, but was None");
            }
            return value.AsEnumerable().Single();
        }
    }
}
