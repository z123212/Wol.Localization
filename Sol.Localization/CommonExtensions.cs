using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace Sol.Localization
{
    public static class CommonExtensions
    {
        /// <summary>
        /// <see cref="Localization.RepliceLinkString(string)"/>
        /// </summary>
        public static MethodInfo RepliceLinkStringType = AccessTools.Method(typeof(Localization), nameof(Localization.RepliceLinkString), new[] { typeof(string) });
        /// <summary>
        /// <see cref="Localization.GetLocalizationStringAndWrite(string)"/>
        /// </summary>
        public static MethodInfo GetLocalizationStringAndWriteType = AccessTools.Method(typeof(Localization), nameof(Localization.GetLocalizationStringAndWrite), new[] { typeof(string) });

        public static CodeMatcher WhenValid(this CodeMatcher codeMatcher, Func<CodeMatcher, CodeMatcher> func)
        {
            if (codeMatcher?.IsValid == true)
            {
                return func?.Invoke(codeMatcher);
            }
            return codeMatcher;
        }
        /// <summary>
        /// 为MCommand的实例附加一个原始的MadlibString方法，因为这个方法被该补丁修补了
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="strOriginal"></param>
        /// <returns></returns>
        public static string MadlibStringFixOrigin(this MCommand cmd, string strOriginal)
        {
            if (string.IsNullOrEmpty(strOriginal))
            {
                return null;
            }
            return strOriginal;
            //var trav = Traverse.Create<MCommand>();
            //trav.Field("s_nMadlibDepth").SetValue(0);
            //trav.Field("s_strMadlibOriginal").SetValue(strOriginal);
            //var strTrim = MData.TrimMore(MData.UnescapeQuote(strOriginal));
            //var r = cmd.StrMadlibInternal(strTrim);//trav.Method("StrMadlibInternal", strTrim).GetValue<string>();
            //string result = r.Replace("<pct>", "%");
            //trav.Field("s_strMadlibOriginal").SetValue(null);
            ////Debug.Log($"MCommandEx.MadlibStringFixOrigin:入参：{strOriginal};出参:{result}");
            //return result;
        }

        public static void LogInstructions(this IEnumerable<CodeInstruction> codeInstructions)
        {
            //return;
#if DEBUG
            var codess = codeInstructions.ToList();
            StringBuilder sb = new StringBuilder("\n");
            for (int i = 0; i < codess.Count; i++)
            {
                var item = codess[i];
                sb.Append("L:").Append(i.ToString("x2")).Append("    ").Append(item.opcode.ToString());
                if (item.operand != null)
                {
                    sb.Append("  ").Append(item.operand.ToString());
                }
                sb.AppendLine();
            }
            Debug.Log(sb.ToString());
#endif
        }

     


    }
}
