# Meandering Heroes

## Overview

Broadly, Meandering Heroes will be a Fantasy Simulation game, crossed with a [Play by Mail game](https://en.wikipedia.org/wiki/Play-by-mail_game) from yesteryear (that I remember fondly). Big goal is that MH will be a living, persistent multi-player world where a player is in control of a guild, and gives their members (Heroes) instructions and goals, and a rich A.I. will determine the success or failure of your Heroes. It will be designed for players to log in, read the reports of what has occurred since their last visit, and give new orders as required. Orders can take days or weeks real-time to resolve.

Well, that's the current ultimate goal. It will start a bit less ambitious than that.

**First MVP** - one Hero, random Monsters, see what happens?

## Why?

This idea has been kicking around for a while. The project started an an experiment to learn F# (and that abandoned project is here too), but now I'm going to attempt it in C#, using techniques I learnt about in the following books:

- [Functional Programming with C# by Simon J. Painter](https://www.oreilly.com/library/view/functional-programming-with/9781492097068/)
- [Functional Programming in C#, Second Edition by Enrico Buonanno](https://www.manning.com/books/functional-programming-in-c-sharp-second-edition)

Assuming I keep at it, perhaps I can rewrite the core engine in F# some day.

## Milestones

I'm trying to be realistic, and breaking it down to small (achievable?) steps in the form of these milestones:

- [x] Make a basic Move test - give a Hero an a destination and see them move to that destination
- [ ] Make a hunter-prey test - a hero will hunt a deer for food - the deer will try not to be food
- [ ] Add hunger mechanics - test to see if Hero will try to find food (hunt or forage) to stay alive
