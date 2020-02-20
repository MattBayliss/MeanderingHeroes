open Fuchu
open MeanderingHeroes.Engine

let testGrid = 
    let intgrid = seq { 1 .. 10 }
    seq { 1 .. 10 } |> Seq.map (
        fun x -> 
            seq { 1 .. 10 } |> Seq.map (fun y -> { coords = { x = double x; y = double y }; terrain = Terrain.Grass })
        )

let tests =
    testList "Move Test Group" [
        testCase "one test" <|
            fun _ -> Assert.Equal("2+2", 4, 2+2)
        testCase "another test" <|
            fun _ -> Assert.Equal("3+3", 6, 3+3)
    ]

[<EntryPoint>]
let main args = defaultMainThisAssembly args