# The Level 0
### *Overview*

The Level 0 is a first-person 3D psychological experience where the player is trapped in an endless, eerie sequence of elevator stops and office hallways late at night. Blurring the line between reality and surrealism, the game challenges the player's perception through subtle, randomized anomalies and a controlling narrator. Player must figure out what happened and a way to escape this nightmare loop. But is there really an exit?

This project was built using Unity and focuses on creating a concise, high-impact horror loop with an emphasis on atmosphere, player agency, and the technical implementation of surreal, looping environments.

### *Game Objective*

The core objective is to escape the building (reach the ground floor) by figuring out the secret code, the correct sequence of numbers, and inputting it into the elevator's numpad.

### *How to Play*

`Walk Forward:` The main loop involves walking out of the elevator and navigating the hallway, which uses a progression counter and teleportation logic to create the illusion of endless progression, to find the next elevator (or door).

`Observe Closely:` Pay attention to every detail in the environment. Subtle changes are often the most important.

`Detect Anomalies:` If you spot something unusual (an "anomaly"), your progression might depend on reversing your direction or moving to the next floor quickly.

`Find the Hints:` Search the environment for the viewfinder/flashlight object, random hints, and surroundings for context which is essential for revealing the hidden numerical sequence.

`Interaction:` Left Click to interact with the environment. Move mouse for direction. WASD/Direction Key for movement.

`Enter the Code:` Once you have the full code sequence, interact with the elevator numpad to enter it and attempt your escape.

`Decision-Based Progression:` Player choices (e.g., following instructions, avoiding anomalies) directly affect the narrative path, leading to either a repeated loop or an "ending" transition.

### *Features*

### Technical & Visual Highlights

- Stencil Buffer Hint System: Implemented a unique mechanic where the player uses a flashlight/viewfinder to reveal hidden objects or hints along the walls, providing crucial information for the escape.

- Dynamic Psychological Visuals: Post-processing effects (Vignette, Chromatic Aberration, Film Grain) dynamically update as the game progresses, visually representing the player's descent into hysteria and the blurring of reality.

- Dynamic Narration System: A custom narration manager utilizes Scriptable Objects (Presets) to deliver unique, state-tracking dialogues and wall texts that change based on the player's current floor and game progression.

- Complex Anomaly Behaviors: Includes advanced anomaly logic, such as a walking NPC with a transition between normal and a "glitch" animation state, and anomalies that dynamically track and react to the player's position.

- Optimized Level Reuse: Four core scenes are reused across nine different floors, with dynamic elements and narration profiles ensuring that each visit feels distinct and progresses the story.

- Dynamic Audio: Dynamic sound effects and ambience, controlled by coroutines and random audio clips, enhance the eerie atmosphere.


### *Inspiration*

- The Exit 8: The core inspiration for the "spot the anomaly and reverse your course" loop.

- The Stanley Parable: Inspired the concept of a controlling, fourth-wall-breaking narrator who psychologically influences the player's choices and narrative progression.

- Other Psychological Horror Games: Drawing on themes of surreal, paradoxical, and limited vertical spaces, common in psychological horror and "liminal space" games.

