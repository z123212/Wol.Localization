using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wol.Localization.Injections
{
    [HarmonyPatch(typeof(CombatActionButton))]
    class CombatActionButtonFixPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HandleCommandButtonDescription", new Type[] { typeof(MEvalContext), typeof(MCommand) })]
        static bool HandleCommandButtonDescriptionPrefixPatch(CombatActionButton __instance, MEvalContext ectx, MCommand cmd)
        {
            if (cmd.op == MCommand.Op.SAY)
            {
                var trav = Traverse.Create(__instance);
                if (!trav.Field("m_fDisallowedByScript").GetValue<bool>() && trav.Field("m_strbDescription").GetValue<StringBuilder>() != null)
                {
                    string text = cmd.StrArgSay(" ");
                    if (trav.Field("m_strbDescription").GetValue<StringBuilder>().Length > 0)
                    {
                        trav.Field("m_strbDescription").GetValue<StringBuilder>().Append('\n');
                    }
                    text = Localization.GetLocalizationStringAndWrite(text);
                    trav.Field("m_strbDescription").GetValue<StringBuilder>().Append(text);
                    return false;
                }
            }
            return true;
        }
    }
}
