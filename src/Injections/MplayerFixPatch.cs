using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace Wol.Localization.Injections
{
    [HarmonyPatch(typeof(MPlayer))]
    class MplayerFixPatch
    {

        [HarmonyTranspiler]
        [HarmonyPatch("DeserializeInternal")]
        static IEnumerable<CodeInstruction> DeserializeInternalTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            /*
 find:
 ldarg.0
 ldloc.s	V_14 (14)
 ldloc.s	V_15 (15)
 ldfld	string JSONObject::str
 call	string MData::UnescapeQuote(string)
 callvirt	instance void MData::SetString(string, string)


 replace:
 ldloc.s	V_15 (15)
 ldfld	string JSONObject::str
 call	string MData::UnescapeQuote(string)
 ldc.i4.3
 call	string Localization::GetLocalizationStringAndWrite2(string, valuetype Localization/LoadLocalType)
 stloc.s	V_16 (16)
 ldarg.0
 ldloc.s	V_14 (14)
 ldloc.s	V_16 (16)
             */
            var unescapeQuoteMi = AccessTools.Method(typeof(MData), "UnescapeQuote", new[] { typeof(string) });
            var setStringMi = AccessTools.Method(typeof(MData), "SetString", new[] { typeof(string), typeof(string) });

            var lswMi = AccessTools.Method(typeof(Localization), nameof(Localization.GetLocalizationStringAndWrite2), new[] { typeof(string), typeof(Localization.LoadLocalType) });

            var strLocalVer = il.DeclareLocal(typeof(string));
            //FileLog.Log("\n================================================\n");
            var inst = new CodeMatcher(instructions)
                .Start()
                .MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(c =>
                {
                    var lb = c.operand as LocalBuilder;
                    if (lb != null)
                    {
                        return c.opcode == OpCodes.Ldloc_S && lb.LocalIndex == 14 && lb.LocalType == typeof(string);
                    }
                    return false;
                }),
                new CodeMatch(c =>
                {
                    var lb = c.operand as LocalBuilder;
                    if (lb != null)
                    {
                        return c.opcode == OpCodes.Ldloc_S && lb.LocalIndex == 15 && lb.LocalType == typeof(JSONObject);
                    }
                    return false;
                }),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(JSONObject), nameof(JSONObject.str))),
                new CodeMatch(OpCodes.Call, unescapeQuoteMi)
                //new CodeMatch(OpCodes.Call, setStringMi)
                )
                .Insert(
                new CodeInstruction(OpCodes.Ldloc_S, 15),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(JSONObject), nameof(JSONObject.str))),
                new CodeInstruction(OpCodes.Call, unescapeQuoteMi),
                new CodeInstruction(OpCodes.Ldc_I4_3),
                new CodeInstruction(OpCodes.Call, lswMi),
                new CodeInstruction(OpCodes.Stloc_S, strLocalVer),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_S, 14),
                new CodeInstruction(OpCodes.Ldloc_S, strLocalVer)
                )

                .InstructionEnumeration();
            //;
            //FileLog.Log("\n================================================\n");
            //Debug.LogError($"pos={inst.Pos};len={inst.Length}");
            //return instructions;

            //inst.LogInstructions();
            //instructions.LogInstructions();
            return inst;
            //return toReturn;

        }
    }
}
