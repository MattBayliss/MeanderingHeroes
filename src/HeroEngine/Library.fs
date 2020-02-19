namespace MeanderingHeroes

module Say =
    let hello name =
        printfn "Hello %s" name

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
    }

    type Terrain = Grass=0 | Forest=1

    type GridSquare = {
        coords : Point; // squares will be in a 2 dimension array, so this is kind of duplicated info, but will be used to calculate distances?
        terrain : Terrain;
        // temperature could be a function of coords, day of the year (WordState.tick), and a broader weather system function?
    }

    type WorldState = {
        agents : Agent list;
        tick : int32;
        mapGrid : seq<seq<GridSquare>>;
    }

    type Instruction = {
        agentId : int32;
        command : string;
    }

    let WorldTick (currentState : WorldState) (instructions : Instruction list) = 
        "helllo"

    