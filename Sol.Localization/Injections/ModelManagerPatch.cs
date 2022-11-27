using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sol.Localization.Injections
{
    [HarmonyPatch]
    public class ModelManagerPatch
    {
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
        static MethodInfo FIsNextMi = AccessTools.Method(typeof(ModelManager), "FIsObject", new Type[] { typeof(JSONObject) });
        static MethodInfo _FTryParseScriptState = AccessTools.Method(typeof(ModelManager), "FTryParseScriptState", new Type[] { typeof(string), typeof(MScript.State), typeof(JSONObject), typeof(Dictionary<int, int>) });

        static bool FTryParseScriptState(string txt, MScript.State state, JSONObject jSON, Dictionary<int, int> dic =null) => (bool)_FTryParseScriptState.Invoke(null, BindingFlags.NonPublic, null, new object[] { txt, state, jSON, dic }, null);

        static FieldInfo s_fOverwriteOk = AccessTools.Field(typeof(ModelManager), "s_fOverwriteOk");

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ModelManager), "ParseScripts", new[] { typeof(JSONObject) })]
        static bool ParseScriptsTranspiler(ref JSONObject jsScripts, ref Dictionary<string, Dictionary<string, Dictionary<int, int>>> ___mpStrscriptMpstrscriptMpisjoNline)
        {

            //if (!Traverse.Create(__instance).Method("FIsObject", jsScripts).GetValue<bool>())
            //{
            //    return false;
            //}

            if (!(bool)FIsNextMi.Invoke(null, BindingFlags.NonPublic, null, new object[] { jsScripts }, null))
            {
                return false;
            }
            foreach (string text in jsScripts.keys)
            {
                using (Lc.LcStringCombo<string>(new string[] { "loading script ", text }))
                {
                    var jsonObject = jsScripts[text];
                    Dictionary<string, Dictionary<int, int>> cached = null;
                    if (___mpStrscriptMpstrscriptMpisjoNline != null)
                    {
                        ___mpStrscriptMpstrscriptMpisjoNline.TryGetValue(text, out cached);
                    }
                    MScript ms;
                    if (jsonObject.IsArray)
                    {
                        ms = new MScript(text);
                        MScript.State state = ms.NewState("start", null);
                        if (!FTryParseScriptState( text, state, jsonObject, null))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (jsonObject.keys == null)
                        {
                            continue;
                        }
                        ms = new MScript(text);
                        foreach (var item in jsonObject.keys)
                        {
                            using (Lc.LcStringCombo<string>(new string[]{"loading node ",item}))
                            {
                                var job = jsonObject[item];
                                if (item!=null && job.list!=null)
                                {
                                    var mpLine = default(Dictionary<int, int>);
                                    if (cached !=null)
                                    {
                                        cached.TryGetValue(item, out mpLine);
                                    }

                                    string text3 = item;
                                    Log.WriteGameLog(text3, true);
                                    string[] array = null;
                                    int num = item.IndexOf('(');
                                    if (num > 0)
                                    {
                                        int num2 = item.LastIndexOf(')');
                                        if (num2 > num)
                                        {
                                            string text4 = item.Substring(num + 1, num2 - num - 1);
                                            speedstr[] array2;
                                            if (!string.IsNullOrEmpty(text4) && Arguments.FTrySplitArgs(text4, out array2, ArgumentsStyle.Deluxe))
                                            {
                                                array = new string[array2.Length];
                                                for (int i = 0; i < array.Length; i++)
                                                {
                                                    Log.WriteGameLog(array2[i], true);
                                                    array[i] = array2[i];
                                                }
                                            }
                                            text3 = item.Substring(0, num).Trim();
                                            string.IsNullOrEmpty(text3);
                                        }
                                    }
                                    MScript.State state2 = ms.NewState(text3, array);
                                    FTryParseScriptState(text, state2, job, mpLine);
                                }
                            }
                        }
                    }
                    
                    if ((bool)s_fOverwriteOk.GetValue(null))
                    {
                        ModelManager.scripts[text] = ms;
                    }
                    else
                    {
                        ModelManager.scripts.Add(text, ms);
                    }
                }
            }
            return false;
        }
    }
}
