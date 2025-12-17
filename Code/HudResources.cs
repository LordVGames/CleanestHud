using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2.UI;
using UnityEngine.UI;
using static CleanestHud.Main;
using RoR2.ContentManagement;
namespace CleanestHud;


internal class HudResources
{
    internal static class HudAssets
    {
        private static readonly AssetReferenceT<Texture2D> _textWhiteReference = new(RoR2BepInExPack.GameAssetPaths.Version_1_35_0.RoR2_Base_Common.texWhite_png);
        internal static Sprite WhiteSprite;
        private static readonly AssetReferenceT<Material> _fontMaterialReference = new(RoR2BepInExPack.GameAssetPaths.Version_1_35_0.RoR2_Base_Common_Fonts_Bombardier.tmpBombDropshadow3D_mat);
        internal static Material FontMaterial;

        private static readonly AssetReferenceT<GameObject> _gameEndReportPanelAssetReference = new(RoR2BepInExPack.GameAssetPaths.Version_1_35_0.RoR2_Base_UI.GameEndReportPanel_prefab);
        private static readonly AssetReferenceT<GameObject> _scoreboardStripAssetReference = new(RoR2BepInExPack.GameAssetPaths.Version_1_35_0.RoR2_Base_UI.ScoreboardStrip_prefab);
        private static readonly AssetReferenceT<GameObject> _itemIconIngameAssetReference = new(RoR2BepInExPack.GameAssetPaths.Version_1_35_0.RoR2_Base_UI.ItemIconScoreboard_InGame_prefab);
        private static readonly AssetReferenceT<GameObject> _chatBoxAssetReference= new(RoR2BepInExPack.GameAssetPaths.Version_1_35_0.RoR2_Base_UI.ChatBox_prefab);
        private static readonly AssetReferenceT<GameObject> _statStripTemplateAssetReference = new(RoR2BepInExPack.GameAssetPaths.Version_1_35_0.RoR2_Base_UI.StatStripTemplate_prefab);

        internal static void SetupAssets()
        {
            AssignSomeAssets();
            EditOtherAssets();
        }

        private static void AssignSomeAssets()
        {
            Texture2D texWhite = null;
            AssetAsyncReferenceManager<Texture2D>.LoadAsset(_textWhiteReference).Completed += (handle) =>
            {
                texWhite = handle.Result;
            };
            WhiteSprite = Sprite.Create(texWhite, new Rect(0f, 0f, 4f, 4f), Vector2.zero);
            AssetAsyncReferenceManager<Material>.LoadAsset(_fontMaterialReference).Completed += (handle) =>
            {
                FontMaterial = handle.Result;
            };
        }

        private static void EditOtherAssets()
        {
            AssetAsyncReferenceManager<GameObject>.LoadAsset(_gameEndReportPanelAssetReference).Completed += (handle) =>
            {
                AssetEdits.RemoveGameEndReportPanelDetails(handle.Result);
                AssetAsyncReferenceManager<GameObject>.UnloadAsset(_gameEndReportPanelAssetReference);
            };
            AssetAsyncReferenceManager<GameObject>.LoadAsset(_scoreboardStripAssetReference).Completed += (handle) =>
            {
                AssetEdits.EditScoreboardStripAsset(handle.Result);
                AssetAsyncReferenceManager<GameObject>.UnloadAsset(_scoreboardStripAssetReference);
            };
            AssetAsyncReferenceManager<GameObject>.LoadAsset(_itemIconIngameAssetReference).Completed += (handle) =>
            {
                AssetEdits.EditItemIconIngame(handle.Result);
                AssetAsyncReferenceManager<GameObject>.UnloadAsset(_itemIconIngameAssetReference);
            };
            AssetAsyncReferenceManager<GameObject>.LoadAsset(_chatBoxAssetReference).Completed += (handle) =>
            {
                AssetEdits.RemoveChatBoxDetails(handle.Result);
                AssetAsyncReferenceManager<GameObject>.UnloadAsset(_chatBoxAssetReference);
            };
            AssetAsyncReferenceManager<GameObject>.LoadAsset(_statStripTemplateAssetReference).Completed += (handle) =>
            {
                // doesn't need it's own assetedits method yet
                handle.Result.DisableImageComponent();
                AssetAsyncReferenceManager<GameObject>.UnloadAsset(_statStripTemplateAssetReference);
            };
        }
    }



    internal static class ImportantHudTransforms
    {
        // in simulacrum the RunInfoHudPanel is InfiniteTowerUI(Clone)
        // in normal runs it's ClassicRunInfoHudPanel(Clone)
        internal static Transform UpperLeftCluster = null;
        internal static Transform UpperRightCluster  = null;
        internal static Transform RunInfoHudPanel = null;
        internal static Transform TimerPanel = null;
        internal static Transform RightInfoBar = null;
        internal static Transform BarRoots = null;
        internal static Transform HealthBarRoot = null;
        internal static Transform LevelDisplayCluster = null;
        internal static Transform ExpBarRoot = null;
        internal static Transform DifficultyBar = null;
        internal static Transform DifficultyBarContent = null;
        internal static Transform SetDifficultyPanel = null;
        internal static Transform BottomRightCluster = null;
        internal static Transform BottomCenterCluster = null;
        internal static Transform BuffDisplayRoot = null;
        internal static Transform SkillDisplayRoot = null;
        internal static Transform SprintCluster = null;
        internal static Transform InventoryCluster = null;
        internal static Transform ArtifactPanel = null;
        internal static Transform TopCenterCluster = null;
        internal static Transform BossHealthBarContainer = null;
        internal static Transform MapNameCluster = null;
        internal static Transform MapNameClusterSubtext = null;
        // yes, these 2 are different parts of the HUD
        internal static Transform SimulacrumDefaultWaveUI = null;
        internal static Transform InfiniteTowerDefaultWaveUI = null;
        internal static Transform SimulacrumWavePanel = null;
        internal static Transform NotificationArea = null;
        internal static Transform RightCluster = null;
        internal static Transform ContextNotification = null;
        internal static Transform InspectPanelArea
        {
            get
            {
                if (!IsHudFinishedLoading)
                {
                    return null;
                }

                Transform container = Helpers.GetContainerFromScoreboardPanel(MyHudLocator.FindChild("ScoreboardPanel"));
                Transform inspectPanel = container.GetChild(2);
                return inspectPanel.GetChild(0);
            }
        }
        internal static Transform ScoreboardPanel = null;
        internal static Transform ScoreboardPanelContainer = null;
        internal static Transform SuppressedItems = null;
        internal static Transform ScoreboardStripContainer = null;
        internal static Transform ScoreboardPanelHorizontalBox = null;
        internal static Transform CrosshairExtras = null;


        internal static void FindImportantHudTransforms()
        {
            UpperLeftCluster = MyHudLocator.FindChild("UpperLeftCluster");
            UpperRightCluster = MyHudLocator.FindChild("UpperRightCluster");
            RunInfoHudPanel = UpperRightCluster.GetChild(0);
            TimerPanel = RunInfoHudPanel.Find("TimerPanel");
            RightInfoBar = RunInfoHudPanel.Find("RightInfoBar");
            BarRoots = MyHudLocator.FindChild("BottomLeftCluster").Find("BarRoots");
            HealthBarRoot = BarRoots.GetChild(1);
            LevelDisplayCluster = BarRoots.GetChild(0);
            ExpBarRoot = LevelDisplayCluster.Find("ExpBarRoot");
            DifficultyBar = RunInfoHudPanel.Find("DifficultyBar");
            DifficultyBarContent = DifficultyBar.Find("Scroll View/Viewport/Content");
            SetDifficultyPanel = RunInfoHudPanel.Find("SetDifficultyPanel");
            BottomRightCluster = MyHudLocator.FindChild("BottomRightCluster");
            BottomCenterCluster = MyHudLocator.FindChild("BottomCenterCluster");
            BuffDisplayRoot = MyHudLocator.FindChild("BuffDisplayRoot");
            SkillDisplayRoot = MyHudLocator.FindChild("SkillDisplayRoot");
            SprintCluster = SkillDisplayRoot.Find("SprintCluster");
            InventoryCluster = SkillDisplayRoot.Find("InventoryCluster");
            ScoreboardPanel = MyHudLocator.FindChild("ScoreboardPanel");
            ScoreboardPanelContainer = Helpers.GetContainerFromScoreboardPanel(ScoreboardPanel);
            SuppressedItems = ScoreboardPanelContainer.GetChild(3);
            ScoreboardStripContainer = ScoreboardPanelContainer.GetChild(1);
            ScoreboardPanelHorizontalBox = ScoreboardPanelContainer.Find("InspectPanel/InspectPanelArea/InspectionPanel")?.GetChild(0);
            ArtifactPanel = RightInfoBar.Find("ArtifactPanel");
            TopCenterCluster = MyHudLocator.FindChild("TopCenterCluster");
            BossHealthBarContainer = MyHudLocator.FindChild("BossHealthBar").parent.parent;
            MapNameCluster = MyHud.mainContainer.transform.Find("MapNameCluster");
            MapNameClusterSubtext = MapNameCluster.Find("Subtext");
            SimulacrumDefaultWaveUI = RunInfoHudPanel.Find("DefaultWaveUI");
            InfiniteTowerDefaultWaveUI = RunInfoHudPanel.Find("DefaultWaveUI");
            SimulacrumWavePanel = RunInfoHudPanel.Find("WavePanel");
            NotificationArea = MyHudLocator.FindChild("NotificationArea");
            RightCluster = MyHudLocator.FindChild("RightCluster");
            ContextNotification = RightCluster.Find("ContextNotification");
            CrosshairExtras = MyHudLocator.FindChild("CrosshairExtras");
        }
    }
}