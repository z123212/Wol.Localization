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
    [HarmonyPatch]
    public class MLanguageFixPatch
    {
        static MethodInfo mb = AccessTools.Method(typeof(MCommand), "MadlibString", new Type[] { typeof(string) });
        static MethodInfo _call = AccessTools.Method(typeof(Localization), "MadlibStringFix", new Type[] { typeof(string), typeof(MCommand), typeof(int) });
        [HarmonyPatch(typeof(MLanguage))]
        [HarmonyPatch("TranslateString")]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            

            IEnumerable<CodeInstruction> inst = new CodeMatcher(instructions)
                .Start()
                .MatchForward(false, new CodeMatch(OpCodes.Call, mb))
                .Insert(new CodeInstruction(OpCodes.Ldnull), new CodeInstruction(OpCodes.Ldc_I4_0))
                .Advance(2)
                .Set(OpCodes.Call, _call)
                .InstructionEnumeration();

            //inst.LogInstructions();
            return inst;
            //return toReturn;

        }
    }
}
