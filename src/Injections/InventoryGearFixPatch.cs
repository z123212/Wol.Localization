using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Wol.Localization.Injections
{
    [HarmonyPatch(typeof(InventoryGear))]
    class InventoryGearFixPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("slot", MethodType.Setter)]
        static bool slotPrefixPatch(InventoryGear __instance, string value,ref Text ___slotName,ref GameObject ___emptyName)
        {
            ___slotName.text = value;
            //__instance.slotName.text = value;
            Text componentInChildren = ___emptyName.GetComponentInChildren<Text>();
            if (componentInChildren != null)
            {
                componentInChildren.text = Localization.GetLocalizationStringAndWrite2("no " + value, Localization.LoadLocalType.ToUpper);
            }
            return false;
        }
    }
}
