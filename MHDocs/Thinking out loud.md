I should narrow the scope of models affected, need to better facilitate unit testing

Generically,
```
State => Command => Event

State => Event => State
```


```

Doer => Intent => Event

Event => Reaction => Event


Hero => MoveIntent => ArrivedEvent

ArrivedEvent => AmbushReaction => AttackEvent

Hero => HuntIntent => Hero { with attributes: [Stealthy, Alert]}
Prey => GrazeIntent => Prey {with attributes: [Alert, Foraging]}


```