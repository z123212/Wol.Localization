using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace StrandedDeep.Localization
{
    internal class ChAssentManager
    {


        public static void Load()
        {
            AssetBundle assetBundle = null;
            string text = Path.Combine(Init.PlguinBasePath, "SimChinese.unity3d");
            if (!File.Exists(text))
            {
                Debug.LogError("[错误] 文件不存在 " + text);
            }
            try
            {
                
                assetBundle = AssetBundle.LoadFromFile(text);
                //Font_FZLTZH = assetBundle.LoadAsset<Font>("FZLTZH");
                //Font_JDTH = assetBundle.LoadAsset<Font>("JDTH");
                //Font_Lolita = assetBundle.LoadAsset<Font>("Lolita");
                Font_SunExtA = assetBundle.LoadAsset<Font>("Sun-ExtA");
                //Font_YaHei = assetBundle.LoadAsset<Font>("YaHei");
                //Font_SHS = assetBundle.LoadAsset<Font>("SourceHanSansSC-Medium");
                //TMPFA_JKG = assetBundle.LoadAsset<TMP_FontAsset>("jkg SDF");
                TMPFA_SHSJP = assetBundle.LoadAsset<TMP_FontAsset>("sun SDF");
                if (TMPFA_SHSJP==null)
                {
                    Debug.LogError("Sun-ExtA SDF 加载失败啊");
                }
                //Font_FZLTZH.name = "_TH_" + Config.Font_FZLTZH.name;
                //Font_JDTH.name = "_TH_" + Config.Font_JDTH.name;
                //Font_Lolita.name = "_TH_" + Config.Font_Lolita.name;
                Font_SunExtA.name = "_TH_" + Font_SunExtA.name;
                //Font_YaHei.name = "_TH_" + Config.Font_YaHei.name;
                //Font_SHS.name = "_TH_" + Config.Font_SHS.name;
                Debug.Log($"没意外应该加载完毕了字体，Font_SunExtA.name ={Font_SunExtA.name }");
            }
            catch (Exception ex)
            {
                Debug.LogError("PluginConfig - Load");
                Debug.LogError("Message: " + ex.Message);
                Debug.LogError("StackTrace: " + ex.StackTrace);
            }
            finally
            {
                if (assetBundle != null)
                {
                    assetBundle.Unload(false);
                }
            }
        }


        public static Font Font_SunExtA;


        //public static TMP_FontAsset TMPFA_JKG;

        public static TMP_FontAsset TMPFA_SHSJP;
    }
}
