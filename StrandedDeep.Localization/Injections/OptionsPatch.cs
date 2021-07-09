using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeep.Localization.Injections
{
    [HarmonyPatch(typeof(Options))]
    class OptionsPatch
    {

        [HarmonyPrefix, HarmonyPatch("Load")]
        static void LoadPatch()
        {
            if (!File.Exists(FilePath.OPTIONS_FILE))
            {
                Debug.LogWarning("Options::Load:: Could not load options data. No file was found. \nOptions settings have been reverted to defaults.");
                return;
            }
            Debug.Log(FilePath.OPTIONS_FILE);
        }
    }
}
