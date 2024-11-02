
```c#

delegate Doers[] Reaction(Func<Event, bool> Trigger, Func<Doer, Doer> ActionToTake)

(Event, Reaction) => Doers[]

```