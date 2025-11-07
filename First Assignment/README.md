# RVA G1 - First Assignment Report

## Students’ Identification

| Name | Student Nr. | Email |
| --- | --- | --- |
| Manuel Alves | up201906910 | up201906910@up.pt |
| Pedro Marcelino | up202108754 | up202108754@up.pt |
| Rodrigo Esteves | up202403070 | up202403070@up.pt |

## Project Instructions


- Unity Editor Version: 6000.2.6f
- AR SDK: Vuforia Engine
- Markers Used: 4 markers in total
    - 1 marker displays the Laser (Oxygen)
    - 3 markers correspond to colored frames (Red, Green, Blue)
- These markers can be found at the following [link](https://developer.vuforia.com/library/assets/images/target_samples/unity/mars_target_images.pdf)

## Project Overview

The project consists of an Augmented Reality arcade-style game where the player uses a
laser to destroy as many asteroids as possible within one minute.

## Game Features

**Objective** 

The player has 60 seconds to destroy as many asteroids as possible. Asteroids
can only be destroyed if the laser color matches the asteroid color. The laser color is
changed by passing the laser through colored frames.

**Start & Aiming**

When the player starts the game from the Main Menu, the AR scene is loaded and a
60-second timer begins. The laser becomes visible and active when the camera detects the
Laser (Oxygen) marker.

The laser shoots forward according to the marker’s orientation. Aiming is achieved by
rotating or moving the marker in front of the camera, or by moving the device around the
marker. If the marker goes out of view or tracking is lost, the laser is temporarily disabled;
pointing the camera back at the marker reactivates it.


**Color Mixing System**
The game uses a simple additive color system to make the player interact with the other 3 markers.

- The laser starts in a white state.
- When the laser passes through a colored frame marker, it adds that color channel to the laser.

Frames do not give points; they only change the laser’s color so that the player can destroy specific asteroids.

There are 3 frame markers, each one corresponding to a primary color:

- Red Frame: adds Red to the laser
- Green Frame: adds Green to the laser
- Blue Frame: adds Blue to the laser

If the player passes through more than one frame before shooting, the colors combine into a secondary color, the order does not matter:

- Red + Green: Yellow
- Red + Blue: Magenta
- Green + Blue: Cyan

**Asteroids and Target Matching**

Asteroids are spawned around the laser marker in AR and each asteroid is spawned with a
specific target color which can be either a primary color (Red, Green, or Blue) or a
secondary color (Yellow, Magenta, or Cyan).
Whenever the laser’s raycast collides with an asteroid, the system first verifies whether the
current laser color matches the asteroid’s color. If this condition is satisfied, the asteroid is
destroyed, triggering a visual explosion or pop effect, playing a sound, and increasing the
player’s score according to the color tier of the asteroid.

**Scoring**

The scoring system is designed to reward the player more based on the complexity of
making the color. Destroying a primary-color asteroid (Red, Green, or Blue) grants 1 point, while destroying a secondary-color asteroid (Yellow, Magenta, or Cyan), which will require a
combination of two different frame colors, grants 3 points.

**Timer and End of Round**

Each match is limited to a 60-second round. The time elapsed is shown on the UI, and when
the timer reaches 1 minute the game ends and the End Screen is shown with the
Leaderboard screen. From the leaderboard, the player can conveniently return to the Main
Menu to start a new round.



