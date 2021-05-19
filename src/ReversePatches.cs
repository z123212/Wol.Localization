using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wol.Localization
{
    //[HarmonyPatch]
    public static class ReversePatches
    {
        //[HarmonyReversePatch]
        //[HarmonyPatch(typeof(MCommand), "StrProcessArgFunctions")]
        //[HarmonyReversePatch]
        //[HarmonyPatch(typeof(MCommand), "StrProcessPickOne", typeof(string))]
        //public static string StrProcessPickOne(this MCommand instance, string strIn)
        //{
        //    throw new NotImplementedException("stub");
        //}
        //[HarmonyReversePatch]
        //[HarmonyPatch(typeof(MCommand), "StrPickOne")]
        //public static string StrPickOne( string strIn, int iChStart)
        //{
        //    throw new NotImplementedException("stub");
        //}
        //[HarmonyReversePatch]
        //[HarmonyPatch(typeof(MCommand), "StrProcessTranslate", typeof(string))]
        //public static string StrProcessTranslate(this MCommand instance, string str)
        //{
        //    throw new NotImplementedException("stub");
        //}


        static MethodInfo StrProcessArgFunctionsMethodInfo = typeof(MCommand).GetMethod("StrProcessArgFunctions", BindingFlags.NonPublic | BindingFlags.Static);
        static MethodInfo StrMadlibInternalMethodInfo = typeof(MCommand).GetMethod("StrMadlibInternal", BindingFlags.NonPublic | BindingFlags.Static);

        public static string StrProcessArgFunctions(this MCommand command, string str)
        {
            return StrProcessArgFunctionsMethodInfo.Invoke(command, new[] { str }).ToString();
        }
        public static string StrMadlibInternal(this MCommand command, string str)
        {
            return StrMadlibInternalMethodInfo.Invoke(command, new[] { str }).ToString();
        }
    }
}
