using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wol.Localization.Injections
{
    [HarmonyPatch(typeof(CreateCharacterStateWaa))]
    class CreateCharacterStateWaaFixPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnFinished", new Type[] { typeof(bool) })]
        static bool OnFinishedPrefixPatch(CreateCharacterStateWaa __instance, bool fAccepted)
        {
            var trav = Traverse.Create(__instance);
            if (trav.Field("m_fDone").GetValue<bool>() || __instance.statemachineCur == null || !__instance.statemachineCur.IsState("create_character_waa"))
            {
                return true;
            }
            if (fAccepted)
            {
                string strTitle = string.Format(Localization.GetLocalizationStringAndWrite("Play as {0} ?"), trav.Field<CharacterCreatorShootingGallery>("m_ccsg").Value.fullName);
                string localizationStringAndWrite = Localization.GetLocalizationStringAndWrite("We're ready to get going! Are you?");
                string localizationStringAndWrite2 = Localization.GetLocalizationStringAndWrite("Yeah!");
                string localizationStringAndWrite3 = Localization.GetLocalizationStringAndWrite("No...");
                __instance.statemachineCur.Push(new DelayState(
                    new ConfirmState(
                        strTitle,
                        localizationStringAndWrite,
                        localizationStringAndWrite2,
                        localizationStringAndWrite3,
                        b => OnConfirmStartGameMi.Invoke(__instance, new object[] { b })
                    ),
                    0.5f, false));

            }
            trav.Field("m_fDone").SetValue(true);
            //__instance.m_fDone = true;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("OnConfirmStartGame", new Type[] { typeof(bool) })]
        static bool OnConfirmStartGamePrefixPatch(CreateCharacterStateWaa __instance, bool f)
        {

            if (f)
            {
                var trav = Traverse.Create(__instance);
                var ccsg = trav.Field<CharacterCreatorShootingGallery>("m_ccsg").Value;
                string text = SavedGame.NewGame(ccsg.firstName, ccsg.lastName, ccsg.fullName, ccsg.isCowgirl, ccsg.meatReward, ccsg.animal);
                if (string.IsNullOrEmpty(text))
                {
                    string localizationStringAndWrite = Localization.GetLocalizationStringAndWrite("Something went wrong!");
                    string localizationStringAndWrite2 = Localization.GetLocalizationStringAndWrite("We tried to write save file data to your computer, but were not able to!  We can’t let you keep playing, because you’d never be able to save your progress.\n\nIf you'd like help with this problem, please visit our discussions on Steam.");
                    string localizationStringAndWrite3 = Localization.GetLocalizationStringAndWrite("Let me try again");
                    string localizationStringAndWrite4 = Localization.GetLocalizationStringAndWrite("Quit the game");
                    WestOfLoathing.instance.state_machine.Push(new ConfirmState(localizationStringAndWrite, localizationStringAndWrite2, localizationStringAndWrite3, localizationStringAndWrite4, b => OnConfirmFailedNewGameCreation(b, __instance)));

                }
                WestOfLoathing.instance.state_machine.Push(new DelayState(new StartGameplayState(text), 0.5f, true));
                trav.Field("m_fDone").SetValue(true);
                //this.m_fDone = true;
            }
            return false;
        }

        static MethodInfo OnConfirmStartGameMi = AccessTools.Method(typeof(CreateCharacterStateWaa), "OnConfirmStartGame", new[] { typeof(bool) }, null);
        static MethodInfo OnConfirmFailedNewGameCreationMi = AccessTools.Method(typeof(CreateCharacterStateWaa), "OnConfirmFailedNewGameCreation", new[] { typeof(bool) }, null);

        //[HarmonyReversePatch]
        //[HarmonyPatch("OnConfirmStartGame", typeof(bool))]
        //private static void OnConfirmStartGame(bool f)
        //{
        //    // its a stub so it has no initial content
        //    throw new NotImplementedException("It's a stub");
        //}
        //[HarmonyReversePatch]
        //[HarmonyPatch("OnConfirmFailedNewGameCreation", MethodType.Normal)]
        private static void OnConfirmFailedNewGameCreation(bool f, CreateCharacterStateWaa obj)
        {
            // its a stub so it has no initial content
            OnConfirmFailedNewGameCreationMi.Invoke(obj, new object[] { f });
        }
    }
}
