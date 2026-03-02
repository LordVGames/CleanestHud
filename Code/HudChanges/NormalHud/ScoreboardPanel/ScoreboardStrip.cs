using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using RoR2.UI;
namespace CleanestHud.HudChanges.NormalHud.ScoreboardPanel;


internal static class ScoreboardStrips
{
    internal static void HudDetailEdits()
    {
        ImportantHudTransforms.ScoreboardStripContainer.DisableImageComponent();
    }


    internal static void EditScoreboardStripEquipmentSlotHighlight(ScoreboardStrip scoreboardStrip)
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