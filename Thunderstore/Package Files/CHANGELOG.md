## 1.1.0

Added support for some of HUDdleUP's HUD elements
Fixed a few HUD elements not actually being edited
Fixed Void Fiend's corruption meter being stuck mid-squish if its animations are stopped while it's in the middle of its squishing animation

## 1.0.4

Fixed error related to editing ally hp bars

## 1.0.3

Fixed moon detonation timer panel not being aligned correctly

## 1.0.2

Fixed mod trying to setup the editor component for the suppressed items UI when it shouldn't

## 1.0.1

Fixed Seeker's meditation UI not changing position

## 1.0.0

Added support for special HUD elements from the following mod characters:
- Driver
- Myst

Fixed many bugs:
- Fixed consistent difficulty bar coloring not working
- Fixed Seeker's lotus being slightly off center
- Fixed dead players in the inventories menu preventing styling for other scoreboard strips below it
- - Part of support for [RestoreScoreboard](https://thunderstore.io/c/riskofrain2/p/itsschwer/RestoreScoreboard/)
- Fixed the panel showing monsters' items having it's background removed when playing as a character that was blacklisted from the hud
- Fixed the suppressed items panel not being styled correctly
- - It still jitters a bit if you click on an item but I don't know if there's anything I can do about that
- Fixed item icons in the TAB menu still getting full outline changes while playing a character who is blacklisted
- - It's still changed slightly but there's nothing I can do about it since I have to edit the item icon asset
- Fixed HP bars sometimes being too light-green
- Fixed MUL-T's other equipment slot getting it's own little key prompt
- Fixed the position of some HUD elements that were slightly off
- Fixed equipment slots looking a little weird
- Fixed end screen's chat box not being styled

Reduced the number of `.Find` calls
- This should (if it works how I think it does) decrease the time to do all the HUD edits when loading into a stage, possibly decreasing stage load time a tiny bit

Heavily re-organized code

Added code events in the mod for other devs to know when each stage of CleanestHud's edits are finished
- Useful for if a modded HUD element copies off of a vanilla HUD element that gets edited by the HUD after the mod's HUD element spawns
- There are also events for when some config options change
- Read the README for more info

## 0.11.5

Fixed for SOTS phase 3

Fixed buffs list slowing becoming more off-center the longer the list is

Made another attempt to fix the hud color sometimes being wrong in multiplayer
- Untested, but might actually fix it

Repositioned seeker lotus UI position a bit so it doesn't overlap the sprint & inventory keybind reminders


## 0.11.4

Fixed SkillsPlusPlus causing the HUD editing process to die

Made color change apply a frame after the camera target change happens, similar to survivor-specific HUD element edits
- This is another attempt to fix another player's color being applied in multiplayer but I have good feeling about this one

## 0.11.3

Recompiled using latest dependencies
- The RoR2-related dependencies for the mod's code weren't updated since SOTS initial release, so this may fix some weird errors related to UI

## 0.11.2

Reworked coloring system a bit
- This should hopefully also fix the HUD sometimes using a different player's color in multiplayer

Hopefully fixed NRE related to item icon coloring

Fixed the player's name going to the middle of their inventory section when LookingGlass isn't installed

Fixed consistent difficulty bar coloring not working at the "HAHAHAHA" segment for real this time

Slightly moved the items list in the inventories menu

## 0.11.1

Fixed a specific setup with the DPSMeter mod causing this mod to die

## 0.11.0

Renamed many config options, double check your settings!

Added a config options for:
- Showing the section labels in the inventories screen (off by default)
- Showing the outlines for skill icons (on by default)
- - This will hopefully support equipment icons too in the future

Fixed ambient level "Lv." text not being changed to "Level"

Changed ambient level & stage count text to be completely white

Fixed a missing space before the right cloud in the boss subtitle
- AFAIK this has always been a thing in vanilla, now it's finally fixed

Made boss hp bar coloring perfectly cover the background
- Before this, the background would be peeking out a little bit on both ends of the bar

Removed outline from inspect panel

Fixed xp bar being a *tiny* bit misaligned from the hp bar

Fixed buff bar still being moved for blacklisted character bodies

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