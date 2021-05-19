using HarmonyLib;
using Steamworks;
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
    [HarmonyPatch(typeof(TitleScreenUI))]
    class TitleScreenUIFixPatch
    {
        static MethodInfo OnConfirmViewStoreSteamMi = AccessTools.Method(typeof(TitleScreenUI), "OnConfirmViewStoreSteam", new[] { typeof(bool) });

        [HarmonyPrefix]
        [HarmonyPatch("ShowDlcList", typeof(Vector3))]
        static bool ShowDlcListPrefix(TitleScreenUI __instance,ref Vector3 pos)
        {
            if (SteamUtils.IsOverlayEnabled())
            {
                SteamFriends.ActivateGameOverlayToStore(SteamManager.WOL_APPID, EOverlayToStoreFlag.k_EOverlayToStoreFlag_None);
                return false;
            }
            string localizationStringAndWrite = Localization.GetLocalizationStringAndWrite("Visit the Steam store");
            string localizationStringAndWrite2 = Localization.GetLocalizationStringAndWrite("Do you want to view available DLC in Steam?");
            string localizationStringAndWrite3 = Localization.GetLocalizationStringAndWrite("Yes");
            string localizationStringAndWrite4 = Localization.GetLocalizationStringAndWrite("No");
            WestOfLoathing.instance.state_machine.Push(new ConfirmState(localizationStringAndWrite, localizationStringAndWrite2, localizationStringAndWrite3, localizationStringAndWrite4,
                b => OnConfirmViewStoreSteamMi.Invoke(__instance, new object[] { b })
                //new Action<bool>(TitleScreenUI.OnConfirmViewStoreSteam)
                ));
            return false;
        }
    }
}
