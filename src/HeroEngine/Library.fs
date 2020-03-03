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


    let WorldTick (currentState : WorldState) (newInstructions : AgentInstruction list) : WorldState * Event list = 

        let nextTick = currentState.tick + 1
        let newinsts = newInstructions 
                       |> List.map (fun ni -> { 
                           agentId = ni.agentId; 
                           instruction = ni.instruction;
                           state = Running nextTick 
                           })
        let instructions = currentState.runningInstructions @ newinsts

        let rec doInstructions instructions agents acc =
            let (accInsts, accAgents) = acc
            match agents with
            | [] -> (accInsts, accAgents @ agents)
            | agent :: agentsTail ->              
                let rec getInstructionsForAgent aid ainstructions iaacc =
                    match ainstructions with
                    | [] -> iaacc
                    | ihead :: itail ->
                        let (ifa, others) = iaacc
                        if ihead.agentId = aid then
                            getInstructionsForAgent aid itail (ifa @ ihead, others)
                        else
                            getInstructionsForAgent aid itail (ifa, others @ ihead)
                
                let (instructionsForAgent, otherInsts) = getInstructionsForAgent agent.id instructions ([], [])
                
                // process instructionsForAgent

                acc
        let (newInsts, newAgents) =  doInstructions instructions currentState.agents ([], [])
        { agents = newAgents; runningInstructions = newInstructions;  }




        instructions |> List.map (
            fun i ->
                let agent = currentState.agents |> List.map (fun a -> a.id = i.agentId)
                match i.instruction with
                | Move dest -> 
        )

        

        currentState.runningInstructions
        { agents = currentState.agents; runningInstructions = currentState.runningInstructions @ newInstructions; tick = currentState.tick + 1; mapGrid = currentState.mapGrid }, []

