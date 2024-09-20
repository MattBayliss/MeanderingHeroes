Should inherit from a base class, that is shared by monsters, NPCs, anything with some agency.

Heroes have [[Intents]] - a [[Player]] can add an Intent in some future GUI. Or the simulation will add some automatically.

Player examples:
- specific
	- go to that location
- vague 
	- hunt for food
	- kill monsters for fame and glory

System generated examples
- find a healer
- run from dragon that just appeared

As the Game processes Intents, it produces [[Events]]. Example Events:
- Hero arrived at their destination
- Prey spotted in the map cell you're passing through

[[Events]] then lead to [[Interrupts]]

1. If Hero passes into Monster's area (Event)
2. Monster will try to eat them (Interrupt)

1. If a blizzard occurs (Event)
2. Hero looks for shelter (Interrupt)

Interrupts may cause an Intent to be thwarted for this turn.

There will be Player defined Interrupts in the GUI - maybe like:
- IF `Event meets Condition` THEN `Interrupt`
- IF `Hero sees someone in distress` THEN `assist them`

And game engine defined ones
- IF `Monster fails morale check` THEN `monster try to run away`

Turns continue until the heat death of the universe? All heroes dead? 
