using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StrandedDeep.Localization.Injections
{
    [HarmonyPatch]
    class TextMeshProPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextMeshPro), "LoadFontAsset")]
        public static void LoadFontAssetHook(object __instance)
        {
            FontPatch.TMP_Patch(ref __instance);
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TextMeshPro), "LoadFontAsset")]
        public static void LoadFontAssetHook3(object __instance)
        {
            if (__instance.GetType() == typeof(TextMeshPro))
            {
                var font = Traverse.Create(__instance).Field("m_fontAsset").GetValue<TMP_FontAsset>();
                if (font != null)
                {
                    Debug.LogError($"4444:font={font.name}");
                }
            }
        }

        // Token: 0x06000022 RID: 34 RVA: 0x00002381 File Offset: 0x00000581
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextMeshProUGUI), "LoadFontAsset")]
        public static void LoadFontAssetHook2(object __instance)
        {
            FontPatch.TMP_Patch(ref __instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TextMeshProUGUI), "LoadFontAsset")]
        public static void LoadFontAssetHook4(object __instance)
        {
            if (__instance.GetType() == typeof(TextMeshPro))
            {
                var font = Traverse.Create(__instance).Field("m_fontAsset").GetValue<TMP_FontAsset>();
                if (font != null)
                {
                    Debug.LogError($"555:font={font.name}");
                }
            }
        }
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(Text))]
        //[HarmonyPatch("text", MethodType.Getter)]
        //public static void UGUI_TextPropertyHook(ref string value, ref Text __instance)
        //{
        //    FontPatch.UIText_Patch(ref __instance);
        //}

    }
}
