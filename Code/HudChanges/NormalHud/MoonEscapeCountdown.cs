using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using RoR2.UI;
using System;
using UnityEngine;
using static CleanestHud.HudResources;
using static CleanestHud.HudResources.ImportantHudTransforms;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.NormalHud;


[MonoDetourTargets(typeof(EscapeSequenceController))]
internal static class MoonEscapeCountdown
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.EscapeSequenceController.SetHudCountdownEnabled.Postfix(SetHudCountdownEnabled);
    }

    private static void SetHudCountdownEnabled(EscapeSequenceController self, ref HUD hud, ref bool shouldEnableCountdownPanel)
    {
        if (!IsHudEditable || TopCenterCluster == null)
        {
            return;
        }


        Transform hudCountdownPanel = MyHudLocator.FindChild("TopCenterCluster").Find("HudCountdownPanel(Clone)");
        if (hudCountdownPanel == null)
        {
            Log.Warning("Couldn't find HudCountdownPanel for editing???");
            return;
        }

        hudCountdownPanel.localPosition = new Vector3(0, -200f, 0f);

        // panel > Juice > Container
        Transform container = hudCountdownPanel.GetChild(0).GetChild(0);

        container.Find("Backdrop").DisableImageComponent();
        container.Find("Border").DisableImageComponent();

        Transform countdownTitleLabel = container.Find("CountdownTitleLabel");
        HGTextMeshProUGUI countdownTitleLabelMesh = countdownTitleLabel.GetComponent<HGTextMeshProUGUI>();
        countdownTitleLabelMesh.fontSharedMaterial = HudAssets.FontMaterial;
        countdownTitleLabelMesh.color = Color.red;

        Transform countdownLabel = container.Find("CountdownLabel");
        HGTextMeshProUGUI countdownLabelMesh = countdownLabel.GetComponent<HGTextMeshProUGUI>();
        countdownLabelMesh.fontSharedMaterial = HudAssets.FontMaterial;
        countdownLabelMesh.color = Color.red;
    }
}