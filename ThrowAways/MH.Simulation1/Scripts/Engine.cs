using Godot;
using Microsoft.VisualBasic.FileIO;
using System;

public partial class Engine : Node
{
    [Signal]
    public delegate void EngineTickEventHandler();

    public override void _Ready()
    {
        
    }


}