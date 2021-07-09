using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beam.UI;
using HarmonyLib;
using UnityEngine;

namespace StrandedDeep.Localization.Injections
{
    [HarmonyPatch(typeof(GeneralMenuPresenter))]
    class GeneralMenuPresenterPatch
    {

        [HarmonyPostfix]
        [HarmonyPatch("GeneralMenuApplyButton_Click")]
        static void GeneralMenuApplyButton_ClickPatch()
        {
            Debug.LogWarning(Options.GeneralSettings.LangPath);
        }
    }
}
