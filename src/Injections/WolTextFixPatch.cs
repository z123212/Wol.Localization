using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Wol.Localization.Injections
{
    [HarmonyPatch]
    class WolTextFixPatch
    {
        //Awake

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(UIBehaviour), "Awake")]
        //[MethodImpl(MethodImplOptions.NoInlining)]
        static void BaseAwakeReverse() { }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WolText), "Awake")]
        static bool AwakePrefix(WolText __instance, ref int ___m_sizeBase)
        {

            WolTextFixPatch.BaseAwakeReverse();
            var trav = Traverse.Create(__instance);
            //var bText = trav.Property("text").GetValue<string>();
            if (!string.IsNullOrEmpty(__instance.text))
            {

                __instance.text = Localization.ReplaceWolText(__instance.text);
                //trav.Property("text").SetValue(Localization.ReplaceWolText(bText));
                //base.text = Localization.ReplaceWolText(base.text);
                //用于做测试，随后需要删除掉
                Log.WriteGameLog($"当前文本：{__instance.text}\nTracking:{Log.GetStackTraceModelName()}\n===================\n");
                //Localization.Log.Write(string.Format("文本：原始：{0}；转码后：{1}\r\n调用路径：\r\n{2}\r\n\r\n", this.originTxt, base.text, Localization.Log.GetStackTraceModelName()));
            }
            ___m_sizeBase = trav.Property<int>("fontSize").Value;//base.fontSize;
            FontSwitcher.AddText(__instance);

            return false;

        }
        //[HarmonyTranspiler]
        //[HarmonyPatch("OnPopulateMesh", typeof(VertexHelper))]
        //static IEnumerable<CodeInstruction> OnPopulateMeshTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        //{

        //    var codes = new CodeMatcher(instructions)
        //        .Start()
        //        .MatchForward(false,
        //        new CodeMatch(OpCodes.Ldstr, "<color=") // 02A1
        //        );
        //    if (codes.IsValid)
        //    {
        //        codes.Set(OpCodes.Ldstr, "<&无效补丁$");          //02B5   276

        //    }

        //    codes.MatchForward(false, new CodeMatch(OpCodes.Ldstr, "</color>"));
        //    if (codes.IsValid)
        //    {
        //        codes.Set(OpCodes.Ldstr, "</&无效补丁$>");          //02B5   276

        //    }


        //    return codes.InstructionEnumeration();

        //    // return codes;

        //}

        static MethodInfo FIsNextMi = AccessTools.Method(typeof(WolText), "FIsNext", new Type[] { typeof(char[]), typeof(int), typeof(string) });

        static bool CallFIsNext(char[] aCh, int iCh, string str)
        {
            return (bool)FIsNextMi.Invoke(null, BindingFlags.NonPublic, null, new object[] { aCh, iCh, str }, null);
        }

        static Color32 color = new Color32(64, 128, 192, 255); //测试使用的颜色
        static Color32 color2 = new Color32(15, 50, 192, 255); //测试使用的颜色
        /// <summary>
        /// 修复由于汉化补丁引起中文显示不全的问题
        /// </summary>
        /// <param name="__instance">WolText的实例相当于this</param>
        /// <param name="toFill">原方法的参数</param>
        /// <param name="___m_DisableFontTextureRebuiltCallback">私有字段</param>
        /// <param name="___m_Boldness">私有字段</param>
        /// <param name="___m_TempVerts">私有字段</param>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WolText), "OnPopulateMesh", typeof(VertexHelper))]
        static bool OnPopulateMeshPrefix(WolText __instance, VertexHelper toFill, ref bool ___m_DisableFontTextureRebuiltCallback, ref float ___m_Boldness, ref UIVertex[] ___m_TempVerts)
        {
            //return false;
            var trav = Traverse.Create(__instance);
            if (__instance.font != null)
            {
                ___m_DisableFontTextureRebuiltCallback = true;
                //trav.Field("m_DisableFontTextureRebuiltCallback").SetValue(true);
                Vector2 size = __instance.rectTransform.rect.size;
                TextGenerationSettings textGenerationSettings = __instance.TgsGetAndAdjustString(size, __instance.text, out var str);
                var flag = false;
                FontStyle fontStyle = textGenerationSettings.fontStyle;
                if (fontStyle != FontStyle.Bold)
                {
                    if (fontStyle == FontStyle.BoldAndItalic)
                    {
                        flag = true;
                        textGenerationSettings.fontStyle = FontStyle.Italic;
                    }
                }
                else
                {
                    flag = true;
                    textGenerationSettings.fontStyle = FontStyle.Normal;
                }
                __instance.cachedTextGenerator.PopulateWithErrors(str, textGenerationSettings, __instance.gameObject);
                Rect rect = __instance.rectTransform.rect;
                Vector2 textAnchorPivot = Text.GetTextAnchorPivot(__instance.alignment);
                Vector2 zero = Vector2.zero;
                zero.x = ((textAnchorPivot.x == 1f) ? rect.xMax : rect.xMin);
                zero.y = ((textAnchorPivot.y == 0f) ? rect.yMin : rect.yMax);
                Vector2 vector = __instance.PixelAdjustPoint(zero) - zero;
                IList<UIVertex> verts = __instance.cachedTextGenerator.verts;


                float d = 1f / __instance.pixelsPerUnit;
                int count = verts.Count;
                var tmpText = __instance.text;
                char[] array = tmpText.ToCharArray();
                //Debug.LogError("=================================================");
                Debug.LogError(str);
                ////用于做测试，随后需要删除掉
                //Log.WriteGameLog($"当前文本：{__instance.text}\nTracking:{Log.GetStackTraceModelName()}\n===================\n");

                float num = ___m_Boldness * (float)__instance.fontSize;
                int num2 = (int)num;
                float num3 = num - (float)num2;

                toFill.Clear();
                int num4 = 0;
                int num5 = 0;
                int num6 = 0;//所提取字符数
                int i = 0;
                bool needFix = true;//是否需要补齐显示区域

                while (i < array.Length)
                {
                    #region 字符判断
                    #region 注释掉的
                    //char c = array[i];
                    //nFlag = false;
                    //if (c == '\n' || c == '\t' || c == ' ')
                    //{
                    //    //先留再这里吧
                    //    goto IL_325;
                    //}
                    //else if (c != '<')
                    //{
                    //    nFlag = true;
                    //    goto IL_325;
                    //}

                    //if (CallFIsNext(array, i, "<b>"))
                    //{
                    //    num4++;
                    //    i += 2;
                    //    nFlag = false;
                    //}
                    //else if (CallFIsNext(array, i, "</b>"))
                    //{
                    //    num4--;
                    //    i += 3;
                    //    nFlag = false;
                    //}
                    //else if (CallFIsNext(array, i, "<size="))
                    //{
                    //    nFlag = false;
                    //    for (i += 6; i < array.Length; i++)
                    //    {
                    //        if (array[i] == '>')
                    //        {
                    //            break;
                    //        }
                    //    }
                    //}
                    //else if (CallFIsNext(array, i, "</size>"))
                    //{
                    //    nFlag = false;
                    //    i += 6;
                    //}
                    ////加入下面部分代码会导致汉化文本显示不全，但是不加又会造成popup锚点不对
                    //else if (CallFIsNext(array, i, "<color="))
                    //{
                    //    nFlag = false;
                    //    for (i += 7; i < array.Length; i++)
                    //    {
                    //        if (array[i] == '>')
                    //        {
                    //            break;
                    //        }
                    //    }
                    //    goto IL_325;
                    //}
                    //else if (CallFIsNext(array, i, "</color>"))
                    //{
                    //    nFlag = false;
                    //    i += 7;
                    //    goto IL_325;
                    //}
                    //else if (CallFIsNext(array, i, "<i>"))
                    //{
                    //    i += 2;
                    //    nFlag = false;
                    //    goto IL_325;
                    //}
                    //else if (CallFIsNext(array, i, "</i>"))
                    //{
                    //    i += 3;
                    //    nFlag = false;
                    //    goto IL_325;
                    //}
                    ////到这里
                    //else if (CallFIsNext(array, i, "<link="))
                    //{
                    //    nFlag = false;
                    //    i += 6;
                    //    int num7 = i;
                    //    while (i < array.Length && array[i] != '>')
                    //    {
                    //        i++;
                    //    }
                    //    if (i > num7)
                    //    {
                    //        num5++;
                    //        var linkStr = new string(array, num7, i - num7);
                    //        Debug.LogError("linkStr:" + linkStr);
                    //        trav.Method("StartLink", new object[] { linkStr }).GetValue();
                    //        //__instance.StartLink(new string(array, num7, i - num7));
                    //    }
                    //}
                    //else
                    //{
                    //    if (!CallFIsNext(array, i, "</link>"))
                    //    {
                    //        nFlag = true;
                    //        goto IL_325;
                    //    }
                    //    else
                    //    {
                    //        nFlag = false;
                    //        num5--;
                    //        i += 6;
                    //    }
                    //} 
                    #endregion
                    char c = array[i];
                    if (c <= '\n')
                    {
                        if (c != '\t' && c != '\n')
                        {
                            goto IL_325;
                        }
                    }
                    else if (c != ' ')
                    {
                        if (c != '<')
                        {
                            goto IL_325;
                        }
                        if (CallFIsNext(array, i, "<b>"))
                        {
                            num4++;
                            i += 2;
                        }
                        else if (CallFIsNext(array, i, "</b>"))
                        {
                            num4--;
                            i += 3;
                        }
                        else if (CallFIsNext(array, i, "<link="))
                        {
                            i += 6;
                            int num7 = i;
                            while (i < array.Length && array[i] != '>')
                            {
                                i++;
                            }
                            if (i > num7)
                            {
                                num5++;
                                //this.StartLink(new string(array, num7, i - num7));
                                var linkStr = new string(array, num7, i - num7);
                                trav.Method("StartLink", new object[] { linkStr }).GetValue();
                            }
                        }
                        else if (CallFIsNext(array, i, "</link>"))
                        {
                            num5--;
                            i += 6;
                        }
                        else if (CallFIsNext(array, i, "<size="))
                        {
                            for (i += 6; i < array.Length; i++)
                            {
                                if (array[i] == '>')
                                {
                                    break;
                                }
                            }
                        }
                        else if (CallFIsNext(array, i, "</size>"))
                        {
                            i += 6;
                        }
                        else if (CallFIsNext(array, i, "<color="))
                        {
                            for (i += 7; i < array.Length; i++)
                            {
                                if (array[i] == '>')
                                {
                                    break;
                                }
                            }
                        }
                        else if (CallFIsNext(array, i, "</color>"))
                        {
                            i += 7;
                        }
                        else if (CallFIsNext(array, i, "<i>"))
                        {
                            i += 2;
                        }
                        else
                        {
                            if (!CallFIsNext(array, i, "</i>"))
                            {
                                goto IL_325;
                            }
                            i += 3;
                        }
                    }
                #endregion
                IL_481:
                    i++;
                    continue;
                IL_325:
                    if (num6 > count - 4)
                    {
                        needFix = false;
                        break;
                    }
                    for (int j = 0; j < 4; j++)
                    {
                        ___m_TempVerts[j] = verts[num6];
                        ___m_TempVerts[j].position *= d;
                        ___m_TempVerts[j].position.x += vector.x;
                        ___m_TempVerts[j].position.y += vector.y;
                        //if (j % 2 == 0)
                        //{
                        //    ___m_TempVerts[j].color = color;
                        //}
                        num6++;

                    }

                    //sb.Append(array[i]).Append("(i:").Append(i).Append("-n:").Append(num6).Append("-l:").Append(txtIndex).Append(");");
                    toFill.AddUIVertexQuad(___m_TempVerts);
                    if (num5 > 0)
                    {
                        //trav.Method("AppendLinkRect", new[] { tmp }).GetValue();
                        trav.Method("AppendLinkRect", new[] { ___m_TempVerts }).GetValue();
                        //__instance.AppendLinkRect(__instance.m_TempVerts);
                    }
                    if (!flag && num4 <= 0)
                    {
                        goto IL_481;
                    }
                    for (int k = 0; k < num2; k++)
                    {
                        for (int l = 0; l < 4; l++)
                        {
                            ___m_TempVerts[l].position.x += 1f;
                        }
                        toFill.AddUIVertexQuad(___m_TempVerts);
                        //toFill.AddUIVertexQuad(trav.Field<UIVertex[]>("m_TempVerts").Value);
                    }
                    if (num3 > 0.0001f)
                    {
                        for (int m = 0; m < 4; m++)
                        {
                            ___m_TempVerts[m].position.x += num3;
                            if (m % 2 == 1) //测试用代码
                            {
                                ___m_TempVerts[m].color = color;
                            }
                        }
                        toFill.AddUIVertexQuad(___m_TempVerts);
                        goto IL_481;
                    }
                    goto IL_481;
                }
                if (needFix)
                {
                    var _c = count - 4;
                    //Debug.LogError($"num6:{num6};_c:{_c}");
                    while (num6 < _c)
                    {
                        int tempVertsIndex = num6 & 3;
                        ___m_TempVerts[tempVertsIndex] = verts[num6];
                        ___m_TempVerts[tempVertsIndex].position *= d;
                        ___m_TempVerts[tempVertsIndex].position.x += vector.x;
                        ___m_TempVerts[tempVertsIndex].position.y += vector.y;
                        if (tempVertsIndex == 3)
                        {
                            toFill.AddUIVertexQuad(___m_TempVerts);
                        }
                        num6++;
                    }
                }
                ___m_DisableFontTextureRebuiltCallback = false;
                //Debug.LogWarning(sb.ToString());
            }
            return false;

            // return codes;

        }

        /*
                static void PopulateMeshInner(VertexHelper toFill)
                {

                    Vector2 extents = rectTransform.rect.size;
                    var settings = GetGenerationSettings(extents);
                    cachedTextGenerator.Populate(text, settings);
                    Rect inputRect = rectTransform.rect;
                    // get the text alignment anchor point for the text in local space  
                    Vector2 textAnchorPivot = GetTextAnchorPivot(m_FontData.alignment);
                    Vector2 refPoint = Vector2.zero;
                    refPoint.x = (textAnchorPivot.x == 1 ? inputRect.xMax : inputRect.xMin);
                    refPoint.y = (textAnchorPivot.y == 0 ? inputRect.yMin : inputRect.yMax);
                    // Determine fraction of pixel to offset text mesh.  
                    Vector2 roundingOffset = PixelAdjustPoint(refPoint) - refPoint;
                    // Apply the offset to the vertices  
                    IList<UIVertex> verts = cachedTextGenerator.verts;
                    float unitsPerPixel = 1 / pixelsPerUnit;
                    //Last 4 verts are always a new line...  
                    int vertCount = verts.Count - 4;
                    toFill.Clear();
                    if (roundingOffset != Vector2.zero)
                    {
                        for (int i = 0; i < vertCount; ++i)
                        {
                            int tempVertsIndex = i & 3;
                            m_TempVerts[tempVertsIndex] = verts[i];
                            m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                            m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                            m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                            if (tempVertsIndex == 3)
                                toFill.AddUIVertexQuad(m_TempVerts);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < vertCount; ++i)
                        {
                            int tempVertsIndex = i & 3;
                            m_TempVerts[tempVertsIndex] = verts[i];
                            m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                            if (tempVertsIndex == 3)
                                toFill.AddUIVertexQuad(m_TempVerts);
                        }
                    }
                    m_DisableFontTextureRebuiltCallback = false;
                }
        */

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WolText), "AppendLinkRect", new[] { typeof(UIVertex[]) })]
        static bool AppendLinkRectPrefix(WolText __instance, UIVertex[] verts, ref List<LinkRecord> ___m_aryLinkrec, ref List<Link> ___m_aryLink)
        {


            Vector2 vector = Vector2.Min(verts[0].position, verts[2].position);
            Vector2 vector2 = Vector2.Max(verts[0].position, verts[2].position);



            if (vector.y == vector2.y || vector.x == vector2.x)
            {
                return false;
            }
            //verts[0].color = color2;
            //verts[2].color = color;
            Vector2 b = (vector2 - vector) * 0.15f;
            vector -= b;
            vector2 += b;
            Rect rect = new Rect(vector, vector2 - vector);
            List<Rect> rects = ___m_aryLinkrec[___m_aryLinkrec.Count - 1].rects;
            List<Link> links = ___m_aryLinkrec[___m_aryLinkrec.Count - 1].links;
            if (rects.Count > 0)
            {
                Rect rect2 = rects[rects.Count - 1];

                if (rect.xMin > rect2.xMin && rect.xMax > rect2.xMax && rect.yMin < rect2.yMax && rect.yMax > rect2.yMin)
                {

                    rect2.min = Vector2.Min(rect2.min, rect.min);
                    rect2.max = Vector2.Max(rect2.max, rect.max);
                    rects[rects.Count - 1] = rect2;
                    links[links.Count - 1].UpdateRect(rect2);
                    return false;
                }
            }
            rects.Add(rect);
            Link link;
            if (___m_aryLink != null && ___m_aryLink.Count > 0)
            {
                link = ___m_aryLink[___m_aryLink.Count - 1];
                ___m_aryLink.RemoveAt(___m_aryLink.Count - 1);
                link.gameObject.SetActive(true);
            }
            else
            {
                link = new GameObject("link_" + ___m_aryLinkrec.Count, new Type[]
                {
                typeof(Link)
                }).GetComponent<Link>();
                link.transform.SetParent(__instance.transform, false);
            }

            link.href = ___m_aryLinkrec[___m_aryLinkrec.Count - 1].href;
            Debug.LogWarning($"link.href:{link.href};x:{rect.x};y:{rect.y};w:{rect.width};h:{rect.height}");
            link.UpdateRect(rect);

            links.Add(link);
            return false;
        }

        private struct LinkRecord
        {
            // Token: 0x04000B7F RID: 2943
            public List<Rect> rects;

            // Token: 0x04000B80 RID: 2944
            public List<Link> links;

            // Token: 0x04000B81 RID: 2945
            public string href;
        }
    }
}

/*
402	03F2	ldc.i4.0
403	03F3	stloc.s	V_23 (23)
404	03F5	br.s	434 (0439) ldloc.s V_23 (23)



 
 */