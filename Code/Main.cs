using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mono.Cecil.Cil;
using MonoDetour;
using MonoDetour.Cil;
using MonoDetour.DetourTypes;
using MonoDetour.HookGen;
using MonoMod.Cil;
using RoR2;
using RoR2.UI;
using System.Linq;
using CleanestHud.HudChanges;
using static CleanestHud.HudResources;
using MiscFixes.Modules;
using RoR2.CameraModes;
using RoR2.ConVar;
namespace CleanestHud;


public static class Main
{
    public static HUD MyHud = null;
    public static ChildLocator MyHudLocator = null;
    public static bool IsHudFinishedLoading = false;
    public static bool IsHudUserBlacklisted = true;
    public static bool IsHudEditable
    {
        get
        {
            //Log.Warning($"IsHudFinishedLoading is {IsHudFinishedLoading}");
            //Log.Warning($"IsHudUserBlacklisted is {IsHudUserBlacklisted}");
            //Log.Warning($"MyHudLocator is {MyHudLocator}");
            return IsHudFinishedLoading && !IsHudUserBlacklisted && MyHudLocator != null;
        }
    }
    internal static bool IsColorChangeCoroutineWaiting = false;
    internal static List<Color> LastKnownScoreboardBodyColors = [];
    internal static CharacterBody HudCameraTargetBody
    {
        get
        {
            if (!MyHud)
            {
                Log.Error("HUD did not exist when trying to get HudCameraTargetBody!");
                return null;
            }
            if (!MyHud.targetBodyObject)
            {
                Log.Error("HUD did not have a valid targetBodyObject!");
                return null;
            }
            return MyHud.cameraRigController.targetBody;
        }
    }



    /// <summary>
    /// The third phase of editing the HUD. This is for coloring various HUD elements based on the body color of the camera target.
    /// </summary>
    public static event Action OnHudColorEditsBegun;



    /// <summary>
    /// When the hud has loaded and is (should be) ready for edits, before the other HUD edit events have fired. Set variables for needed HUD elements with this.
    /// </summary>
    public static event Action OnHudInit;



    [MonoDetourTargets(typeof(HUD))]
    private static class HudAwakeAndDestroy
    {
        [MonoDetourHookInitialize]
        private static void Setup()
        {
            Mdh.RoR2.UI.HUD.Awake.Postfix(Awake);
            Mdh.RoR2.UI.HUD.OnDestroy.Postfix(OnDestroy);
        }

        
        private static void Awake(HUD hud)
        {
            Log.Debug("HUD_Awake");
            MyHud = hud;
            MyHudLocator = MyHud.GetComponent<ChildLocator>();

            // for SOME reason the hp bar's sub bars don't exist on AWAKE????
            // so we need to wait and keep checking until they do exist, THEN and ONLY THEN can we safely do all our hud changes
            Transform hpBarShrunkenRoot = MyHudLocator.FindChild("BottomLeftCluster").Find("BarRoots/HealthbarRoot/ShrunkenRoot");
            MyHud?.StartCoroutine(WaitForHpBarToFinishLoading(hpBarShrunkenRoot));
        }
        private static IEnumerator WaitForHpBarToFinishLoading(Transform hpBarShrunkenRoot)
        {
            while (hpBarShrunkenRoot.childCount == 0)
            {
                Log.Debug("HUD HP bar does not have any children to modify yet, waiting");
                yield return null;
            }

            BeginLiveHudChanges();
        }
        private static void BeginLiveHudChanges()
        {
            IsHudFinishedLoading = true;


            // storing HudCameraTargetBody to prevent doing extra GetComponent calls from getting HudCameraTargetBody
            CharacterBody targetBody = HudCameraTargetBody;


            // removing "(Clone)" from the end of the name
            string properBodyName = targetBody.name[..^7];
            if (ConfigOptions.BodyNameBlacklist_Array.Contains(properBodyName))
            {
                Log.Info($"Character body {properBodyName} is blacklisted from the HUD! 99% of HUD changes will not occur.");
                IsHudUserBlacklisted = true;
                return;
            }
            else
            {
                Log.Debug($"Character body {properBodyName} is NOT blacklisted from the HUD!");
                IsHudUserBlacklisted = false;
            }

            // sometimes an extra sub bar gets perma enabled??? it makes the healthbar a very light green and i don't want that
            /* HudDetails.RemoveBadHealthSubBarFromPersonalHealthBar();
            // this can also happen to all allycards, but it won't be handled in AllyCardController_Awake for already existing allycards when loading in because IsHudUserBlacklisted isn't changed to false yet
            // so they have to manually be changed now
            MyHud?.StartCoroutine(HudDetails.DelayRemoveBadHealthSubBarFromAllAllyCards()); */

            if (!IsHudUserBlacklisted)
            {
                ImportantHudTransforms.FindImportantHudTransforms();
                OnHudInit?.Invoke();
                HudStructure.BeginEdits();
                HudDetails.BeginEdits();
                // HUD coloring is handled in CameraTargetHooks under DelayOnCameraChange
            }


            CanvasGroup wholeHudCanvasGroup = MyHud.GetOrAddComponent<CanvasGroup>();
            wholeHudCanvasGroup.alpha = ConfigOptions.HudTransparency.Value;
        }


        private static void OnDestroy(HUD hud)
        {
            IsHudFinishedLoading = false;
            IsHudUserBlacklisted = true;
            IsColorChangeCoroutineWaiting = false;
            HudColor.SurvivorColor = Color.clear;
        }
    }



    [MonoDetourTargets(typeof(CameraModeBase))]
    internal static class CameraTargetHooks
    {
        [MonoDetourHookInitialize]
        private static void Setup()
        {
            Mdh.RoR2.CameraModes.CameraModeBase.OnTargetChanged.Postfix(OnTargetChanged);
        }


        private static void OnTargetChanged(CameraModeBase self, ref CameraRigController cameraRigController, ref CameraModeBase.OnTargetChangedArgs args)
        {
            if (!cameraRigController.targetBody)
            {
                Log.Debug("cameraRigController.targetBody was invalid? returning");
                return;
            }
            if (MyHud != null)
            {
                MyHud?.StartCoroutine(DelayOnCameraChange(cameraRigController));
            }
            else
            {
                Log.Debug("MyHud was null in CameraModeBase_OnTargetChanged somehow, not doing DelayOnCameraChange");
            }
        }
        private static IEnumerator DelayOnCameraChange(CameraRigController cameraRigController)
        {
            // delay a frame to make sure everything that needs to be changed/setup has been (i.e. survivor specific ui)
            yield return null;
            Log.Debug("DelayOnCameraChange");
            if (cameraRigController.targetBody == null)
            {
                Log.Error("targetCharacterBody WAS NULL IN DelayOnCameraChange! NO HUD CHANGES WILL OCCUR!");
                yield break;
            }


            if (ConfigOptions.AllowHudStructureEdits.Value)
            {
                HudChanges.NormalHud.SpectatorLabel.MoveSpectatorLabel();
            }
            if (ConfigOptions.AllowSurvivorSpecificEdits.Value)
            {
                HudChanges.SurvivorSpecific.SurvivorSpecific.FireSurvivorSpecificUIEvent();
            }
            if (ConfigOptions.AllowHudColorEdits.Value)
            {
                MyHud?.StartCoroutine(HudColor.SetSurvivorColorFromTargetBody(cameraRigController.targetBody));
            }
        }
    }



    [MonoDetourTargets(typeof(BaseConVar))]
    private static class AttemptRepositionOnResolutionChange
    {
        [MonoDetourHookInitialize]
        private static void Setup()
        {
            Mdh.RoR2.ConVar.BaseConVar.AttemptSetString.Postfix(OnConvarStringChanged);
        }

        private static void OnConvarStringChanged(BaseConVar self, ref string newValue)
        {
            if (self.name == "resolution")
            {
                // try as we might, this doesn't really work
                HudStructure.RepositionAllHudElements();
            }
        }
    }
}