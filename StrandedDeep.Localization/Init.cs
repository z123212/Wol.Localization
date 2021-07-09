using BepInEx;
//using BepInEx.Common;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeep.Localization
{
    /// <summary>
    /// 入口文件
    /// </summary>
    [BepInPlugin("StrandedDeep.Localization.plugins.LocalizationMod", "孤岛求生汉化插件mod", "1.0.0.0")]
    public class Init : BaseUnityPlugin
    {
        //313120
        // 在插件启动时会直接调用Awake()方法
        void Awake()
        {
            try
            {
                //Localization.Load();//初始化本地化数据
                //if (ZyConfig.OpenLocaliz)
                //{
                ChAssentManager.Load();

                Harmony harmony = new Harmony("StrandedDeep.Localization.plugins.LocalizationMod");
                harmony.PatchAll();
                //}

                //Harmony.CreateAndPatchAll(typeof(ModelManagerInjectFixPatch));
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex);
                base.Logger.LogError("Mod Pack failed to load.");
                return;
            }
            base.Logger.LogInfo("Mod Pack successfully loaded!");

        }

        // 在所有插件全部启动完成后会调用Start()方法，执行顺序在Awake()后面；
        void Start()
        {
            //Debug.Log("这里是Start()方法中的内容!");
        }

        BepInEx.Configuration.KeyboardShortcut key = new BepInEx.Configuration.KeyboardShortcut(KeyCode.F9);
        //BepInEx.Configuration.KeyboardShortcut ConfigUiKey = new BepInEx.Configuration.KeyboardShortcut(KeyCode.F10);
        // 插件启动后会一直循环执行Update()方法，可用于监听事件或判断键盘按键，执行顺序在Start()后面
        void Update()
        {

            //if (key.IsDown())
            //{
            //    Localization.Reload();
            //    Debug.Log("汉化词条重新加载完毕");
            //}
            //if (ConfigUiKey.IsDown())
            //{
            //    Debug.Log("F10 按下");
            //    //switchConfigUI.Show();
            //    switchConfigUI.IsShow = !switchConfigUI.IsShow;
            //}
        }
        public static string PlguinBasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); //Utility.ParentDirectory;//.PluginsDirectory;
        //public static string Path_Plugin = Path.Combine(PlguinBasePath, "StrandedDeep.Localization");


        internal static uint ComputeStringHash(string s)
        {
            uint num = 0;
            if (s != null)
            {
                num = 2166136261U;
                for (int i = 0; i < s.Length; i++)
                {
                    num = ((uint)s[i] ^ num) * 16777619U;
                }
            }
            return num;
        }
    }
}
