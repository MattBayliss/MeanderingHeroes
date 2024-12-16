
A Reaction gets triggered by an Event
- if `Trigger(event) == true`, run `ActionToTake(event.Doer)`, which returns the new state of `event.Doer`, and the new state of the `Doer` reacting
- i.e. if a Hunter attacks some Prey, the new states might be:
	- both Hunter and Prey wounded, 
	- Hunter wounded, Prey dead
	- Hunter exhausted, Prey escaped
	- Hunter and Prey exhausted

```c#

delegate (Doer EventDoer, Doer Reactor) Reaction(Func<Event, bool> Trigger, Func<Doer, (Doer EventDoer, Doer Reactor)> ActionToTake)

```

**Too complicated - pivot to Hero vs Hex**