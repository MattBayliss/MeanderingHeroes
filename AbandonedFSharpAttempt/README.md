# Meandering Heroes

## Overview

Broadly, Meandering Heroes will be a Fantasy RTS, but a "game" can take months or years. MH will be a living, persistent multi-player world where a player is in control of a guild, and gives their members (Heroes) instructions and goals, and a rich A.I. will determine the success or failure of your Heroes. It will be designed for players to log in, read the reports of what has occurred since their last visit, and give new orders as required. Orders can take days or weeks real-time to resolve.

Well, that's the ultimate goal. It will start a bit less ambitious than that.

It is also a vechile to learn some more of the following technologies:

* [Reactive extensions][2]
* F# (for A.I. and queue processing)
* UI: [Bolero (Blazor for F# kinda)][1]

## High level type definitions


## High level functions

### WorldTick

`WorldState -> Instruction list -> WorldState`

Each 'tick' of the world will increment time in the *WorldState*. Basic AI will move things along. If there are any actual player instructions this tick, they are added to the World State. I want the WorldTick to be pretty dumb. PCs, NPCs, nations, might all have more complex AIs that generate *Instructions*, that are then passed to the WorldTick function.

## Milestones

1. ~~Design basic architecture and get a "move" action unit test to pass~~
1. Make a hunter-prey test - hunter will 'see' prey, chase it, attack it. Considerations:
	1. Perception distance
	1. Prey evasion, threat detection, - running - random, always away
	1. Sprinting + exhaustion
	1. Attacking + health + wounded
	1. Hunger? Satiated stat, food supplies

1. Create Map UI

## Goals / Rainy day ideas / Brain farts

* People can make their own bots that read WorldState or events and generate their own instructions?


## References

### UI development

* [Bolero: F# in WebAssembly][1]

### Reactive extensions

* [fsharpforfunandprofit.com/posts/concurrency-reactive/][2]

### Unit test tools

* https://github.com/mausch/Fuchu
* https://fscheck.github.io/FsCheck/QuickStart.html

[1]: https://fsbolero.io/
[2]: https://fsharpforfunandprofit.com/posts/concurrency-reactive/