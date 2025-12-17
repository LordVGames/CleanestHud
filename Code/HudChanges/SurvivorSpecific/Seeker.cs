using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using RoR2.UI;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.SurvivorSpecific;


internal static class Seeker
{
    private static bool IsHudCameraTargetSeeker
    {
        get
        {
            // not using .name because that includes (Clone) and i don't care about putting that in every get method like this
            return HudCameraTargetBody?.baseNameToken == "SEEKER_BODY_NAME";
        }
    }


    internal static void RepositionSeekerLotusUI()
    {
        if (!IsHudEditable || !IsHudCameraTargetSeeker)
        {
            return;
        }

        Transform bottomCenterCluster = MyHudLocator.FindChild("BottomCenterCluster");
        Transform bottomCenterClusterScaler = bottomCenterCluster.GetChild(2);
        Transform utilityArea = bottomCenterClusterScaler.GetChild(1);
        Transform utilityAreaDisplayRoot = utilityArea.GetChild(0);
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
        utilityAreaDisplayRoot.Find("SeekerLotusUI(Clone)").localPosition = newLotusLocalPosition;
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
}