I should narrow the scope of models affected, need to better facilitate unit testing

Generically,
```
State => Command => Event

State => Event => State
```


```

Doer => Command => Event

Event => Reaction => Event


Hero => MoveIntent => ArrivedEvent

ArrivedEvent => AmbushReaction => AttackEvent

Hero => HuntIntent => Hero { with attributes: [Stealthy, Alert]}
Prey => GrazeIntent => Prey {with attributes: [Alert, Foraging]}

```

How should a turn work?

Each Doer's Command may trigger an Event - allow other Doers to respond to Event with Reactions, leading to new Commands?

OR

Allow all Doers to act in a turn, then process all Reactions, then rollback a Doer's Events if they were interupted... no.. too messy

Make a turn = 1 hour in game world 

Order of processing in a Turn:

- Movement
- Arrived Reactions
- Resulting Commands (flee, fight)
- Conflict Resolution
- Outcome Events

Does a Command like Movement get broken down into turn-sized chunks for events to occur?

```
1 turn = 1 hour



```
