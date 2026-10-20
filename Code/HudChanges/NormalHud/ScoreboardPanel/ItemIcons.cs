using Mdh.RoR2.UI.KeyItemIcon;
using Mono.Cecil.Cil;
using MonoDetour;
using MonoDetour.Cil;
using MonoDetour.HookGen;
using MonoMod.Cil;
using RoR2.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static RoR2.BodyGravityForce;
namespace CleanestHud.HudChanges.NormalHud.ScoreboardPanel;


[MonoDetourTargets(typeof(ItemIcon))]
[MonoDetourTargets(typeof(KeyItemIcon))]
internal static class ItemIcons
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.UI.ItemIcon.SetItemIndex_RoR2_UI_ItemIcon_SetItemIndexArgs.Postfix(SetItemIndex3);
        // can't really subscribe to the event from this for some reason
        // also why does this run constantly while the tab menu is upppppp
        Mdh.RoR2.UI.KeyItemIcon.ModifyDisplayData.Postfix(EditKeyItemIcon);
    }


    private static void EditKeyItemIcon(KeyItemIcon keyItemIcon, ref KeyItemIcon.DisplayData newDisplayData)
    {
        if (newDisplayData.Equals(in keyItemIcon.currentDisplayData))
        {
            return;
        }
        if (!keyItemIcon.transform.GetChild(1).TryGetComponent<RawImage>(out RawImage navFocusHighlightRawImage))
        {
            // this happens with the key item icon by the skills hud
            return;
        }


        Texture2D newNavHighlight = ModAssets.AssetBundle.LoadAsset<Texture2D>("NewNavHighlight");
        if (navFocusHighlightRawImage.texture != newNavHighlight)
        {
            navFocusHighlightRawImage.texture = newNavHighlight;
        }
        ColorKeyItemIcon(keyItemIcon);
    }
    internal static void ColorKeyItemIcon(KeyItemIcon keyItemIcon)
    {
        // GetChild(1) is NavFocusHighlight
        if (!keyItemIcon.transform.GetChild(1).TryGetComponent<RawImage>(out RawImage navFocusHighlightRawImage))
        {
            return;
        }


        Color colorToUse;
        if (keyItemIcon.targetBody == null)
        {
            Color longBackgroundColor = keyItemIcon.transform.parent.GetComponent<Image>().color;
            longBackgroundColor.a = 1; // restore original survivor body color
            colorToUse = longBackgroundColor;
        }
        else
        {
            colorToUse = Helpers.GetAdjustedColor(keyItemIcon.targetBody.bodyColor);
        }


        if (!Helpers.AreColorsEqualIgnoringAlpha(navFocusHighlightRawImage.color, colorToUse))
        {
            navFocusHighlightRawImage.color = colorToUse;
        }
    }


    private static void SetItemIndex3(ItemIcon itemIcon, ref ItemIcon.SetItemIndexArgs args)
    {
        if (IsHudUserBlacklisted || itemIcon == null || itemIcon.image == null || itemIcon.name != "ItemIconScoreboard_InGame(Clone)")
        {
            return;
        }


        // doing this doesn't actually cause that much lag and only happens for whatever icons are added/updated
        // it also makes future mass glowimage coloring better on performance
        itemIcon.glowImage = itemIcon.transform.GetChild(1).GetComponent<RawImage>();
        HudColor.ColorSingleItemIconHighlight(itemIcon);
    }
}