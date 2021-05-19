using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wol.Localization.Injections
{
    [HarmonyPatch(typeof(LoadMenuItem))]
    class LoadMenuItemFixPatch
    {

        [HarmonyPrefix]
        [HarmonyPatch("OnDelete")]
        static bool OnDeletePrefix(LoadMenuItem __instance)
        {
            GameStateMachine state_machine = WestOfLoathing.instance.state_machine;
            if (!state_machine.HasState("confirm"))
            {
                SoundPlayer.instance.PlayUISound(UISound.DELETECHARBUTTON);
                string title = Localization.GetLocalizationStringAndWrite("Delete this saved game?");
                string strMessage = string.Format(Localization.GetLocalizationStringAndWrite("This character, {0}, will be GONE! You can't undo this!"), __instance.name);
                string confirmTxt = Localization.GetLocalizationStringAndWrite("Yes");
                string cancelTxt = Localization.GetLocalizationStringAndWrite("No");
                state_machine.Push(new ConfirmState(title, strMessage, confirmTxt, cancelTxt, b => OnConfirmFailedNewGameCreation(b, __instance)));
            }
            return false;
        }

        static MethodInfo OnConfirmDeleteMi = AccessTools.Method(typeof(LoadMenuItem), "OnConfirmDelete", new[] { typeof(bool) }, null);

        private static void OnConfirmFailedNewGameCreation(bool f, LoadMenuItem obj)
        {
            // its a stub so it has no initial content
            OnConfirmDeleteMi.Invoke(obj, new object[] { f });
        }
    }
}
