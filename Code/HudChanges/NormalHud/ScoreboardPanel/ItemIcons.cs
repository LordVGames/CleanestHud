using System;
using Mono.Cecil.Cil;
using MonoDetour;
using MonoDetour.Cil;
using MonoDetour.HookGen;
using MonoMod.Cil;
using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.NormalHud.ScoreboardPanel;

[MonoDetourTargets(typeof(ItemIcon))]
internal static class ItemIcons
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.UI.ItemIcon.SetItemIndex_RoR2_ItemIndex_System_Int32_System_Single.ILHook(SetItemIndex);
    }


    private static void SetItemIndex(ILManipulationInfo info)
    {
        ILWeaver w = new(info);



        // actually set glowImage to something (because gearbox never did!!!!!!!!!!!!)
        w.MatchRelaxed(
            x => x.MatchLdarg(0) && w.SetCurrentTo(x),
            x => x.MatchLdfld<ItemIcon>("glowImage"),
            x => x.MatchCall<UnityEngine.Object>("op_Implicit")
        ).ThrowIfFailure()
        .InsertBeforeCurrent(
            w.Create(OpCodes.Ldarg_0),
            w.CreateDelegateCall((ItemIcon itemIcon) =>
            {
                if (IsHudUserBlacklisted)
                {
                    return;
                }
                if (itemIcon.image.name != "ItemIconScoreboard_InGame(Clone)")
                {
                    return;
                }


                // doing this doesn't actually cause that much lag and only happens for whatever icons are added/updated
                // it also makes future mass glowimage coloring better on performance
                itemIcon.glowImage = itemIcon.transform.GetChild(1).GetComponent<RawImage>();
                HudColor.ColorSingleItemIconHighlight(itemIcon);
            })
        );



        ILLabel afterVanillaHighlightColoring = w.DefineLabel();
        // turns out there's normally unused code to set said glowimage's color to the item's rarity color with 0.75 alpha
        // that's cool but we're doing our own coloring methods so we're gonna Br over it
        w.MatchRelaxed(
            x => x.MatchLdcR4(0.75f),
            x => x.MatchNewobj<Color>(),
            x => x.MatchCallvirt<Graphic>("set_color") && w.SetCurrentTo(x)
        ).ThrowIfFailure()
        .MarkLabelToCurrentNext(afterVanillaHighlightColoring);

        w.MatchRelaxed(
            x => x.MatchLdarg(0) && w.SetCurrentTo(x),
            x => x.MatchLdfld<ItemIcon>("glowImage"),
            x => x.MatchCall<UnityEngine.Object>("op_Implicit")
        ).ThrowIfFailure()
        .InsertBeforeCurrentStealLabels(
            w.Create(OpCodes.Br, afterVanillaHighlightColoring)
        );
    }
}