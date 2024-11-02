Events will have a [[Noise]] level - based on the Doer / Hero's activity / Intent, any relevant skills or attributes, and a random factor.

[[Reactions]] will have a [[Threshold]], where the [[Noise]] has to be over a certain amount, and the amount over, plus the stats of the Doer reacting, plus a random factor will determine in the Reaction triggers.

```c#

record Event(Intent fromIntent, Doer source)

Func<Event, Func<Event, bool>, Event> Triggered

```