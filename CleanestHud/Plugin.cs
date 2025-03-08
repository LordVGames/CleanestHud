using System;
using BepInEx;
using RoR2;
using R2API.Utils;

namespace CleanestHud
{
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(LookingGlass.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(SS2.SS2Main.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static PluginInfo PluginInfo { get; private set; }
        public const string PluginAuthor = "LordVGames";
        public const string PluginName = "CleanestHud";
        public const string PluginVersion = "0.10.0";
        public const string PluginGUID = PluginAuthor + "." + PluginName;

        public void Awake()
        {
            PluginInfo = Info;
            Log.Init(Logger);
            ModAssets.Init();
            ConfigOptions.BindConfigOptions(Config);
            // prefab changes need to happen very early because when the hud is fully initialized the prefab may have already been spawned and then it can't be edited that way
            HudResources.HudAssets.LoadHudAssets();
            //HudChanges.HudStructure.AssetEdits.EditHudElementPrefabs();
            HudChanges.HudDetails.AssetEdits.EditHudElementPrefabDetails();



            On.RoR2.UI.HUD.Awake += Main.OnHooks.HUD_Awake;
            On.RoR2.UI.HUD.OnDestroy += Main.OnHooks.HUD_OnDestroy;
            On.RoR2.CameraModes.CameraModeBase.OnTargetChanged += Main.OnHooks.CameraModeBase_OnTargetChanged;

            On.RoR2.UI.AllyCardController.Awake += Main.OnHooks.AllyCardController_Awake;
            On.RoR2.UI.AllyCardController.UpdateInfo += Main.OnHooks.AllyCardController_UpdateInfo;
            On.RoR2.UI.HealthBar.InitializeHealthBar += Main.OnHooks.HealthBar_InitializeHealthBar;
            On.RoR2.UI.InfiniteTowerWaveProgressBar.OnEnable += Main.OnHooks.InfiniteTowerWaveProgressBar_OnEnable;
            On.RoR2.UI.NotificationUIController.SetUpNotification += Main.OnHooks.NotificationUIController_SetUpNotification;
            On.RoR2.UI.ScoreboardController.Rebuild += Main.OnHooks.ScoreboardController_Rebuild;
            On.RoR2.UI.ScoreboardController.SelectFirstScoreboardStrip += Main.OnHooks.ScoreboardController_SelectFirstScoreboardStrip;
            On.RoR2.UI.DifficultyBarController.OnCurrentSegmentIndexChanged += Main.OnHooks.DifficultyBarController_OnCurrentSegmentIndexChanged;
            On.RoR2.VoidSurvivorController.OnOverlayInstanceAdded += Main.OnHooks.VoidSurvivorController_OnOverlayInstanceAdded;
            On.EntityStates.Seeker.MeditationUI.SetupInputUIIcons += Main.OnHooks.MeditationUI_SetupInputUIIcons;

            IL.RoR2.UI.BuffDisplay.UpdateLayout += Main.ILHooks.BuffDisplay_UpdateLayout;
            IL.RoR2.UI.ItemIcon.SetItemIndex += Main.ILHooks.ItemIcon_SetItemIndex;
            //IL.RoR2.UI.ScoreboardStrip.UpdateItemCountText += Main.ILHooks.ScoreboardStrip_UpdateItemCountText;

            InfiniteTowerRun.onWaveInitialized += Main.Events.InfiniteTowerRun_onWaveInitialized;

            if (ModSupport.Starstorm2.ModIsRunning)
            {
                CharacterBody.onBodyInventoryChangedGlobal += ModSupport.Starstorm2.CharacterBody_onBodyInventoryChangedGlobal;
            }
        }
    }
}