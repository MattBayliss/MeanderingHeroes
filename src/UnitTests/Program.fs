//#load "..\..\paket-files\mausch\Fuchu\Fuchu\Fuchu.fs"
//#load @"..\HeroEngine\Library.fs"

open MeanderingHeroes.Engine
open Fuchu

let testGrid = 
    Array2D.init 10 10 (fun x y ->{ coords = { x = double (x + 1); y = double (y + 1) }; terrain = Terrain.Grass } )

let tests =
    testList "Move Test Group" [
        testCase "move east" <|
            fun _ ->
                // simulate the ticks we need until target is reached
                let start = { x = 1.0; y = 1.0 }
                let destination = {x=10.0; y= 1.0}
                let mover = { id = 1; name = "Sally Walker"; position = start; health = 1.0F; speed = 1.0F }; 
                let instruction = {agentId = 1; instruction = Move destination; state = Pending}
                let moveWorldState = { agents = [ mover ]; runningInstructions = []; tick = 0; mapGrid = testGrid };
                // tick 1
                let (state1, events) = WorldTick moveWorldState [ instruction ]
                Assert.Equal("tick incremented", 1, state1.tick)
                Assert.Equal("one event", 1, events.Length)
                let event = List.head events
                Assert.Equal("Event for agent 1", mover.id, event.agentId)
                Assert.Equal("Event for tick 1", 1, event.tick)
                match event.eventType with
                | MoveEvent (f, t) -> Assert.Equal("Move event from (1,1) to (2,1)", ({x=1.0; y=1.0}, {x=2.0; y=1.0;}), (f, t))
                let runningInstructionsTick1 = state1.runningInstructions
                Assert.Equal("1 running instruction", 1, runningInstructionsTick1.Length)
                let rinstruction1 = List.head runningInstructionsTick1
                match rinstruction1.state with
                | Running tick -> Assert.Equal("instruction started on tick 1", 1, tick);
                | Completed ct -> Assert.Equal("instruction should not be complete", 0, ct);
                | Pending -> Assert.Equal("instruction should not be pending", 0, 1);
                let agenttick1 = List.find (fun agent -> agent.id = mover.id) state1.agents
                Assert.Equal("WorldState agent has moved to (2, 1)", {x=2.0; y=1.0;}, agenttick1.position)
                // progress to halfway (tick 4)
                let (state2, _) = WorldTick state1 []
                let (state3, _) = WorldTick state2 []
                let (state4, events4) = WorldTick state3 []

                let runUntilNoEvents startState = 
                    let rec recMoveStates lastState (astates, aevents) =
                        match lastState with
                        | state when List.isEmpty state.runningInstructions -> (astates, aevents)
                        | state -> let (nextState, newEvents) = WorldTick state [] 
                                   recMoveStates nextState (astates @ [state], aevents @ newEvents ) 
                    recMoveStates startState ([], [])

                let (moveStates, moveEvents) = runUntilNoEvents state1

                Assert.Equal("last event is at destination", destination, List.last moveEvents |> fun lastEvent -> match lastEvent.eventType with | MoveEvent (a, b) -> b )


                Assert.Equal("at 4th tick", 4, state4.tick)
                Assert.Equal("one event", 1, events.Length)
                let event = List.head events4
                Assert.Equal("Event for agent 1", mover.id, event.agentId)
                Assert.Equal("Event for tick 4", 4, event.tick)
                match event.eventType with
                | MoveEvent (f, t) -> Assert.Equal("Move event from (4,1) to (5,1)", ({x=4.0; y=1.0}, {x=5.0; y=1.0;}), (f, t))
                let runningInstructionsTick4 = state4.runningInstructions
                Assert.Equal("1 running instruction", 1, runningInstructionsTick4.Length)
                let rinstruction4 = List.head runningInstructionsTick4
                match rinstruction4.state with
                | Running tick -> Assert.Equal("instruction started on tick 1", 1, tick);
                | Completed ct -> Assert.Equal("instruction should not be complete", 0, ct);
                | Pending -> Assert.Equal("instruction should not be pending", 0, 1);
                let agenttick4 = List.find (fun agent -> agent.id = mover.id) state4.agents
                Assert.Equal("WorldState agent has moved to (5, 1)", {x=5.0; y=1.0;}, agenttick4.position)

        testCase "another test" <|
            fun _ -> Assert.Equal("3+3", 6, 3+3)
    ]

[<EntryPoint>]
let main args = defaultMainThisAssembly args