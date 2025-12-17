using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources.ImportantHudTransforms;
using RoR2.UI;
using MonoDetour.HookGen;
using MonoDetour;
using MonoDetour.Cil;
using MonoMod.Cil;
namespace CleanestHud.HudChanges.NormalHud;


internal static class BossHpBar
{
    internal static void HudStructureEdits()
    {
        Transform bossHealthBarRoot = TopCenterCluster.Find("BossHealthBarRoot");
        Transform bossHealthBarRootRect = bossHealthBarRoot.GetComponent<RectTransform>();
        bossHealthBarRootRect.localPosition = new Vector3(0f, -160, -3.8f);
        bossHealthBarRoot.TryDestroyComponent<VerticalLayoutGroup>();

        Transform bossContainer = bossHealthBarRoot.Find("Container");
        bossContainer.TryDestroyComponent<VerticalLayoutGroup>();

        Transform bossHealthBarContainer = bossContainer.Find("BossHealthBarContainer");
        RectTransform bossHealthBarContainerRect = bossHealthBarContainer.GetComponent<RectTransform>();
        bossHealthBarContainerRect.localPosition = new Vector3(0, 25, 0);

        Transform bossNameLabel = bossContainer.Find("BossNameLabel");
        RectTransform bossNameLabelRect = bossNameLabel.GetComponent<RectTransform>();
        bossNameLabelRect.localPosition = new Vector3(0f, 32.5f, 0f);

        Transform bossSubtitleLabel = bossContainer.Find("BossSubtitleLabel");
        RectTransform bossSubtitleLabelRect = bossSubtitleLabel.GetComponent<RectTransform>();
        bossSubtitleLabelRect.localPosition = new Vector3(0f, -40f, 0f);

        HGTextMeshProUGUI bossSubtitleLabelMesh = bossSubtitleLabel.GetComponent<HGTextMeshProUGUI>();
        bossSubtitleLabelMesh.color = Color.white;

        Transform bossBackgroundPanel = bossHealthBarContainer.Find("BackgroundPanel");
        RectTransform bossBackgroundPanelRect = bossBackgroundPanel.GetComponent<RectTransform>();
        bossBackgroundPanelRect.localPosition = new Vector3(0f, -42.5f, 0f);
        bossBackgroundPanelRect.localScale = new Vector3(1f, 1.5f, 1f);

        // make the colored bars completely cover the background
        Vector3 newFillPanelScale = new(1.02f, 1, 1);

        Transform delayFillPanel = bossBackgroundPanel.Find("DelayFillPanel");
        delayFillPanel.localScale = newFillPanelScale;
        Image delayFillPanelImage = delayFillPanel.GetComponent<Image>();
        delayFillPanelImage.color = new Color32(138, 0, 0, 255);

        Transform fillPanel = bossBackgroundPanel.GetChild(1);
        fillPanel.localScale = newFillPanelScale;

        Transform shieldPanel = bossBackgroundPanel.Find("ShieldPanel");
        Transform healthText = bossBackgroundPanel.Find("HealthText");
        RectTransform healthTextRect = healthText.GetComponent<RectTransform>();
        healthTextRect.localPosition = new Vector3(0f, 11.5f, -5.85840205f);
        healthTextRect.localEulerAngles = Vector3.zero;
        healthTextRect.localScale = new Vector3(1.25f, 0.8325f, 1.25f);

        int shieldPanelSiblingIndex = shieldPanel.GetSiblingIndex();
        int healthTextSiblingIndex = healthText.GetSiblingIndex();
        healthText.SetSiblingIndex(shieldPanelSiblingIndex);
        shieldPanel.SetSiblingIndex(healthTextSiblingIndex);
    }


    internal static void HudDetailEdits()
    {
        BossHealthBarContainer.DisableImageComponent();
        Image bossBackgroundPanelImage = BossHealthBarContainer.Find("BackgroundPanel").GetComponent<Image>();
        // this is set to true on purpose to enable dark background for missing boss hp
        bossBackgroundPanelImage.enabled = true;
    }


    [MonoDetourTargets(typeof(BossGroup))]
    private static class SubtitleCloudFix
    {
        [MonoDetourHookInitialize]
        private static void Setup()
        {
            Mdh.RoR2.BossGroup.UpdateObservations.ILHook(UpdateObservations);
        }


        private static void UpdateObservations(ILManipulationInfo info)
        {
            ILWeaver w = new(info);
            w.MatchRelaxed(
                x => x.MatchLdstr("<sprite name=\"CloudRight\" tint=1>") && w.SetCurrentTo(x)
            ).ThrowIfFailure()
            .InsertAfterCurrent(
                w.CreateDelegateCall((string cloudRightString) =>
                {
                    // hopoo forgot to put an extra space before the right cloud
                    // then they never fixed it lol
                    return cloudRightString.Insert(0, " ");
                })
            );
        }
    }
}