open Fuchu
open MeanderingHeroes.Engine

let testGrid = 
    Array2D.init 10 10 (fun x y ->{ coords = { x = double (x + 1); y = double (y + 1) }; terrain = Terrain.Grass } )

let initialTestWorldState = { agents = List.Empty; tick = 1; mapGrid = testGrid }

let tests =
    testList "Move Test Group" [
        testCase "move east" <|
            fun _ ->
                // simulate the ticks we need until target is reached
                let mover = { id = 1; name = "Sally Walker"; position = { x = 1.0; y = 1.0 }; health = 1.0F; speed = 1.0F }; 
                let instruction = moveInstruction mover (10.0, 1.0)
                let moveWorldState = { agents = [ mover ]; tick = 1; mapGrid = testGrid };

                // tick 1
                let state1 = MeanderingHeroes.Engine.WorldTick moveWorldState [ instruction ]

                
                
                Assert.Equal("2+2", 4, 2+2)
        testCase "another test" <|
            fun _ -> Assert.Equal("3+3", 6, 3+3)
    ]

[<EntryPoint>]
let main args = defaultMainThisAssembly args