using EntityStates.Seeker;
using Mdh.EntityStates.Seeker.Meditate;
using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using RoR2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.HudResources.ImportantHudTransforms;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.SurvivorSpecific;


internal static class Seeker
{
    private static bool IsHudEditable
    {
        get
        {
            return Main.IsHudEditable
            && ConfigOptions.AllowSurvivorSpecificEdits.Value
            && HudCameraTargetBody != null
            && HudCameraTargetBody.bodyIndex == DLC2Content.BodyPrefabs.SeekerBody.bodyIndex;
        }
    }


    internal static void RepositionSeekerLotusUI()
    {
        if (!IsHudEditable)
        {
            return;
        }


        // this is ran twice when spawning in as seeker but it errors out here the first time?????
        // it works the 2nd time though
        Vector3 newLotusLocalPosition = Vector3.zero;
        switch (ConfigOptions.SeekerLotusHudPosition.Value)
        {
            case ConfigOptions.SpecialConfig.SeekerLotusHudPosition.AboveSkillsMiddle:
                newLotusLocalPosition = new Vector3(98, -85, 0);
                    
                break;
            case ConfigOptions.SpecialConfig.SeekerLotusHudPosition.LeftOfSkills:
                newLotusLocalPosition = new Vector3(-82, -150, 0);
                break;
        }
        newLotusLocalPosition.y += ConfigOptions.ShowSkillKeybinds.Value ? 0 : 18;
        UtilityAreaDisplayRoot.Find("SeekerLotusUI(Clone)").localPosition = newLotusLocalPosition;
    }


    internal static void RepositionSeekerMeditationUI()
    {
        try
        {
            Transform seekerMeditateUi = MyHudLocator.FindChild("CrosshairExtras").Find("SeekerMeditateUI(Clone)");
            switch (ConfigOptions.SeekerMeditateHudPosition.Value)
            {
                case ConfigOptions.SpecialConfig.SeekerMeditateHudPosition.AboveHealthBar:
                    seekerMeditateUi.localPosition = new Vector3(2.5f, 265, 0);
                    break;
                case ConfigOptions.SpecialConfig.SeekerMeditateHudPosition.OverCrosshair:
                    seekerMeditateUi.localPosition = new Vector3(2.5f, 400, 0);
                    break;
            }
        }
        catch
        {
            Log.Debug("Couldn't reposition seeker meditation hud!");
            return;
        }
    }


    [MonoDetourTargets(typeof(Meditate))]
    private static class Hooks
    {
        [MonoDetourHookInitialize]
        private static void Setup()
        {
            Mdh.EntityStates.Seeker.Meditate.SetupInputUIIcons.Postfix(OnMeditate);
        }


        private static void OnMeditate(Meditate self)
        {
            if (IsHudUserBlacklisted)
            {
                return;
            }


            RepositionSeekerMeditationUI();
        }
    }
}