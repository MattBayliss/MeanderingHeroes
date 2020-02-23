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

    type Terrain = Grass=0 | Forest=1

    type GridSquare = {
        coords : Point; // squares will be in a 2 dimension array, so this is kind of duplicated info, but will be used to calculate distances?
        terrain : Terrain;
    }
    // temperature could be a function of coords, day of the year (WordState.tick), and a broader weather system function?

    type WorldState = {
        agents : Agent list;
        tick : int32;
        mapGrid : GridSquare[,];
    }

    type Instruction = {
        agentId : int32;
        command : string;
    }

    type Event = {
        agentId : int32;
        from: Point;
        dest: Point;
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

    let WorldTick (currentState : WorldState) (instructions : Instruction list) : WorldState * Event list = 
        { agents = currentState.agents; tick = currentState.tick + 1; mapGrid = currentState.mapGrid }, []

