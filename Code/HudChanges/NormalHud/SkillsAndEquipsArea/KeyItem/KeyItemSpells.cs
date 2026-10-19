using MonoDetour;
using MonoDetour.HookGen;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static CleanestHud.HudResources;
namespace CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea.KeyItem;


[MonoDetourTargets(typeof(CursedBookExtraDisplay))]
internal static class KeyItemSpells
{
    private const int _defaultXPos = -30;
    private const int _defaultYPos = -36;
    private const int _spacingPerSpellPast1 = 20;
    private static Transform _spellsDisplay;


    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudDetails.OnHudDetailEditsBegun += HudDetails_OnHudDetailEditsBegun;
        Mdh.RoR2.UI.CursedBookExtraDisplay.RebuildIcons.Postfix(SpellsRebuild);
        Mdh.RoR2.UI.HUD.OnDestroy.Postfix(Destroy);
    }
    

    private static void HudDetails_OnHudDetailEditsBegun()
    {
        ImportantHudTransforms.KeyItemSlot.Find("DisplayRoot/FlashPanel, Reminder").DisableImageComponent();
    }


    private static void SpellsRebuild(CursedBookExtraDisplay self)
    {
        if (self.iconAllocator.elements.Count < 1)
        {
            return;
        }


        if (_spellsDisplay == null)
        {
            _spellsDisplay = ImportantHudTransforms.KeyItemSlot.Find("ExtraDisplay");
        }
        _spellsDisplay?.localPosition = new Vector3(_defaultXPos - (_spacingPerSpellPast1 * (self.iconAllocator.elements.Count - 1)), _defaultYPos);
        // why isn't the parent SpellSlotHolder? .parent jumps over it into CursedBookExtraDisplay(Clone)
        self.iconAllocator.elements[0].parent.transform.Find("SpellSlotHolder").DisableImageComponent();
    }


    private static void Destroy(HUD self)
    {
        _spellsDisplay = null;
    }
}