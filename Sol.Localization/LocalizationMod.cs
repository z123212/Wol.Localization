using BepInEx;
using HarmonyLib;
using Sol.Localization.ConfigUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Sol.Localization
{
    [BepInPlugin("Sol.Localization.plugins.LocalizationMod", "SOL汉化插件mod", "1.0.0.0")]
    public class LocalizationMod : BaseUnityPlugin
    {

        // 在插件启动时会直接调用Awake()方法
        void Awake()
        {
            try
            {
                Localization.Load();//初始化本地化数据
                if (ZyConfig.OpenLocaliz)
                {
                    Harmony harmony = new Harmony("Sol.Localization.plugins.LocalizationMod");
                    harmony.PatchAll();
                }

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
        BepInEx.Configuration.KeyboardShortcut ConfigUiKey = new BepInEx.Configuration.KeyboardShortcut(KeyCode.F10);
        // 插件启动后会一直循环执行Update()方法，可用于监听事件或判断键盘按键，执行顺序在Start()后面
        void Update()
        {

            if (key.IsDown())
            {
                Localization.Reload();
                Debug.Log("汉化词条重新加载完毕");
            }
            if (ConfigUiKey.IsDown())
            {
                Debug.Log("F10 按下");
                //switchConfigUI.Show();
                switchConfigUI.IsShow = !switchConfigUI.IsShow;
            }
        }
        // 在插件关闭时会调用OnDestroy()方法
        void OnDestroy()
        {
            Debug.Log("当你看到这条消息时，就表示我已经被关闭一次了!");
        }

        void InitFileds()
        {


        }
        private SwitchConfigUI switchConfigUI = new SwitchConfigUI();
        void OnGUI()
        {
            switchConfigUI.Show();
        }
    }
}
