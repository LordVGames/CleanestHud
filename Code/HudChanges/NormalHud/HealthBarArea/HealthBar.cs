using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using MonoDetour.HookGen;
using MonoDetour;
namespace CleanestHud.HudChanges.NormalHud.HealthBarArea;


[MonoDetourTargets(typeof(RoR2.UI.HealthBar))]
internal static class HealthBar
{
    internal static void HudStructureEdits()
    {
        // TODO what is GetChild(1) here?
        Transform healthBarRoot = ImportantHudTransforms.BarRoots.GetChild(1);

        Image healthBarRootImage = healthBarRoot.GetComponent<Image>();
        healthBarRootImage.sprite = HudAssets.WhiteSprite;

        Transform shrunkenRoot = healthBarRoot.Find("ShrunkenRoot");
        if (shrunkenRoot.childCount != 0)
        {
            Transform child1 = shrunkenRoot.GetChild(0);
            child1.gameObject.SetActive(false);
            Transform child2 = shrunkenRoot.GetChild(1);
            child2.gameObject.SetActive(false);
        }
    }




    [MonoDetourHookInitialize]
    private static void SetupHooks()
    {
        Mdh.RoR2.UI.HealthBar.InitializeHealthBar.Prefix(ImproveHealthBarTextVisuals);
    }
    private static void ImproveHealthBarTextVisuals(RoR2.UI.HealthBar self)
    {
        // only edit the player's healthbar which has a special name
        if (self.name != "HealthbarRoot")
        {
            return;
        }
        Log.Debug("HealthBar_InitializeHealthBar");



        // sots added a new way of showing the hp bar's text which is sadly lower quality than the original
        // luckily we can just disable the new way and it will fall back to the old higher quality way
        self.spriteAsNumberManager = null;
        // but it doesn't appear by default so we need to toggle the old one off and the new one on
        Transform managedSpriteHP = self.transform.Find("Managed_Sprite_HP");
        if (managedSpriteHP != null)
        {
            managedSpriteHP.gameObject.SetActive(false);
        }
        else
        {
            Log.Debug("Couldn't find managedSpriteHP! This is OK though since it isn't needed for the HUD.");
        }
        Transform slash = self.transform.Find("Slash");
        if (slash != null)
        {
            slash.gameObject.SetActive(true);
        }
        else
        {
            Log.Error("Couldn't find \"Slash\" on the HP bar! Another mod may have already disabled/removed it. This means the improved HP bar text will not be shown, and no text will be shown if \"managedSpriteHP\" was successfully set to inactive. Report this on github!");
        }
    }
}