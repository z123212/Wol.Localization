using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace Wol.Localization.Injections
{
    [HarmonyPatch(typeof(NpcStore))]
    class NpcStoreFixPatch
    {
        //NpcStore.ReloadLocal() : void @0600094A
        [HarmonyTranspiler]
        [HarmonyPatch("HandleBuySell")]
        static IEnumerable<CodeInstruction> HandleBuySellTranspiler(IEnumerable<CodeInstruction> instructions)
        {

            var codes = new CodeMatcher(instructions)
                .Start()
                .MatchForward(true, new CodeMatch(OpCodes.Ldstr, "Not enough Meat"))
                .Advance(1)
                .WhenValid(c =>
                {
                    return c.Advance(1)
                      .Insert(new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType));
                })
                .Start()
                .MatchForward(true, new CodeMatch(OpCodes.Ldstr, "You don't have enough Meat to buy that."))
                .WhenValid(c =>
                {
                    return c.Advance(1)
                      .Insert(new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType));
                })
                .Start()
                .MatchForward(true, new CodeMatch(OpCodes.Ldstr, "Dang"))
                .WhenValid(c =>
                {
                    return c.Advance(1)
                      .Insert(new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType));
                })
                .Start()
                .MatchForward(true, new CodeMatch(OpCodes.Ldstr, "Really sell this?"))
                .WhenValid(c =>
                {
                    return c.Advance(1)
                      .Insert(new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType));
                })
                .Start()
                .MatchForward(true, new CodeMatch(OpCodes.Ldstr, "This item is currently equipped. Are you sure you want to sell it?"))
                .WhenValid(c =>
                {
                    return c.Advance(1)
                      .Insert(new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType));
                })
                .Start()
                .MatchForward(true, new CodeMatch(OpCodes.Ldstr, "Yes"))
                .WhenValid(c =>
                {
                    return c.Advance(1)
                      .Insert(new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType));
                })
                .Start()
                .MatchForward(true, new CodeMatch(OpCodes.Ldstr, "No"))
                .WhenValid(c =>
                {
                    return c.Advance(1)
                      .Insert(new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType));
                })

                .InstructionEnumeration();
            //codes.LogInstructions();
            return codes;
        }

        [HarmonyPrefix]
        [HarmonyPatch("UpdateSelectedItemCountLabel", new[] { typeof(NpcStoreItem) })]
        static bool UpdateSelectedItemCountLabelPrefix(NpcStore __instance, NpcStoreItem nsi)
        {
            if (__instance.countLabel == null)
            {
                return false;
            }
            if (nsi == null || nsi.item == null || !nsi.isStoreItem)
            {
                __instance.countLabel.gameObject.SetActive(false);
                return false;
            }
            int num = MPlayer.instance.CItem(nsi.item.id);
            if (num < 1)
            {
                __instance.countLabel.gameObject.SetActive(false);
                return false;
            }
            __instance.countLabel.gameObject.SetActive(true);

            if (num == 1)
            {
                __instance.countLabel.text = string.Format(Localization.GetLocalizationStringAndWrite(__instance.countLabelFormatSingle), num);
                return false;
            }
            __instance.countLabel.text = string.Format(Localization.GetLocalizationStringAndWrite(__instance.countLabelFormatMultiple), num);
            return false;
        }

        [HarmonyTranspiler]
        [HarmonyPatch("HandleSelectItem", new[] { typeof(NpcStoreItem) })]
        static IEnumerable<CodeInstruction> HandleSelectItemTranspiler(IEnumerable<CodeInstruction> instructions)
        {

            var codes = new CodeMatcher(instructions)
                .Start()
                .MatchForward(true, new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(NpcStore), nameof(NpcStore.buyText))))
                .WhenValid(c =>
                {
                    return c.Advance(5)
                    .Insert(new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType));
                })
                .Start()
                .MatchForward(true, new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(NpcStore), nameof(NpcStore.buyTitle))))
                .WhenValid(c =>
                {
                    return c.Advance(5)
                    .Insert(new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType));
                })


                .InstructionEnumeration();

            codes.LogInstructions();

            return codes;
        }

        //        [HarmonyTranspiler]
        //        [HarmonyPatch(MethodType.Constructor)]
        //        static IEnumerable<CodeInstruction> NpcStoreTranspiler(IEnumerable<CodeInstruction> instructions)
        //        {
        //            var codes = new CodeMatcher(instructions)
        //                .Start()
        //                .MatchForward(true, new CodeMatch(OpCodes.Ldstr, "Click an item in your inventory or the store."))
        //                .WhenValid(c =>
        //                {
        //                    return c.Advance(1)
        //                      .Insert(new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType));
        //                })
        //                .Start()
        //                .MatchForward(true, new CodeMatch(OpCodes.Ldstr, "SELL FOR"))
        //                .WhenValid(c =>
        //                {
        //                    return c.Advance(1)
        //                      .Insert(new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType));
        //                })
        //                .Start()
        //                .MatchForward(true, new CodeMatch(OpCodes.Ldstr, "BUY FOR"))
        //                .WhenValid(c =>
        //                {
        //                    return c.Advance(1)
        //                      .Insert(new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType));
        //                })
        //                .Start()
        //                .MatchForward(true, new CodeMatch(OpCodes.Ldstr, "SELL"))
        //                .WhenValid(c =>
        //                {
        //                    return c.Advance(1)
        //                      .Insert(new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType));
        //                })
        //                .Start()
        //                .MatchForward(true, new CodeMatch(OpCodes.Ldstr, "BUY"))
        //                .WhenValid(c =>
        //                {
        //                    return c.Advance(1)
        //                      .Insert(new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType));
        //                })
        //                .Start()
        //                .MatchForward(true, new CodeMatch(OpCodes.Ldstr, "You have {0} of these."))
        //                .WhenValid(c =>
        //                {
        //                    return c.Advance(1)
        //                      .Insert(new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType));
        //                })
        //                .Start()
        //                .MatchForward(true, new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(NpcStore), nameof(NpcStore.countLabelFormatMultiple))))
        //                .WhenValid(c =>
        //                {
        //                    /*
        //24	0058	ldarg.0
        //25	0059	ldfld	string NpcStore::countLabelFormatSingle
        //26	005E	call	void [UnityEngine.CoreModule]UnityEngine.Debug::LogError(object)

        //                     */
        //                    //return c.Advance(1)
        //                    return c.Insert(new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType))
        //                    .Advance(1)
        //                    .Insert(
        //                        new CodeInstruction(OpCodes.Ldarg_0, null),
        //                        new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(NpcStore), nameof(NpcStore.countLabelFormatSingle))),
        //                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Debug), nameof(Debug.LogError), new[] { typeof(object) }))
        //                    )
        //                    ;
        //                })

        //                .InstructionEnumeration();
        //            codes.LogInstructions();
        //            return codes;
        //        }
    }
}
