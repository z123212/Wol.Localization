using HarmonyLib;
using StrandedDeep.Localization.Injections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StrandedDeep.Localization
{
    internal class FontPatch
    {
        // Token: 0x06000033 RID: 51 RVA: 0x000028A8 File Offset: 0x00000AA8
        public static void TMP_Patch(ref object __instance)
        {
            if (__instance == null)
            {
                return;
            }
            if (LocalizationHandlerPatch.LoadCh)
            {

                Log.Write("TMP_Patch:调用链", true);
                Material value;
                if (__instance.GetType() == typeof(TextMeshPro))
                {
                    value = Traverse.Create(__instance).Field("m_renderer").Field("sharedMaterial").GetValue<Material>();
                }
                else
                {
                    value = Traverse.Create(__instance).Field("m_sharedMaterial").GetValue<Material>();
                }
                //TMP_Text tmp_Text = (TMP_Text)__instance;
                //TMP_FontAsset tmp_FontAsset = null;

                TMP_FontAsset tmp_FontAsset = ChAssentManager.TMPFA_SHSJP;

                if (tmp_FontAsset != null)
                {
                    value.SetTexture(ShaderUtilities.ID_MainTex, tmp_FontAsset.material.GetTexture(ShaderUtilities.ID_MainTex));
                    Traverse.Create(__instance).Field("m_fontAsset").SetValue(tmp_FontAsset);
                    Debug.LogError("1222222:" + tmp_FontAsset.name);
                    return;
                }
                else {
                    Debug.LogError("66666666666666:");
                }

            }

        }

        // Token: 0x06000034 RID: 52 RVA: 0x000029D8 File Offset: 0x00000BD8
        public static void UIText_Patch(ref Text __instance)
        {
            Debug.LogError("333333");
            if (LocalizationHandlerPatch.LoadCh)
            {
                __instance.font = ChAssentManager.Font_SunExtA;
            }
        }
    }
}
