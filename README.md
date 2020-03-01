# Meandering Heroes

## Overview

Broadly, Meandering Heroes will be a Fantasy RTS, but a "game" can take months or years. MH will be a living, persistent multi-player world where a player is in control of a guild, and gives their members (Heroes) instructions and goals, and a rich A.I. will determine the success or failure of your Heroes. It will be designed for players to log in, read the reports of what has occurred since their last visit, and give new orders as required. Orders can take days or weeks real-time to resolve.

Well, that's z the ultimate goal. It will start a bit less ambitious than that.

It is also a vechile to learn some more of the following technologies:

* Reactive extensions
* F# (for A.I. and queue processing)
* SAFE stack?

## High level type definitions


## High level functions

### WorldTick

`WorldState -> Instruction list -> WorldState`

Each 'tick' of the world will increment time in the *WorldState*. Basic AI will move things along. If there are any actual player instructions this tick, they are added to the World State. I want the WorldTick to be pretty dumb. PCs, NPCs, nations, might all have more complex AIs that generate *Instructions*, that are then passed to the WorldTick function.

## Milestones

1. Design basic architecture and get a "move" action unit test to pass
2. Create Map to show progress of your Hero

## Dev notes for Milestone 1

* User will give a command, like move my guy to (1, 10)
* an "intent" will persist for how many ticks, to get that guy to (1, 10)
* once the destination is reached, the intent is cleared.
* each tick will generate events for that guy, so that the user can see a report, or a UI can update, etc. An observable for events?
* If I'm trying to be immutable, then WorldState would be a new instance every time. What does that mean for memory? What if the WorldState is HUGE?
* Should instructions instead apply to the Agents, or user guided pieces?
* Every part of the process is mutable it seems. What are the immutable parts?
	* the map grid
	* the agent (hero|npc|animals) entities, but not their current state (health, location, certain stats)
	* events - what happened to agents are snapshots of time

* **Fuck it.** I'm just going to create new copies of agents and worldstates each time for this first cut. Read current state from a database or flat-file, process all instructions, write back to persistent storage. Done. Maybe.


## Goals / Rainy day ideas / Brain farts

* People can make their own bots that read WorldState or events and generate their own instructions?


## References

### Reactive extensions

* https://fsharpforfunandprofit.com/posts/concurrency-reactive/

### Unit test tools

* https://github.com/mausch/Fuchu
* https://fscheck.github.io/FsCheck/QuickStart.html