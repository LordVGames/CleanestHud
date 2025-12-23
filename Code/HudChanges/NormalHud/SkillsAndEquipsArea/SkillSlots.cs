using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudChanges.HudColor;
using static CleanestHud.HudResources;
using RoR2.UI;
namespace CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;


internal static class SkillSlots
{
    public const float SkillSlotSpacing = 105;

    
    internal static void HudStructureEdits()
    {
        // not doing a normal "for" loop so i don't have "MyHud.skillIcons[i]" everywhere
        Vector3 newSkillPosition;
        int i = 0;
        Vector3 centerSkillIconLocalPosition = MyHud.skillIcons[2].transform.localPosition;
        foreach (SkillIcon skillIcon in MyHud.skillIcons)
        {
            newSkillPosition = skillIcon.transform.localPosition;
            switch (i)
            {
                case 0:
                    newSkillPosition.x = (centerSkillIconLocalPosition.x - (SkillSlotSpacing * 2));
                    break;
                case 1:
                    newSkillPosition.x = (centerSkillIconLocalPosition.x - SkillSlotSpacing);
                    break;
                case 2:
                    break;
                case 3:
                    newSkillPosition.x = centerSkillIconLocalPosition.x + SkillSlotSpacing;
                    break;
            }
            skillIcon.transform.localPosition = newSkillPosition;
            i++;


            skillIcon.cooldownRemapPanel = null;

            GameObject cooldownPanel = skillIcon.transform.Find("CooldownPanel").gameObject;
            cooldownPanel.SetActive(false);

            Transform cooldownText = skillIcon.transform.Find("CooldownText");

            RectTransform cooldownTextRect = cooldownText.GetComponent<RectTransform>();
            cooldownTextRect.localPosition = new Vector3(-18f, 19.5f, 0f);

            HGTextMeshProUGUI cooldownTextMesh = cooldownText.GetComponent<HGTextMeshProUGUI>();
            cooldownTextMesh.color = Color.white;

            Image iconPanel = skillIcon.iconImage;
            RectTransform iconPanelRect = iconPanel.GetComponent<RectTransform>();
            iconPanelRect.localScale = Vector3.one * 1.1f;

            RectTransform isReadyPanelRect = skillIcon.isReadyPanelObject.GetComponent<RectTransform>();
            isReadyPanelRect.localScale *= 1.1f;

            GameObject skillBackgroundPanel = skillIcon.transform.Find("SkillBackgroundPanel").gameObject;
            skillBackgroundPanel.SetActive(ConfigOptions.ShowSkillKeybinds.Value);

            Transform skillStockRoot = skillIcon.transform.Find($"Skill{i}StockRoot");
            Transform skillStockRootText = skillStockRoot.Find("StockText");
            HGTextMeshProUGUI skillStockRootTextMesh = skillStockRootText.GetComponent<HGTextMeshProUGUI>();
            skillStockRootTextMesh.color = Color.white;
        }
    }


    internal static void HudDetailEdits()
    {
        SetSkillOutlinesStatus();
    }


    internal static void HudColorEdits()
    {
        foreach (SkillIcon skillIcon in MyHud.skillIcons)
        {
            GameObject isReadyPanel = skillIcon.isReadyPanelObject;
            Image isReadyPanelImage = isReadyPanel.GetComponent<Image>();
            isReadyPanelImage.color = SurvivorColor;
        }
    }


    internal static void SetSkillOutlinesStatus()
    {
        foreach (var skillIcon in MyHud.skillIcons)
        {
            Transform isReadyPanel = skillIcon.transform.GetChild(0);
            Image iconOutline = isReadyPanel.GetComponent<Image>();
            iconOutline.enabled = ConfigOptions.ShowSkillAndEquipmentOutlines.Value;
        }
    }
}