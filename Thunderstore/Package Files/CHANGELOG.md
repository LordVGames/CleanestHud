## 0.11.0

Fixed ambient "Lv." text not being changed to "Level"

Changed ambient level & stage count text to be completely white

Added a config option for showing the section labels in the inventories screen (off by default)

Fixed a missing space before the right cloud in all boss subtitles
- AFAIK this has always been a thing in vanilla, now finally fixed

Made boss hp bar coloring perfectly cover the background
- Before this, the background would be peeking out a little bit on both ends of the bar

## 0.10.2

Fixed potential error related to item icon coloring

Fixed errors when LookingGlass isn't installed
- The hud is meant to be fully usable without it, I just messed some things up when it comes to handling if the mod is running or not

## 0.10.1

Fixed error spam caused by some other mod messing with the hp bar
- I genuinely have no clue what mod was causing this, but the fix was easy

## 0.10.0

Rewrote a lot of positioning code
- This means that everything should be properly positioned no matter the screen size, be it 4:3 or ultrawide

Item highlights now get survivor colored using a better method
- Should be more performant at high item counts so the config option for this is now on by default.

The item icon & inventory strip highlight textures have been changed to a cleaner one

Tweaked the inventories menu item display
- Item icons are more spaced out (there's practically no space between icons in vanilla at high item counts)
- Item icons display should take up more height in a player's inventories menu section

Fixed an extra background detail for the item icon in the inspect not being removed

Fixed background for the inspect button reminder not being removed

Centered player names on the inventory menu
- This also includes some support for LookingGlass' item counters setting

Evenly spaced out skill & equipment icons

Tweaked SS2 composite injector equipment slot positions
- Slots past the 10th start at the 5th instead of the first
- - This makes it not overlap with the hp bar at ultrawide resolutions

Made HP bar text smoother

Changed "Lv: ##" text to "Level ##"

Fixed NRE when opening inventories menu when someone had no items

Fixed consistent difficulty bar coloring not working past the start of "HAHAHAHA"

Fixed positioning of the "Items" label in the inventories menu

## 0.9.0

First release
- Not everything is finished though