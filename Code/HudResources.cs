﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2.UI;
using UnityEngine.UI;
using static CleanestHud.Main;

namespace CleanestHud
{
    internal class HudResources
    {
        internal static class HudAssets
        {
            internal static Sprite WhiteSprite;
            internal static Material FontMaterial;

            internal static GameObject SimulacrumWaveUI;
            internal static GameObject EnemyInfoPanel;
            internal static GameObject GameEndReportPanel;
            internal static GameObject ViendCrosshair;
            internal static GameObject ViendCorruption;
            internal static GameObject ScoreboardStrip;
            internal static GameObject ItemIconScoreboard;
            internal static GameObject MoonDetonationPanel;
            internal static GameObject StatStripTemplate;
            internal static GameObject ChatBox;
            internal static GameObject ItemIconPrefab;

            internal static void LoadHudAssets()
            {
                Texture2D texWhite = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/texWhite.png").WaitForCompletion();
                WhiteSprite = Sprite.Create(texWhite, new Rect(0f, 0f, 4f, 4f), Vector2.zero);
                FontMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/Fonts/Bombardier/tmpBombDropshadow3D.mat").WaitForCompletion();

                ChatBox = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/ChatBox.prefab").WaitForCompletion();
                SimulacrumWaveUI = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerDefaultWaveUI.prefab").WaitForCompletion();
                EnemyInfoPanel = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/EnemyInfoPanel.prefab").WaitForCompletion();
                GameEndReportPanel = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/GameEndReportPanel.prefab").WaitForCompletion();
                ViendCrosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorCrosshair.prefab").WaitForCompletion();
                ViendCorruption = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorCorruptionUISimplified.prefab").WaitForCompletion();
                ScoreboardStrip = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/ScoreboardStrip.prefab").WaitForCompletion();
                ItemIconScoreboard = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/ItemIconScoreboard.prefab").WaitForCompletion();
                MoonDetonationPanel = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/HudCountdownPanel.prefab").WaitForCompletion();
                StatStripTemplate = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StatStripTemplate.prefab").WaitForCompletion();
                ItemIconPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/ItemIconScoreboard_InGame.prefab").WaitForCompletion();
            }
        }

        internal static class ImportantHudTransforms
        {
            // in simulacrum the RunInfoHudPanel is InfiniteTowerUI(Clone)
            // in normal runs it's ClassicRunInfoHudPanel(Clone)
            internal static Transform RunInfoHudPanel = null;
            internal static Transform RightInfoBar = null;
            internal static Transform BarRoots = null;


            internal static Transform InspectPanelArea
            {
                get
                {
                    if (!IsHudFinishedLoading)
                    {
                        return null;
                    }

                    Transform container = HudChanges.Helpers.GetContainerFromScoreboardPanel(MyHudLocator.FindChild("ScoreboardPanel"));
                    Transform inspectPanel = container.GetChild(2);
                    return inspectPanel.GetChild(0);
                }
            }
            internal static void FindImportantHudTransforms()
            {
                RunInfoHudPanel = MyHudLocator.FindChild("UpperRightCluster").GetChild(0);
                RightInfoBar = RunInfoHudPanel.Find("RightInfoBar");
                BarRoots = MyHudLocator.FindChild("BottomLeftCluster").Find("BarRoots");
            }
        }
    }
}
