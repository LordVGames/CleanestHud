using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using RoR2.UI;
using CleanestHud.ModSupport;
using static CleanestHud.Main;
using static CleanestHud.HudChanges.HudColor;
namespace CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;


internal static class EquipmentSlots
{
    internal static void HudStructureEdits()
    {
        for (int i = 0; i < MyHud.equipmentIcons.Length; i++)
        {
            Vector3 equipmentDisplayRootRectLocalPosition = Vector3.zero;
            float equipmentSlotScaleFactor = 1;
            switch (i)
            {
                case 0:
                    equipmentDisplayRootRectLocalPosition = new Vector3(-20.75f, 16.95f, 0f);
                    equipmentSlotScaleFactor = 0.928f;
                    break;
                case 1:
                    equipmentDisplayRootRectLocalPosition = new Vector3(-20, -5f, 0f);
                    // smaller by default, we don't have to scale any more/less unless we want it to be even smaller
                    equipmentSlotScaleFactor = 1f;
                    break;
            }



            EquipmentIcon equipment = MyHud.equipmentIcons[i];
            Transform equipmentDisplayRoot = equipment.displayRoot.transform;

            RectTransform equipmentDisplayRootRect = equipmentDisplayRoot.GetComponent<RectTransform>();
            equipmentDisplayRootRect.localPosition = equipmentDisplayRootRectLocalPosition;

            Transform equipmentCooldownText = equipmentDisplayRoot.Find("CooldownText");
            RectTransform equipmentCooldownTextRect = equipmentCooldownText.GetComponent<RectTransform>();
            equipmentCooldownTextRect.localPosition = new Vector3(0f, 1f, 0f);

            ScaleEquipmentSlot(equipmentDisplayRoot, equipmentSlotScaleFactor);

            if (HUDdleUPMod.ModIsRunning)
            {
                HUDdleUPMod.EditHuddleUpEquipmentCooldownOverlay();
            }
        }

        RectTransform firstSkillIconTextBackgroundPanelRect = MyHud.skillIcons[0].transform.Find("SkillBackgroundPanel").GetComponent<RectTransform>();
        GameObject equipmentTextBackgroundPanel = MyHud.equipmentIcons[0].displayRoot.transform.Find("EquipmentTextBackgroundPanel").gameObject;
        RectTransform equipmentTextBackgroundPanelRect = equipmentTextBackgroundPanel.GetComponent<RectTransform>();
        equipmentTextBackgroundPanelRect.position = new Vector3(
            equipmentTextBackgroundPanelRect.position.x,
            firstSkillIconTextBackgroundPanelRect.position.y,
            equipmentTextBackgroundPanelRect.position.z
        );
        equipmentTextBackgroundPanel.SetActive(ConfigOptions.ShowSkillKeybinds.Value);
    }
    // A tiny bit of scaling still happens even when the scaleFactor is just 1
    internal static void ScaleEquipmentSlot(Transform equipmentDisplayRoot, float scaleFactor)
    {
        float extraFactor = 0.0448f;
        RectTransform equipmentDisplayRootRect = equipmentDisplayRoot.GetComponent<RectTransform>();
        Transform equipmentBGPanel = equipmentDisplayRoot.Find("BGPanel");
        RectTransform equipmentBGPanelRect = equipmentBGPanel.GetComponent<RectTransform>();
        Transform equipmentIconPanel = equipmentDisplayRoot.Find("IconPanel");
        RectTransform equipmentIconPanelRect = equipmentIconPanel.GetComponent<RectTransform>();
        Transform equipmentIsReadyPanel = equipmentDisplayRoot.Find("IsReadyPanel");

        equipmentDisplayRootRect.localScale = (new Vector3(1f, 0.98f, 1f) * scaleFactor);
        equipmentBGPanelRect.localScale = new Vector3(scaleFactor + extraFactor, scaleFactor + extraFactor, scaleFactor);
        // why is the bg panel moved slightly? now we gotta move it back
        equipmentBGPanel.localPosition = Vector3.zero;
        equipmentIconPanelRect.localScale *= scaleFactor;
        equipmentIsReadyPanel.localScale *= scaleFactor;
    }


    internal static void HudColorEdits()
    {
        foreach (EquipmentIcon equipmentIcon in MyHud.equipmentIcons)
        {
            Image equipmentIsReadyPanelImage = equipmentIcon.isReadyPanelObject.GetComponent<Image>();
            equipmentIsReadyPanelImage.color = SurvivorColor;

            Image equipmentBGPanelImage = equipmentIcon.displayRoot.transform.Find("BGPanel").GetComponent<Image>();
            equipmentBGPanelImage.color = Helpers.GetAdjustedColor(SurvivorColor, colorIntensityMultiplier: DefaultHudColorIntensity);
        }
    }
}