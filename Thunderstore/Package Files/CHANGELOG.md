## 0.10.0

Rewrote a lot of positioning code so that hud elements are (hopefully) properly aligned at any resolution, including ultrawide ones

Item highlights now get survivor colored using a better method
- Should be more performant at high item counts so the config option for this is now on by default.

The item icon & inventory strip highlight textures have been changed to a cleaner one

Tweaked the inventories menu item display
- Item icons are more spaced out (there's practically no space between icons in vanilla at high item counts)
- Item icons display should take up more height in a player's inventories menu section

Fixed an extra background detail for the item icon in the inspect not being removed
- Only noticed it recently as the color difference for this detail and the rest of the inspect panel is *very* slight

(Hopefully) Centered player names on the inventory menu
- This also includes some support for LookingGlass' item counters setting

Made HP bar text smoother

Changed "Lv: ##" text to "Level ##"

Fixed NRE when opening inventories menu when someone had no items

Fixed consistent difficulty bar coloring not working past the start of "HAHAHAHA"

Fixed positioning of the "Items" label in the inventories menu

## 0.9.0

First release
- Not everything is finished though