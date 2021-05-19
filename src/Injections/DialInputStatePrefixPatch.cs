using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wol.Localization.Injections
{
    [HarmonyPatch(typeof(DialInputState))]
    class DialInputStatePrefixPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnSubmit", MethodType.Normal)]
        static bool OnSubmitPrefixPatch(DialInputState __instance,ref Dials ___m_dials, Action<string, int[]> ___m_onSubmit,string ___m_strBucket)
        {
            string[] values = null;
            int[] arg = null;
            var m_dials = ___m_dials;
            if (m_dials != null)
            {
                values = m_dials.currentValues;
                arg = m_dials.currentIndexes;
            }
            __instance.PopSelf();
            //Action<string, int[]> m_onSubmit
            //var m_onSubmit = trav.Field<Action<string, int[]>>("m_onSubmit").Value;
            if (___m_onSubmit != null)
            {
              var txt=  Localization.ResolveLocalizationDuplex(string.Concat(values), ___m_strBucket);
                ___m_onSubmit(txt, arg);
            }

            return false;
        }
    }
}
