using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Wol.Localization.Injections
{
    /// <summary>
    /// 主要用于游戏首页的汉化，修改原来的GameObject所挂载的资源，替换汉化图片或者替换成文字的方式达成
    /// </summary>
    [HarmonyPatch(typeof(TitleScreenUI))]
    class TitleStateWaaPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("UpdateGamepadMenuButtonList")]
        static void UpdateGamepadMenuButtonListPostfix(TitleStateWaa __instance, ref List<Transform> ___m_aryXfmMenuButton)
        {
            if (___m_aryXfmMenuButton?.Count > 0)
            {
                foreach (var item in ___m_aryXfmMenuButton)
                {
                    Debug.Log($"item.name:{item.name}");
                    switch (item.name)
                    {
                        case "continue_sign":
                            {
                                item.gameObject.GetComponentInChildren<SpriteRenderer>().ReplaceContinue();
                            }
                            break;

                        case "load_sign":
                            {
                                item.gameObject.GetComponentInChildren<SpriteRenderer>().ReplaceLoad();
                            }
                            break;

                        case "new_game_sign":
                            {
                                item.gameObject.GetComponentInChildren<SpriteRenderer>().ReplaceNewGame();
                            }
                            break;

                        case "options_sign":
                            {
                                item.gameObject.GetComponentInChildren<SpriteRenderer>().ReplaceOptions();
                            }
                            break;

                        case "quit_sign":
                            item.gameObject.GetComponentInChildren<SpriteRenderer>().ReplaceQuite();
                            break;

                        default:
                            break;
                    }

                }
            }
        }

    }
}
