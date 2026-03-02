using CleanestHud.HudChanges;
using CleanestHud.HudChanges.NormalHud.HealthBarArea;
using CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;
using LeeHyperrealMod;
using LeeHyperrealMod.Content.Controllers;
using Mono.Cecil.Cil;
using MonoDetour;
using MonoDetour.Cil;
using MonoDetour.DetourTypes;
using MonoDetour.HookGen;
using MonoMod.Cil;
using RoR2;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
namespace CleanestHud.ModSupport;


internal static class LeeMod
{
    private static bool? _modexists;
    internal static bool ModIsRunning
    {
        get
        {
            _modexists ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LeeHyperrealPlugin.MODUID);
            return (bool)_modexists;
        }
    }
    private static bool IsLeeHudEditable
    {
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        get
        {
            return ConfigOptions.AllowSurvivorSpecificEdits.Value
            && HudCameraTargetBody != null
            && HudCameraTargetBody.bodyIndex == LeeHyperrealMod.Modules.Survivors.LeeHyperreal.staticBodyPrefab.GetComponent<CharacterBody>().bodyIndex;
        }
    }
    private const float _centerHudHeightBoostAmount = 100f;
    private const float _powerBarX = 1287f;
    private const float _powerBarY = 115f;
    private const float _ultBaseX = 94.5f;
    private const float _ultBaseY = -250f;
    private const float _orbsX = 7f;
    private const float _orbsY = 100f;
    private const float _orbsScaleY = 0.8f;




    [MonoDetourTargets(typeof(LeeHyperrealUIController), GenerateControlFlowVariants = true)]
    private static class Hooks
    {
        [MonoDetourHookInitialize]
        private static void Setup()
        {
            if (!ModIsRunning)
            {
                return;
            }


            Mdh.LeeHyperrealMod.Content.Controllers.LeeHyperrealUIController.InitializeUI.Postfix(AfterLeeHudMade);
            Mdh.LeeHyperrealMod.Content.Controllers.LeeHyperrealUIController.InitializeHoldOKTag.ILHook(OKTagAlwaysLookInCenter);
            Mdh.LeeHyperrealMod.Content.Controllers.LeeHyperrealUIController.InitializeSkillIconMaterial.ILHook(FixLocations);


            CleanestHud.HudChanges.SurvivorSpecific.SurvivorSpecific.OnSurvivorSpecificHudEditsFinished += OnSurvivorSpecificHudEditsFinished;
        }


        private static void AfterLeeHudMade(LeeHyperrealUIController self)
        {
            if (!self.isInitialized)
            {
                return;
            }


            Transform orbs = ImportantHudTransforms.BottomCenterCluster.Find("Orbs(Clone)");
            orbs?.localPosition = new Vector3(_orbsX, _orbsY, orbs.localPosition.z);
            orbs?.localScale = new Vector3(orbs.localScale.x, _orbsScaleY, orbs.localScale.z);

            Transform powerBar = ImportantHudTransforms.BottomLeftCluster.Find("Power Bar Empty(Clone)");
            powerBar?.localPosition = new Vector3(_powerBarX, _powerBarY, powerBar.localPosition.z);

            Transform ultBase = ImportantHudTransforms.Scaler.Find("Ult Base(Clone)");
            ultBase?.localPosition = new Vector3(_ultBaseX, _ultBaseY, ultBase.localPosition.z);
        }


        private static void OKTagAlwaysLookInCenter(ILManipulationInfo info)
        {
            ILWeaver w = new(info);
            Instruction startOfSkip = null!;
            Instruction endOfSkip = null!;


            w.MatchRelaxed(
                x => x.MatchLdsfld<LeeHyperrealPlugin>("isBetterHudInstalled") && w.SetInstructionTo(ref startOfSkip, x),
                x => x.MatchStloc(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchBrfalse(out _) && w.SetInstructionTo(ref endOfSkip, x) && w.SetCurrentTo(x)
            ).ThrowIfFailure()
            .InsertBranchOver(startOfSkip, endOfSkip);
        }


        private static void FixLocations(ILManipulationInfo info)
        {
            ILWeaver w = new(info);
            for (int i = 1; i < 5; i++)
            {
                int currentNum = i; // dumbass c#
                w.MatchRelaxed(
                    x => x.MatchLdstr($"BottomRightCluster/Scaler/Skill{currentNum}Root/IconPanel") && w.SetCurrentTo(x)
                ).ThrowIfFailure()
                .InsertAfterCurrent(
                    w.CreateDelegateCall((string oldString) =>
                    {
                        if (IsLeeHudEditable)
                        {
                            return $"BottomCenterCluster/Scaler/Skill{currentNum}Root/IconPanel";
                        }
                        return oldString;
                    })
                );
            }
        }
    }




    internal static void OnSurvivorSpecificHudEditsFinished()
    {
        if (!IsHudEditable || !ModIsRunning)
        {
            return;
        }


        if (IsLeeHudEditable)
        {
            SkillsScaler.SkillScalerLocalPosition = new Vector2(SkillsScaler.SkillScalerDefaultX, SkillsScaler.SkillScalerDefaultY + _centerHudHeightBoostAmount);
            BarRoots.BarRootsLocalPosition = new Vector2(BarRoots.BarRootsDefaultX, BarRoots.BarRootsDefaultY + _centerHudHeightBoostAmount);
        }
    }
}