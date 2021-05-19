using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Wol.Localization.Injections
{
    [HarmonyPatch(typeof(MItem))]
    class MItemFixPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("StrFullDescriptionNoName", MethodType.Normal)]
        static bool OnSubmitPrefixPatch(MItem __instance, ref string __result)
        {
            //var trav = Traverse.Create(__instance);

            //__result = Localization.GetItemTextWithPlayerItem(__instance);
            var desc = Traverse.Create(__instance).Method("StrFullDescriptionInternal", MItem.DescFlags.None).GetValue<string>();//(MItem.DescFlags.None);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(desc);
            MItem mitem = MPlayer.instance.ItemGetGear(__instance.slot);
            if (mitem != null && __instance.invid != mitem.invid)
            {
                stringBuilder.Append("\n--------------------\n<b><color=blue>");
                stringBuilder.Append(Localization.GetLocalizationString("[EQUIPPED STATS]"));
                stringBuilder.AppendLine("</color></b>\n");
                string value = mitem.StrFullDescriptionStoreNoSellValue();
                stringBuilder.Append(value);
            }
            __result = stringBuilder.ToString();

            return false;
        }

        static MethodInfo LswInfo = AccessTools.Method(typeof(Localization), nameof(Localization.GetLocalizationStringAndWrite), new[] { typeof(string) });

        [HarmonyTranspiler]
        [HarmonyPatch("StrFullDescriptionInternal", typeof(MItem.DescFlags))]
        static IEnumerable<CodeInstruction> StrFullDescriptionInternalTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            //ldstr	"Damage: "
            /*
ldstr	"Damage"
call	string Localization::GetLocalizationStringAndWrite(string)
ldstr	": "
call	string [mscorlib]System.String::Concat(string, string)             
             */
            var codes = new CodeMatcher(instructions)
                .Start()
                .MatchForward(true, new CodeMatch(OpCodes.Ldstr, "Damage: "))
                .Set(OpCodes.Nop,null)
                .Insert(
                    new CodeInstruction(OpCodes.Ldstr, "Damage"),
                    new CodeInstruction(OpCodes.Call, LswInfo),
                    new CodeInstruction(OpCodes.Ldstr, ": "),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), new[] { typeof(string), typeof(string) }))
                )
                .MatchForward(false, new CodeMatch(OpCodes.Ldstr, "Sell Value: ")) //查找 ldstr	"Sell Value: "
                .Set(OpCodes.Nop, null)
                .Insert(
                /*
                 ldstr	"{0}："
ldstr	"Sell Value"
call	string Localization::GetLocalizationStringAndWrite(string)
call	string [mscorlib]System.String::Format(string, object)
                 */
                new CodeInstruction(OpCodes.Ldstr, "{0}："),
                new CodeInstruction(OpCodes.Ldstr, "Sell Value"),
                new CodeInstruction(OpCodes.Call, LswInfo),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), "Format", new[] { typeof(string), typeof(object) }))
                )
                //replace ldstr	" Meat"
                .MatchForward(false, new CodeMatch(OpCodes.Ldstr, " Meat"))
                .Set(OpCodes.Nop, null)
                .Insert(

                new CodeInstruction(OpCodes.Ldstr, " {0}"),
                new CodeInstruction(OpCodes.Ldstr, "Meat"),
                new CodeInstruction(OpCodes.Call, LswInfo),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), "Format", new[] { typeof(string), typeof(object) }))
                )
                .InstructionEnumeration();

            //codes.LogInstructions();

            return codes;
        }
    }
}
