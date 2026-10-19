using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using static CleanestHud.HudResources.ImportantHudTransforms;
using MonoDetour.HookGen;
using MonoDetour;
namespace CleanestHud.HudChanges.NormalHud.HealthBarArea;


[MonoDetourTargets(typeof(RoR2.UI.HealthBar))]
internal static class HealthBar
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudStructure.OnHudStructureEditsBegun += HudStructureEdits;
        HudDetails.OnHudDetailEditsBegun += RemoveBadHealthSubBarFromPersonalHealthBar;
        Mdh.RoR2.UI.HealthBar.InitializeHealthBar.Prefix(ImproveHealthBarTextVisuals);
    }


    internal static void HudStructureEdits()
    {
		Image healthBarRootImage = HealthBarRoot.GetComponent<Image>();
		healthBarRootImage.sprite = HudAssets.WhiteSprite;

		Transform shrunkenRoot = HealthBarRoot.Find("ShrunkenRoot");
		if (shrunkenRoot.childCount != 0)
		{
			Transform badBar1 = shrunkenRoot.GetChild(0);
			badBar1.gameObject.SetActive(false);
			Transform badBar2 = shrunkenRoot.GetChild(1);
			badBar2.gameObject.SetActive(false);


			Transform barrierBar = shrunkenRoot.GetChild(3);
			barrierBar.localPosition = new Vector3(
				barrierBar.localPosition.x,
				-1,
				barrierBar.localPosition.z
			);
			barrierBar.localScale = new Vector3(1, 0.7f, 1);
		}
	}


    internal static void RemoveBadHealthSubBarFromPersonalHealthBar()
    {
        Transform shrunkenRoot = ImportantHudTransforms.BarRoots.Find("HealthbarRoot").Find("ShrunkenRoot");
        Image badHpBarImage0 = shrunkenRoot.GetChild(0).GetComponent<Image>();
        Image badHpBarImage1 = shrunkenRoot.GetChild(1).GetComponent<Image>();
        // these images CAN sometimes be null and i don't want an error message to appear if it is
        // so we're not using DisableImageComponent here
        badHpBarImage0?.enabled = false;
        badHpBarImage1?.enabled = false;
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