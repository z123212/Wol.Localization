using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Wol.Localization.Injections
{
    [HarmonyPatch]
    public class ModelManagerInjectFixPatch
    {
        //private static MethodInfo FIsObjectMethodInfo = typeof(Console).GetMethod("FIsObject", BindingFlags.Instance | BindingFlags.NonPublic);

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ModelManager), "ParseBuckets", new Type[] { typeof(JSONObject) })]
        public static void ParseBucketsFix(ref JSONObject jsBuckets)
        {

            if (!(jsBuckets != null && jsBuckets.type == JSONObject.Type.OBJECT))
            {
                return;
            }
            foreach (string text in jsBuckets.keys)
            {
                JSONObject js = jsBuckets[text];
                if (js.IsArray)
                {
                    foreach (JSONObject jsonobject in js.list)
                    {
                        if (jsonobject.IsString)
                        {
                            string localizationStringAndWriteWithBucket = Localization.GetLocalizationStringAndWriteWithBucket(jsonobject.str, text);
                            jsonobject.str = localizationStringAndWriteWithBucket;
                        }
                    }
                }
            }
        }

        private struct ScriptParseContext
        {
            // Token: 0x04001DE0 RID: 7648
            public MCommand cmd;

            // Token: 0x04001DE1 RID: 7649
            public bool haveThen;

            // Token: 0x04001DE2 RID: 7650
            public bool haveElse;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ModelManager), "ParseScripts", new[] { typeof(JSONObject) })]
        static bool ParseScriptsTranspiler(ModelManager __instance, ref JSONObject jsScripts,ref bool ___m_fOverwriteOk)
        {

            if (!Traverse.Create(__instance).Method("FIsObject", jsScripts).GetValue<bool>())
            {
                return false;
            }
            foreach (string text in jsScripts.keys)
            {
                JSONObject jsonobject = jsScripts[text];
                if (jsonobject.keys != null)
                {
                    Dictionary<string, string> dictionary = jsonobject.ToDictionary();
                    if (dictionary == null)
                    {
                        dictionary = new Dictionary<string, string>();
                    }
                    MScript mscript = new MScript(text, dictionary);
                    foreach (string text2 in jsonobject.keys)
                    {
                        JSONObject jsonobject2 = jsonobject[text2];
                        if (jsonobject2 != null && jsonobject2.list != null)
                        {
                            MScript.State state = mscript.NewState(text2);
                            Stack<ScriptParseContext> stack = new Stack<ScriptParseContext>();
                            foreach (JSONObject jsonobject3 in jsonobject2.list)
                            {
                                if (jsonobject3.list != null && jsonobject3.list.Count != 0)
                                {
                                    string str = jsonobject3.list[0].str;
                                    if (string.IsNullOrEmpty(str))
                                    {
                                        break;
                                    }
                                    MCommand mcommand = new MCommand(str);
                                    MCommand.Op op = mcommand.op;
                                    for (int i = 1; i < jsonobject3.list.Count; i++)
                                    {
                                        if (jsonobject3.list[i] != null)
                                        {
                                            if (jsonobject3.list[i].IsString)
                                            {
                                                mcommand.AddArg(Localization.ModelManagerParseScriptsFix(jsonobject3.list[i].str, i, op));
                                            }
                                            else if (jsonobject3.list[i].IsNumber)
                                            {
                                                mcommand.AddArg(jsonobject3.list[i].n.ToString());
                                            }
                                            else if (jsonobject3.list[i].IsBool)
                                            {
                                                mcommand.AddArg(jsonobject3.list[i].b ? "1" : "0");
                                            }
                                        }
                                    }
                                    if (stack.Count > 0)
                                    {
                                        ScriptParseContext scriptParseContext = stack.Pop();
                                        if (scriptParseContext.cmd.op == MCommand.Op.IF)
                                        {
                                            if (mcommand.op == MCommand.Op.ELSE)
                                            {
                                                scriptParseContext.haveElse = true;
                                                stack.Push(scriptParseContext);
                                            }
                                            else if (scriptParseContext.haveElse)
                                            {
                                                scriptParseContext.cmd.AddCommandElse(mcommand);
                                            }
                                            else if (!scriptParseContext.haveThen)
                                            {
                                                scriptParseContext.cmd.AddCommand(mcommand);
                                                scriptParseContext.haveThen = true;
                                                stack.Push(scriptParseContext);
                                            }
                                            else
                                            {
                                                state.AddCommand(mcommand);
                                            }
                                        }
                                        else if (scriptParseContext.cmd.op == MCommand.Op.QUEUE || scriptParseContext.cmd.op == MCommand.Op.ASIDE)
                                        {
                                            scriptParseContext.cmd.AddCommand(mcommand);
                                        }
                                        else
                                        {
                                            state.AddCommand(mcommand);
                                        }
                                    }
                                    else
                                    {
                                        state.AddCommand(mcommand);
                                    }
                                    if (mcommand.op == MCommand.Op.IF)
                                    {
                                        ScriptParseContext item;
                                        item.cmd = mcommand;
                                        item.haveThen = false;
                                        item.haveElse = false;
                                        stack.Push(item);
                                    }
                                    else if (mcommand.op == MCommand.Op.QUEUE || mcommand.op == MCommand.Op.ASIDE)
                                    {
                                        ScriptParseContext item2;
                                        item2.cmd = mcommand;
                                        item2.haveThen = false;
                                        item2.haveElse = false;
                                        stack.Push(item2);
                                    }
                                }
                            }
                        }
                    }
                    if (___m_fOverwriteOk)
                    {
                        __instance.scripts[text] = mscript;
                    }
                    else
                    {
                        __instance.scripts.Add(text, mscript);
                    }
                }
            }
            return false;
        }
    }
}
