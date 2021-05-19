using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Wol.Localization.Injections
{
    [HarmonyPatch(typeof(Tooltip))]
    class TooltipFixPatch
    {
        //SetTipAt
        //string str, Rect rectLimits
        [HarmonyTranspiler]
        [HarmonyPatch("SetTipAt", new[] { typeof(string), typeof(Rect) })]
        static IEnumerable<CodeInstruction> SetTipAtTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            /*
ldarg.1
call	string Tooltip::StrResolveTooltip(string)
stloc.2     
            
ldarg.1
call	string Localization::RepliceLinkString(string)
call	string Tooltip::StrResolveTooltip(string)
stloc.2
             */
            var codes = new CodeMatcher(instructions)
                .Start()
                //string text = Tooltip.StrResolveTooltip(str);
                .MatchForward(false,
                    new CodeMatch(OpCodes.Ldarg_1),
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Tooltip), "StrResolveTooltip", new[] { typeof(string) })),
                    new CodeMatch(OpCodes.Stloc_2)
                );
            if (codes.IsValid)
            {
                //string text = Tooltip.StrResolveTooltip(Localization.RepliceLinkString(str));
                codes.Advance(1)
                    .Insert(
                    new CodeInstruction(OpCodes.Call, CommonExtensions.RepliceLinkStringType)
                    );
            }




            return codes.InstructionEnumeration();

        }

    }
}
