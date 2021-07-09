using Beam.Language;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeep.Localization.Injections
{
    [HarmonyPatch(typeof(LocalizationHandler))]
    internal class LocalizationHandlerPatch
    {
        internal static bool LoadCh = false;
        static HashSet<Language> _languages = new HashSet<Language>
        {
            new Language("English (Official)", "en-DEFAULT", "en-DEFAULT"),
            new Language("French (Official)", "fr-DEFAULT", "fr-DEFAULT"),
            new Language("Italian (Official)", "it-DEFAULT", "it-DEFAULT"),
            new Language("German (Official)", "de-DEFAULT", "de-DEFAULT"),
            new Language("Spanish (Official)", "es-DEFAULT", "es-DEFAULT"),
            new Language("Russian (Official)", "ru-DEFAULT", "ru-DEFAULT"),
            new Language("Chinese (Official)", "ch-DEFAULT", "ch-DEFAULT")
        };

        [HarmonyPrefix, HarmonyPatch("GetLanguages")]
        static bool GetLanguages(ref IEnumerable<Language> __result)
        {
            Debug.Log("我是GetLanguages");
            __result = _languages;
            return false;


        }

        [HarmonyPrefix, HarmonyPatch("IsDefaultAssetPath", typeof(string))]
        static bool IsDefaultAssetPathPatch(ref string path, ref bool __result)
        {
            __result = (path == "en-DEFAULT" || path == "fr-DEFAULT" || path == "it-DEFAULT" || path == "de-DEFAULT" || path == "es-DEFAULT" || path == "ru-DEFAULT" || path == "ch-DEFAULT");
            return false;
        }

        [HarmonyPrefix, HarmonyPatch("LoadExternalAsset", typeof(string))]
        static bool LoadExternalAssetPatch(LocalizationHandler __instance, string path, ref string[] __result)
        {
            Debug.Log("我是LoadExternalAsset，path=" + path);
            Log.Write($"我是LoadExternalAsset，path={path}", true);
            if (path == "ch-DEFAULT")
            {
                __result = LocalizationCh.Ch;
                return false;
            }
            return true;
        }

        [HarmonyPrefix, HarmonyPatch("LoadAsset", typeof(string))]
        static bool LoadAssetPatch(LocalizationHandler __instance, string path, ref string[] __result)
        {
            Debug.Log("LoadAssetPatch，path=" + path);
            Log.Write($"我是LoadExternalAsset，path={path}", true);
            LoadCh = false;
            if (path == "ch-DEFAULT")
            {
                LoadCh = true;

                __result = LocalizationCh.Ch;
                return false;
            }
            return true;
        }
        //[HarmonyPrefix, HarmonyPatch("LoadAssetEntries", typeof(string))]
        //static bool LoadAssetEntriesPatch(LocalizationHandler __instance, string path, ref string[] __result)
        //{
        //    Debug.Log("我是LoadExternalAsset，path=" + path);
        //    if (path == "ch-DEFAULT")
        //    {
        //        __result = LocalizationCh.Ch;
        //        return false;
        //    }
        //    return true;
        //}
    }
}
