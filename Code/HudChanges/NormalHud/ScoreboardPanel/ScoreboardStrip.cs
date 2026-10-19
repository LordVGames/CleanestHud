using MiscFixes.Modules;
using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using RoR2.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.HudResources;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.NormalHud.ScoreboardPanel;


[MonoDetourTargets]
internal static class ScoreboardStrips
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudDetails.OnHudDetailEditsBegun += HudDetailEdits;
    }


    internal static void HudDetailEdits()
    {
        ImportantHudTransforms.ScoreboardStripContainer.DisableImageComponent();
    }




    internal static void EditscoreboardStripEquipmentSlotHighlight(ScoreboardStrip scoreboardStrip)
    {
        Transform equipmentBackground = scoreboardStrip.equipmentIcon.transform;
        Transform navFocusHighlight = equipmentBackground.GetChild(1);

        RawImage equipmentIconNavFocusHighlightRawImage = navFocusHighlight.GetComponent<RawImage>();
        equipmentIconNavFocusHighlightRawImage.texture = ModAssets.AssetBundle.LoadAsset<Texture2D>("NewNavHighlight");

        HGButton button = equipmentBackground.GetComponent<HGButton>();
        button.m_Colors.highlightedColor = Helpers.ChangeColorWhileKeepingAlpha(button.m_Colors.highlightedColor, Color.white);
        button.m_Colors.normalColor = Helpers.ChangeColorWhileKeepingAlpha(button.m_Colors.normalColor, Color.white);
        button.m_Colors.pressedColor = Helpers.ChangeColorWhileKeepingAlpha(button.m_Colors.pressedColor, Color.white);
        button.m_Colors.selectedColor = Helpers.ChangeColorWhileKeepingAlpha(button.m_Colors.selectedColor, Color.white);
        button.m_Colors.m_ColorMultiplier = 1.5f;
    }
}