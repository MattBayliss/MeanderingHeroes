using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace MeanderingHeroes
{
    public record Map(Cell[][] Cells);

    public record Cell(Terrain Terrain, Weather CurrentWeather, ReadOnlyDictionary<Direction, TravelCost> TravelCosts);
    public record Weather;
    public record Terrain;
    public record TravelCost;
    public enum Direction { North, South, East, West }
}
