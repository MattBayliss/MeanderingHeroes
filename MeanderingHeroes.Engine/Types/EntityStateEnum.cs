using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    // make the Godot animation names use the same text, "Idle", "Walking" etc 
    // for easy parsing
    public enum EntityStateEnum
    {
        Idle,
        Walking
    }
}
