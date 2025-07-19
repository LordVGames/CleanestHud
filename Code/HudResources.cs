using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2.UI;
using UnityEngine.UI;
using static CleanestHud.Main;
using RoR2.ContentManagement;
using CleanestHud.HudChanges;

namespace CleanestHud
{
    internal class HudResources
    {
        internal static class HudAssets
        {
            private static readonly AssetReferenceT<Texture2D> _textWhiteReference = new(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Common.texWhite_png);
            internal static Sprite WhiteSprite;
            private static readonly AssetReferenceT<Material> _fontMaterialReference = new(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Common_Fonts_Bombardier.tmpBombDropshadow3D_mat);
            internal static Material FontMaterial;

            private static readonly AssetReferenceT<GameObject> _gameEndReportPanelAssetReference = new(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_UI.GameEndReportPanel_prefab);
            private static readonly AssetReferenceT<GameObject> _scoreboardStripAssetReference = new(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_UI.ScoreboardStrip_prefab);
            private static readonly AssetReferenceT<GameObject> _itemIconIngameAssetReference = new(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_UI.ItemIconScoreboard_InGame_prefab);
            private static readonly AssetReferenceT<GameObject> _moonDetonationPanelAssetReference = new(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_UI.HudCountdownPanel_prefab);
            private static readonly AssetReferenceT<GameObject> _chatBoxAssetReference= new(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_UI.ChatBox_prefab);
            private static readonly AssetReferenceT<GameObject> _statStripTemplateAssetReference = new(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_UI.StatStripTemplate_prefab);

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
                AssetAsyncReferenceManager<GameObject>.LoadAsset(_moonDetonationPanelAssetReference).Completed += (handle) =>
                {
                    AssetEdits.RemoveMoonDetonationPanelDetails(handle.Result);
                    AssetAsyncReferenceManager<GameObject>.UnloadAsset(_moonDetonationPanelAssetReference);
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
