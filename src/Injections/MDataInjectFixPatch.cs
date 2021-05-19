using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Wol.Localization.Injections
{
    //MData.Category c, string i, Dictionary<string, string> d
    /*
     , new Type[]
  {
        typeof(MData.Category),
        typeof(string),
        typeof(Dictionary<string, string>)

  }
     */
    [HarmonyPatch(typeof(MData), MethodType.Constructor, new Type[]
  {
        typeof(MData.Category),
        typeof(string),
        typeof(Dictionary<string, string>)

})]
    public class MDataInjectFixPatch
    {

        public static void Prefix(MData.Category c, string i,ref Dictionary<string, string> d)
        {
            //Debug.Log("MData.cctor 补丁处");
            Localization.MDataReadLocaliation(d);
        }
    }
}
