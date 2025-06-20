# CleanestHud

This is a revival & continuation of HIFU's CleanerHud mod, with permission. (proof at bottom of readme)


## Info
- Pretty much everything about the hud has been edited in some way.
- Bugs from the original CleanerHud mod are fixed.
- Uses your survivor's color throughout the hud.
- - This doesn't just include the one you're playing as, but any survivor you're spectating.
- Supports modded UI from many mods, including (but not limited to):
- - [Starstorm 2](https://thunderstore.io/package/TeamMoonstorm/Starstorm2/)
- - [LookingGlass](https://thunderstore.io/package/DropPod/LookingGlass/)
- - [Driver](https://thunderstore.io/package/public_ParticleSystem/Driver/)
- - [Myst](https://thunderstore.io/package/JavAngle/Myst/)
- Many parts of the hud are configurable.
- - You can use [Risk of Options](https://thunderstore.io/package/Rune580/Risk_Of_Options/) to configure these settings in-game.
- If a survivor you want to play with doesn't work well with the hud, there's a blacklist you can add them to to prevent the hud from doing changes
- Lots of modded multiplayer sessions were played while this was being developed, so I've made sure it works in multiplayer

Many elements of the hud are also configurable Most are for the hud in general, but some are for the survivor specific hud elements that seeker and void fiend have!

<sub><sup>also this isn't a finished release since i know there's still a few things that don't fully work, I just wanted to get this out since everything's like 90% working</sup></sub>


## Showcase

<sub>NOTE: Some elements of the HUD may look slightly different ingame due to mod updates. Screenshots will be updated when the mod is finished or I feel like there's been enough changes.</sub>

![20250314023946_1](https://github.com/user-attachments/assets/94455d18-acc4-4d40-b41e-3a1dd132b7b8)

<sub>(lemurian inventory shown using another mod)</sub>

![20250314025245_1](https://github.com/user-attachments/assets/76f20eed-24c0-4b2d-a41e-8bb16687ef64)

![20250314025430_1](https://github.com/user-attachments/assets/1e033eb2-0d17-4006-9376-fbc0b6437be1)


## Bugs/Suggestions
This isn't a finished release, so there's likely bugs that I've either already found or haven't yet found. If you do find a bug, or have a suggestion for the hud, either make a github issue here or ping me (lordvgames) in the RoR2 modding discord server.


## Known Issues
- Changing resolutions mid-stage doesn't re-scale the hud
- - Kind of a vanilla issue, especially when going from a big resolution to a small OnInitialHudColorEditsFinishedone, but I feel it should still be mentioned
- Having the "auto highlight when opening the inventory menu" setting disabled causes movement to stop when hovering over a player's inventory
- - Another vanilla issue that's caused immediately by that new option. This can be fixed by re-enabling the option, or it can be worked around by first clicking on an item icon
- Having the [HealthbarImmune](https://thunderstore.io/c/riskofrain2/p/DestroyedClone/HealthbarImmune/) mod causes the hp bar text to disappear
- - May or may not be an issue from HealthbarImmune itself. In any case, installing [NoMoreMath](https://thunderstore.io/package/Goorakh/NoMoreMath/) seems to fix it, even with the "effective health" setting off


## For mod developers
The mod includes some events you can subscribe to that correlate to when certain stages of the HUD editing process are done. These are, in order of completion:
- `HudChanges.HudStructure.OnHudStructureEditsFinished`
- `HudChanges.HudDetails.OnHudDetailEditsFinished`
- `Main.OnSurvivorSpecificHudEditsFinished` (repeatable)
- `HudChanges.HudColor.OnHudColorUpdate` (repeatable)
The repeatable events happen when the camera/HUD's target changes, so any changes that depend on a specific survivor should go in either. `OnSurvivorSpecificHudEditsFinished` will always happen after a camera target change, and `OnHudColorUpdate` will try to run on every camera change, but the event will only be raised after successfully changing the HUD's color.


## <sub><sup>yes, i did get permission to continue the mod</sup></sub>

![image](https://github.com/user-attachments/assets/131bd210-4f3b-42ea-a0cf-ebf7ae7db98f)
