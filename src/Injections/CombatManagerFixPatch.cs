using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Wol.Localization.Injections
{
    [HarmonyPatch(typeof(CombatManager))]
    class CombatManagerFixPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("SetState")]
        static void CombatActionButtonPrefix(CombatManager __instance, ref string ___doItButtonDefaultActionText, ref string ___totalApFormat, ref string ___noApFormat)
        {
            //var trav = Traverse.Create(__instance);
            //Debug.LogError($"CombatActionButtonPostfixPatch===CombatManager .ctor init::___totalApFormat={___totalApFormat}");
            ___doItButtonDefaultActionText = Localization.GetLocalizationStringAndWrite("DO IT");
            ___totalApFormat = Localization.GetLocalizationStringAndWrite("You have {0} Action Point{1}.");
            ___noApFormat = Localization.GetLocalizationStringAndWrite("You have no Action Points left.");
            //Debug.LogError("CombatActionButtonPostfixPatch===CombatManager .ctor init");
        }
    }
}
