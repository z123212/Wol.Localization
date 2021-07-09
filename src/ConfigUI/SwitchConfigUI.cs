using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Wol.Localization.ConfigUI
{
    public class SwitchConfigUI
    {
        Rect BaseFrm;
        internal bool IsShow { get; set; } = false;

        public SwitchConfigUI()
        {


        }

        private void Init(int id)
        {
            GUILayout.BeginArea(BaseFrm);
            GUILayout.Label("配置项");
            //第一行
            GUILayout.BeginHorizontal();
            GUILayout.Label("开启本地化");
            var style = new GUIStyle();

            GUILayout.Toggle(ZyConfig.OpenLocaliz, ZyConfig.OpenLocaliz ? "开启" : "关闭", style);
            GUILayout.EndHorizontal();
            //第二行

            GUILayout.EndArea();
        }

        public void Show()
        {
            if (IsShow)
            {
                //Debug.Log("on show()");
                IsShow = true;
                BaseFrm = new Rect((Screen.width / 2) - 250, (Screen.height / 2) - 250, 500, 500);
                BaseFrm = GUI.Window(20210218, BaseFrm, Init, "我的一个窗口");
            }
            else
            {

            }


        }


    }
}
