namespace MeanderingHeroes

module Engine =
    type Point = {
        x : double;
        y : double;
    }

    type Agent = {
        id  : int32;
        name: string;
        position: Point;
        health: single;
        speed: single;
    }

    type InstructionState =
    | Pending
    | Running of int32
    | Completed of int32

    type InstructionType =
    | Move of Point // destination

    type AgentInstruction = {
        agentId : int32;
        instruction : InstructionType;
        state: InstructionState;
    }

    type Terrain = Grass=0 | Forest=1

    type GridSquare = {
        coords : Point; // squares will be in a 2 dimension array, so this is kind of duplicated info, but will be used to calculate distances?
        terrain : Terrain;
    }
    // temperature could be a function of coords, day of the year (WordState.tick), and a broader weather system function?

    type WorldState = {
        agents : Agent list;
        runningInstructions : AgentInstruction list;
        tick : int32;
        mapGrid : GridSquare[,];
    }

    type EventType =
    | MoveEvent of from:Point * dest:Point

    type Event = {
        tick : int32;
        agentId : int32;
        eventType: EventType;
    }

    let terrainMovementCost terrain =
        match terrain with
            | Terrain.Grass -> 1.0F
            | Terrain.Forest -> 1.5F
            | _ -> 2.0F
    
    let distance (a, b):double = 
        sqrt (a*a + b*b)    

    let moveInstruction worldState agentId target =
        let agent = List.find (fun a -> a.id = agentId ) <| worldState.agents
        let (x, y) = (target.x - agent.position.x, target.y - agent.position.y)
        let d = distance (x, y)
        // identity vector - direction we need to travel, with a length of 1
        // (one day we'll actually need to find the best path)
        let (ix, iy) = (x/d, y/d)
        { id = agent.id; name = agent.name; position = {x = ix * (double agent.speed); y = iy * (double agent.speed)}; speed = agent.speed; health = agent.health }     


    let WorldTick (currentState : WorldState) (newInstructions : AgentInstruction list) = 

        let currentTick = currentState.tick + 1
        let newinsts = newInstructions 
                       |> List.map (fun ni -> { 
                           agentId = ni.agentId; 
                           instruction = ni.instruction;
                           state = Running currentTick 
                           })
        let isRunning instruction = match instruction.state with | Running _ -> true | _ -> false
        let instructions = currentState.runningInstructions @ newinsts |> List.filter isRunning

        let instructionsForAgentId = instructions |> List.groupBy (fun i -> i.agentId)

        let processInstructionForAgent agent instruction tick =
            match instruction.instruction with
            | Move dest ->            
                let (x, y) = (dest.x - agent.position.x, dest.y - agent.position.y)
                let d = distance (x, y)
                let (np, ni) = match d with
                                | a when a <= double agent.speed -> 
                                    (
                                        dest,
                                        {agentId = instruction.agentId; instruction = instruction.instruction; state = Completed tick }
                                    )
                                | _ ->  ({
                                            x = agent.position.x + ((double agent.speed) * x/d);
                                            y = agent.position.y + ((double agent.speed) * y/d)
                                        }, instruction)

                let updatedAgent = { id = agent.id; name = agent.name; position = np; speed = agent.speed; health = agent.health }

                let event = {agentId = agent.id; tick = tick; eventType = MoveEvent (agent.position, np)}

                (updatedAgent, ni, event)

        let replaceAgentIdWithAgent (id, instructions) =
            let agentOpt = (currentState.agents |> List.tryFind (fun a -> a.id = id))
            match agentOpt with
            | None -> None
            | Some agent -> Some (agent, instructions)

        let instructionsForAgent = instructionsForAgentId |> List.choose replaceAgentIdWithAgent

        let processInstructionsForAgent agent instructions =
            let rec recIListForAgent agent instructions acc = 
                match instructions with
                | []     -> acc
                | h :: t -> let (updatedAgent, updatedInstruction, event) = processInstructionForAgent agent h currentTick
                            let (_, instructions, events) = acc
                            recIListForAgent updatedAgent t (updatedAgent, instructions @ [updatedInstruction], events @ [event])

            recIListForAgent agent instructions (agent, [], [])

        let processAgents agentInstructionsPairs =
            let rec recProcessAgents aipairs acc =
                match aipairs with
                | [] -> acc
                | (agent, ilist) :: ailtail -> let (uagent, uinstructions, nevents) = processInstructionsForAgent agent ilist                                 
                                               let (agents, instructions, events) = acc
                                               recProcessAgents ailtail (agents @ [uagent], instructions @ uinstructions, events @ nevents)


            recProcessAgents instructionsForAgent ([], [], [])

        let (agents, instructions, events) = processAgents instructionsForAgent
        
        ({ agents = agents; runningInstructions = instructions; tick = currentTick; mapGrid = currentState.mapGrid }, events)              
