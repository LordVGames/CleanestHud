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
- - [HUDdleUP](https://thunderstore.io/package/itsschwer/HUDdleUP/)
- - [Driver](https://thunderstore.io/package/public_ParticleSystem/Driver/)
- - [Myst](https://thunderstore.io/package/JavAngle/Myst/)
- Many parts of the hud are configurable.
- - You can use [Risk of Options](https://thunderstore.io/package/Rune580/Risk_Of_Options/) to configure these settings in-game.
- If a survivor you want to play with doesn't work well with the hud, there's a blacklist you can add them to to prevent the hud from doing changes
- Lots of modded multiplayer sessions were played while this was being developed, so I've made sure it works in multiplayer

Many elements of the hud are also configurable Most are for the hud in general, but some are for the survivor specific hud elements that seeker and void fiend have!

## Screenshots Showcase

<details>
<summary>Expand</summary>

![20250719144026_1](https://github.com/user-attachments/assets/0d30989d-e9d9-4bb2-91fa-b416d10fa246)

![20250719144412_1](https://github.com/user-attachments/assets/c1aeb896-7c52-4b80-8c6c-ff5824c92917)

![20250719144446_1](https://github.com/user-attachments/assets/8a5d5522-cd47-4f30-ae30-da41923cea9d)

![20250719144530_1](https://github.com/user-attachments/assets/276bd978-1ef3-4c46-b844-fe7ced2c52c2)

### Multiplayer

![oEgXQq6zmr](https://github.com/user-attachments/assets/ed19b46f-640c-4a5d-8d8a-17c380f2df8a)


### Modded Survivor: Driver

![20250719144604_1](https://github.com/user-attachments/assets/17d92117-a181-45be-b6f0-5af8cf1e9f83)


</details>

## Bugs/Suggestions
If you find a bug, or have a suggestion for the hud, either make a github issue or ping me (lordvgames) in the RoR2 modding discord server.

## Known Issues
- Changing resolutions mid-stage doesn't re-scale the hud
- - Kind of a vanilla issue, especially when going from a big resolution to a small one, but I feel it should still be mentioned
- Having the [HealthbarImmune](https://thunderstore.io/c/riskofrain2/p/DestroyedClone/HealthbarImmune/) mod causes the hp bar text to disappear
- - May or may not be an issue from HealthbarImmune itself. In any case, installing [NoMoreMath](https://thunderstore.io/package/Goorakh/NoMoreMath/) seems to fix it, even with the "effective health" setting off
- Sometimes Myst's HUD doesn't appear
- - It's a semi-rare error that randomly happens and it comes straight from Myst itself. Can't really fix it until Myst itself adds support for this HUD.
- Some part of the HUD is broken when an editing phase is turned off
- - The HUD was built assuming parts of it's editing process weren't turned off, especially structure edits. It'd be a ton of work to go back and make sure everything works with those options, so any bugs stemming from these options likely won't be fixed.

## For mod developers
The mod includes some events you can subscribe to that correlate to when certain stages of the HUD editing process are done. These are, in order of completion:
- `HudChanges.HudStructure.OnHudStructureEditsFinished`
- `HudChanges.HudDetails.OnHudDetailEditsFinished`
- `Main.OnSurvivorSpecificHudEditsFinished` (repeatable)
- `HudChanges.HudColor.OnHudColorUpdate` (repeatable)
The repeatable events happen when the camera/HUD's target changes, so any changes that depend on a specific survivor should go in either. `OnSurvivorSpecificHudEditsFinished` will always happen after a camera target change, and `OnHudColorUpdate` will try to run on every camera change, but the event will only be raised after successfully changing the HUD's color.
- Many config options have an event for when they're toggled on/off.

You can look at the existing code for this mod's survivor HUD support for examples on how to use these.

## <sub><sup>yes, i did get permission to continue the mod</sup></sub>

![image](https://github.com/user-attachments/assets/131bd210-4f3b-42ea-a0cf-ebf7ae7db98f)
