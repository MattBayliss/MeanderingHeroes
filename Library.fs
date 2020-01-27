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

    type WorldState = {
        agents : Agent list;
        tick : int32;
    }

    type Instruction = {
        agentId : int32;
        command : string;
    }

    let WorldTick (currentState : WorldState) (instructions : Instruction list) = 
        "helllo"
