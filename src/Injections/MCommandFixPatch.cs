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
    /// <summary>
    /// MCommand 类注入拦截器
    /// </summary>
    //[HarmonyPatch(typeof(MCommand), "MadlibString", new Type[]
    //{
    //    typeof(string)
    //})]
    [HarmonyPatch(typeof(MCommand))]
    public class MCommandFixPatch
    {

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MCommand.MadlibString), new Type[] { typeof(string) })]
        static bool MadlibStringPatch(MCommand __instance, ref string strOriginal, ref string __result, ref int ___s_nMadlibDepth, ref string ___s_strMadlibOriginal)
        {
            ___s_nMadlibDepth = 0;//trav.Field("s_nMadlibDepth").SetValue(0);
            ___s_strMadlibOriginal = strOriginal; //trav.Field("s_strMadlibOriginal").SetValue(strOriginal);
            var strTrim = MData.TrimMore(MData.UnescapeQuote(strOriginal));
            var _diffStr = strTrim;//临时用于比较作用为测试使用
            if (!string.IsNullOrEmpty(strTrim))
            {

                strTrim = Localization.GetLocalizationStringAndWrite(strTrim);
#if DEBUG
                if (_diffStr == strTrim)
                {
                    //如果返回的都一样，做一个特殊日志输出
                    Log.Write($"当前输入:{strTrim}；与输出一致,\nTracing:{Log.GetStackTraceModelName()}\n=================\n");
                }
#endif

            }
            var r = __instance.StrMadlibInternal(strTrim);//call MCommand.StrMadlibInternal 
            string result = r.Replace("<pct>", "%");
            ___s_strMadlibOriginal = null;//trav.Field("s_strMadlibOriginal").SetValue(null);
            __result = result;
            //Debug.Log($"MCommandEx.MadlibStringFixOrigin:入参：{strOriginal};出参:{result}");
            return false;


        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MCommand.StrArg), new Type[] { typeof(int) })]
        static bool StrArgPatch(MCommand __instance, ref string __result, int iStr, ref string[] ___m_aStrArgResolved)
        {

            if (iStr < 0 || iStr >= __instance.argCount)
            {

                return true;
            }
            else
            {
                //var trav = Traverse.Create(__instance);
                var aStrArgResolved = ___m_aStrArgResolved;//Traverse.Create(__instance).Field("m_aStrArgResolved").GetValue() as string[];

                var text = ___m_aStrArgResolved[iStr];
                if (text == null)
                {
                    var argsSplitTrav = Traverse.Create(__instance).Property("argsSplit").GetValue() as string[];

                    var argsSplit = argsSplitTrav[iStr];// trav.Field("argsSplit").GetValue<string[]>()[iStr];
                    var txt = __instance.StrProcessArgFunctions(argsSplit); //Traverse.Create<MCommand>().Method("StrProcessArgFunctions", new Type[] { typeof(string) }).GetValue<string>(argsSplit);
                    //Debug.Log($"MCommandFixPatch.StrArgPatch:\n argsSplit={argsSplit};StrProcessArgFunctions()={txt}");
                    text = Localization.MadlibStringFix(txt, __instance, iStr);

                    ___m_aStrArgResolved[iStr] = text;//trav.Field("m_aStrArgResolved").GetValue<string[]>()[iStr] = text;
                }
                if (__instance.op == MCommand.Op.MAPUNLOCK && iStr > 0)
                {
                    text = Localization.GetLocalizationStringAndWrite(text);
                }
                __result = text;
                //Debug.Log($"MCommandFixPatch.StrArgPatch:\n入参：{iStr};\n出参:{text}");
                return false;
            }

        }

        //private static readonly CodeMatch MatchCallTakeItems =
        //    new CodeMatch(i => i.Calls(typeof(MCommand).GetMethod(nameof(PlanetFactory.TakeBackItemsInEntity))));
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MCommand), "SetPropertyFromArgs", typeof(int), typeof(string))]
        static IEnumerable<CodeInstruction> SetPropertyFromArgsTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            /*
ldarg.0
call	instance valuetype MCommand/Op MCommand::get_op()
call	string Localization::CommandSetPropertyFromArgsFix(string, valuetype MCommand/Op)
             */
            /*
             将原方法中：
    if (text2.StartsWith('='))
	{
		text2 = MExpression.Evaluate(text2.Substring(1)).ToString();
	}
	else
	{
		text2 = MCommand.MadlibString(text2); //被替换点
	}
            替换为：text2 = Localization.CommandSetPropertyFromArgsFix(MCommand.MadlibString(text2), this.op);
             */
            var cmdOp = AccessTools.Property(typeof(MCommand), nameof(MCommand.op)).GetGetMethod();
            var repMi = AccessTools.Method(typeof(Localization), nameof(Localization.CommandSetPropertyFromArgsFix), new[] { typeof(string), typeof(MCommand.Op) });


            var retInst = new CodeMatcher(instructions)
                .Start()
               .MatchForward(true,
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(MCommand), "MadlibString", new[] { typeof(string) }))
               )
               .Advance(1)
               .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, cmdOp),
                    new CodeInstruction(OpCodes.Call, repMi)
               )

               //.RemoveInstructions(3)
               //.Insert(n)


               .InstructionEnumeration();

            //retInst.LogInstructions();
            return retInst;

            throw new NotImplementedException();
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MCommand), "StrBucket", typeof(string))]
        static IEnumerable<CodeInstruction> StrBucketTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            // MCommand.StrMadlibInternal(text);
            /*
58	006F	ldloc.3
59	0070	call	string Localization::GetLocalizationStringAndWrite(string)
60	0075	stloc.3

             */
            var retIl = new CodeMatcher(instructions)
                .Start()
                .MatchForward(false, new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(MCommand), "StrMadlibInternal", new[] { typeof(string) })))
                .Advance(-1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Call,CommonExtensions.GetLocalizationStringAndWriteType), //AccessTools.Method(typeof(Localization), nameof(Localization.GetLocalizationStringAndWrite), new[] { typeof(string) })),
                    new CodeInstruction(OpCodes.Stloc_3)
                )

                .InstructionEnumeration();
            return retIl;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MCommand), "StrMadlibInternal", typeof(string))]
        static IEnumerable<CodeInstruction> StrMadlibInternalTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            // MCommand.StrMadlibInternal(text);
            /*
             * find:
ldloc.0
ldc.i4.0
ldloc.1
callvirt	instance string [netstandard]System.String::Substring(int32, int32)
ldloc.s	V_5 (5)
ldloc.0
ldloc.1
ldloc.2
add
callvirt	instance string [netstandard]System.String::Substring(int32)
call	string [netstandard]System.String::Concat(string, string, string)
stloc.0
br	7 (000D) ldloc.0 
            ==============================
ldloc.s	V_5 (5)
call	string Localization::GetLocalizationStringAndWrite(string)
stloc.s	V_5 (5)
             */

            var subMi = AccessTools.Method(typeof(String), "Substring", new[] { typeof(int), typeof(int) });//typeof(String).GetMethod("Substring", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var ConcatMi = AccessTools.Method(typeof(String), "Concat", new[] { typeof(string), typeof(string), typeof(string) });

            /*
curr:[predicate=yes]
lin:ldloc.s 5 (System.String)
    lb=System.String (5);Opcodes=ldloc.s
     lb.LocalIndex=5; lb.LocalType=System.String
this.predicate = False
             */

            /*
在该方法返回之前的代码：text = text.Substring(0, num) + text3 + text.Substring(num + num2); 前添加汉化代码

             */

            //var local5 = new LocalVariableInfo();

            var retIl = new CodeMatcher(instructions)
                .Start()
                .MatchForward(false,
                    new CodeMatch(OpCodes.Ldloc_0, null),
                    new CodeMatch(OpCodes.Ldc_I4_0, null),
                    new CodeMatch(OpCodes.Ldloc_1, null),
                    new CodeMatch(OpCodes.Callvirt, subMi),
                     new CodeMatch(c =>
                     {
                         var lb = c.operand as LocalBuilder;
                         if (lb == null)
                         {
                             return false;
                         }
                         //Debug.LogError(c.operand);
                         //Debug.LogError(c.operand.GetType().FullName);
                         var strType = typeof(string);
                         return c.opcode == OpCodes.Ldloc_S && lb.LocalIndex == 5 && lb.LocalType == strType;
                     })
                )
            .Insert(
                new CodeInstruction(OpCodes.Ldloc_S, 5),
                new CodeInstruction(OpCodes.Call, CommonExtensions.GetLocalizationStringAndWriteType),//AccessTools.Method(typeof(Localization), nameof(Localization.GetLocalizationStringAndWrite), new[] { typeof(string) })),
                new CodeInstruction(OpCodes.Stloc_S, 5)
            )
            //.ToString();
                .InstructionEnumeration();
            //retIl.LogInstructions();
            return retIl;

        }

        [HarmonyPrefix]
        [HarmonyPatch("StrSay", typeof(string))]
        static void StrSayPrefix(ref string strArg)
        {
            strArg = Localization.GetLocalizationStringAndWrite(strArg);
        }
    }

}
