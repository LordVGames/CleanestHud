using System;
using RoR2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using MiscFixes.Modules;

namespace CleanestHud
{
    internal class AssetEdits
    {
        internal static void EditScoreboardStripAsset(GameObject scoreboardStripAsset)
        {
            scoreboardStripAsset.GetComponent<RawImage>().texture = ModAssets.AssetBundle.LoadAsset<Texture2D>("NewNavHighlight");

            Transform longBackground = scoreboardStripAsset.transform.GetChild(0);
            longBackground.GetComponent<Image>().sprite = HudAssets.WhiteSprite;
            longBackground.Find("ClassBackground").DisableImageComponent();
            // DO NOT remove the items background image here
            // if you do it will break the automatic scaling of the hud at ultrawide resolutions
            // the ScoreboardStripEditor component makes the background image invisible instead
            // same effect with better functionality
            longBackground.Find("EquipmentBackground").DisableImageComponent();
            longBackground.Find("TotalTextContainer/MoneyText").GetComponent<HGTextMeshProUGUI>().color = Color.white;
            // GetChild(2) is NameLabel
            longBackground.GetChild(2).GetComponent<HGTextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            // im pretty sure its fine to disable the backgroud image here since it doesn't scale with resolution
            longBackground.Find("NameLabel/NameFocusHighlight").DisableImageComponent();
            // i would reposition parts of the asset here but then it never works ingame lmao
        }

        // this doesn't actually happen ingame, its just the asset name
        internal static void EditItemIconIngame(GameObject itemIconIngameAsset)
        {
            Transform navFocusHighlightTransform = itemIconIngameAsset.transform.GetChild(1);

            RawImage navFocusHighlightRawImage = navFocusHighlightTransform.GetComponent<RawImage>();
            navFocusHighlightRawImage.texture = ModAssets.AssetBundle.LoadAsset<Texture2D>("NewNavHighlight");

            HGButton button = itemIconIngameAsset.GetComponent<HGButton>();
            // setting the button colors to white makes it not influence glowimage color with yellow (the default)
            // we can always just manually color it back to yellow later anyways
            button.m_Colors.highlightedColor = Helpers.ChangeColorWhileKeepingAlpha(button.m_Colors.highlightedColor, Color.white);
            button.m_Colors.normalColor = Helpers.ChangeColorWhileKeepingAlpha(button.m_Colors.normalColor, Color.white);
            button.m_Colors.pressedColor = Helpers.ChangeColorWhileKeepingAlpha(button.m_Colors.pressedColor, Color.white);
            button.m_Colors.selectedColor = Helpers.ChangeColorWhileKeepingAlpha(button.m_Colors.selectedColor, Color.white);
            // better higlight visibility for some character colors
            button.m_Colors.m_ColorMultiplier = 1.5f;
        }

        internal static void RemoveGameEndReportPanelDetails(GameObject gameEndReportPanelAsset)
        {
            Transform safeAreaJUICEDLMAO = gameEndReportPanelAsset.transform.Find("SafeArea (JUICED)");
            Transform bodyArea = safeAreaJUICEDLMAO.Find("BodyArea");



            Transform headerArea = safeAreaJUICEDLMAO.Find("HeaderArea");

            HGTextMeshProUGUI deathFlavorTextMesh = headerArea.Find("DeathFlavorText").GetComponent<HGTextMeshProUGUI>();
            deathFlavorTextMesh.fontStyle = FontStyles.Normal;
            deathFlavorTextMesh.fontSizeMax = 30f;

            HGTextMeshProUGUI resultLabelMesh = headerArea.Find("ResultArea/ResultLabel").GetComponent<HGTextMeshProUGUI>();
            resultLabelMesh.material = HudAssets.FontMaterial;



            Transform statsAndChatArea = bodyArea.Find("StatsAndChatArea");
            Transform chatArea = statsAndChatArea.Find("ChatArea");
            Transform chatBox = chatArea.GetChild(0);
            Transform standardRect = chatBox.GetChild(2);
            Transform standardRectScrollView = standardRect.GetChild(0);
            Transform statsContainer = statsAndChatArea.Find("StatsContainer");
            Transform statsBody = statsContainer.Find("Stats Body");
            Transform statsBodyScrollview = statsBody.Find("ScrollView");
            Transform statsBodyScrollviewViewportContent = statsBodyScrollview.Find("Viewport/Content");
            Transform scrollbarVertical = statsBodyScrollview.Find("Scrollbar Vertical");

            chatArea.DisableImageComponent();
            // GetChild(0) is to get to "PermanentBg"
            chatBox.GetChild(0).DisableImageComponent();
            // GetChild(1) is to get to "StandardRectScrollViewBackground"
            standardRectScrollView.GetChild(1).gameObject.SetActive(false);
            // GetChild(2) is to get to "StandardRectScrollViewBorderImage"
            standardRectScrollView.GetChild(2).gameObject.SetActive(false);
            statsContainer.Find("BorderImage").gameObject.SetActive(false);
            statsContainer.Find("Stats And Player Nav/Stats Header").DisableImageComponent();
            statsBody.DisableImageComponent();
            statsBodyScrollviewViewportContent.Find("SelectedDifficultyStrip").DisableImageComponent();
            statsBodyScrollviewViewportContent.Find("EnabledArtifactsStrip").DisableImageComponent();
            scrollbarVertical.DisableImageComponent();
            scrollbarVertical.Find("Sliding Area/Handle").DisableImageComponent();

            RectTransform chatAreaRect = chatArea.GetComponent<RectTransform>();
            chatAreaRect.sizeDelta = new Vector2(817f, 200f);
            chatAreaRect.localPosition = new Vector3(441.5f, -311.666666666f, 0f);



            Transform infoArea = bodyArea.Find("RightArea/InfoArea");
            Transform infoBody = infoArea.Find("Info Body");
            Transform itemArea = infoBody.Find("ItemArea");
            Transform itemAreaScrollView = itemArea.Find("ScrollView");
            Transform unlockArea = infoBody.Find("UnlockArea");
            Transform unlockAreaScrollView = unlockArea.Find("ScrollView");

            infoArea.Find("BorderImage").DisableImageComponent();
            infoArea.Find("Info Header").DisableImageComponent();
            itemArea.Find("Item Header").DisableImageComponent();
            itemAreaScrollView.DisableImageComponent();
            itemAreaScrollView.Find("Scrollbar Vertical").DisableImageComponent();
            unlockArea.Find("Unlocked Header").DisableImageComponent();
            unlockAreaScrollView.DisableImageComponent();
            unlockAreaScrollView.Find("Scrollbar Vertical").DisableImageComponent();
        }

        internal static void RemoveMoonDetonationPanelDetails(GameObject moonDetonationPanelAsset)
        {
            RectTransform hudCountdownPanelRect = moonDetonationPanelAsset.GetComponent<RectTransform>();
            // panel's x position is pushed -576.4219 ingame
            hudCountdownPanelRect.localPosition = new Vector3(576.4219f, -200f, 0f);



            Transform container = moonDetonationPanelAsset.transform.Find("Juice").Find("Container");

            container.Find("Backdrop").DisableImageComponent();
            container.Find("Border").DisableImageComponent();

            Transform countdownTitleLabel = container.Find("CountdownTitleLabel");
            HGTextMeshProUGUI countdownTitleLabelMesh = countdownTitleLabel.GetComponent<HGTextMeshProUGUI>();
            countdownTitleLabelMesh.fontSharedMaterial = HudAssets.FontMaterial;
            countdownTitleLabelMesh.color = Color.red;

            Transform countdownLabel = container.Find("CountdownLabel");
            HGTextMeshProUGUI countdownLabelMesh = countdownLabel.GetComponent<HGTextMeshProUGUI>();
            countdownLabelMesh.fontSharedMaterial = HudAssets.FontMaterial;
            countdownLabelMesh.color = Color.red;
        }

        internal static void RemoveChatBoxDetails(GameObject chatBoxAsset)
        {
            chatBoxAsset.DisableImageComponent();



            Transform permanentBG = chatBoxAsset.transform.Find("PermanentBG");

            Image permanentBGImage = permanentBG.GetComponent<Image>();
            permanentBGImage.sprite = HudAssets.WhiteSprite;
            permanentBGImage.color = new Color32(0, 0, 0, 212);

            Transform inputField = permanentBG.Find("Input Field");
            Image inputFieldImage = inputField.GetComponent<Image>();
            inputFieldImage.sprite = HudAssets.WhiteSprite;
            inputFieldImage.color = new Color32(0, 0, 0, 100);

            RectTransform permanentBGRect = permanentBG.GetComponent<RectTransform>();
            permanentBGRect.localPosition = new Vector3(0f, 1f, 0f);
            permanentBGRect.sizeDelta = new Vector2(0f, 48f);



            Transform chatBoxScrollView = chatBoxAsset.transform.Find("StandardRect/Scroll View");

            chatBoxScrollView.Find("Background").DisableImageComponent();
            chatBoxScrollView.Find("BorderImage").DisableImageComponent();

            Transform chatBoxScrollbarVertical = chatBoxScrollView.Find("Scrollbar Vertical");
            Scrollbar chatBoxScrollbarVerticalScrollbar = chatBoxScrollbarVertical.GetComponent<Scrollbar>();
            chatBoxScrollbarVerticalScrollbar.enabled = true;
        }
    }
}