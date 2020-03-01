//#load "C:\git\MeanderingHeroes\paket-files\mausch\Fuchu\Fuchu\Fuchu.fs"

open MeanderingHeroes.Engine
open Fuchu

let testGrid = 
    Array2D.init 10 10 (fun x y ->{ coords = { x = double (x + 1); y = double (y + 1) }; terrain = Terrain.Grass } )

let tests =
    testList "Move Test Group" [
        testCase "move east" <|
            fun _ ->
                // simulate the ticks we need until target is reached
                let mover = { id = 1; name = "Sally Walker"; position = { x = 1.0; y = 1.0 }; health = 1.0F; speed = 1.0F }; 
                let instruction = {agentId = 1; instruction = Move {x=10.0; y= 1.0}; state = Pending}
                let moveWorldState = { agents = [ mover ]; runningInstructions = []; tick = 0; mapGrid = testGrid };
                // tick 1
                let (state1, events) = WorldTick moveWorldState [ instruction ]
                Assert.Equal("tick incremented", 1, state1.tick)

        testCase "another test" <|
            fun _ -> Assert.Equal("3+3", 6, 3+3)
    ]

[<EntryPoint>]
let main args = defaultMainThisAssembly args