using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Wol.Localization.Injections;

namespace Wol.Localization
{
    public class Localization
    {




        private static List<string> cached;
        private static bool isLoad = false;
        internal static string baseDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Localization");
        private static char[] Split = "|".ToCharArray();
        private static object Locker = new object();
        private static string lanPath = "";
        private static bool isOut = false;
        private static string outPath = Path.Combine(Localization.baseDir, "out.txt");
        private static Dictionary<string, string> LanguageCache = new Dictionary<string, string>();
        private static readonly Regex RE_COMMAND = new Regex("^[\\!]?([^\\(]+)\\(");
        private static Regex LinkRegx = new Regex("^literal\\:(.+) (\\[.+?\\])$");
        private static bool isFirstReload = true;
        private static string[] ignoreList = new string[]
            {
            "hasflag",
            "hasstate",
            "hasproperty",
            "isset",
            "rand",
            "min",
            "max",
            "abs",
            "itemroll",
            "hasskill",
            "hasitem",
            "hasgear",
            "matchstring",
            "nodeexists",
            "divisible",
            "emptyspace",
            "hasnpcstoreitem",
            "hasjournal",
            "haseffect",
            "hasachievement",
            "hasdlc",
            "countflags",
            "countdailyflags",
            "countpermaflags"
            };
        private static string[] duplexingList = new string[]
            {
            "prompt_dials_month"
            };
        private static Dictionary<string, string> duplexingCached = new Dictionary<string, string>();

        /// <summary>
        ///	将指定值写入缓存
        /// </summary>
        /// <param name="txt"></param>
        public static void WriteCached(string txt)
        {

            if (!Localization.IsReadLocali(txt))
            {
                return;
            }
            object locker;
            if (Localization.cached == null)
            {
                locker = Localization.Locker;
                lock (locker)
                {
                    Localization.cached = new List<string>();
                    if (File.Exists(Localization.outPath))
                    {
                        foreach (string text in File.ReadAllLines(Localization.outPath))
                        {
                            if (!string.IsNullOrEmpty(text))
                            {
                                Localization.cached.Add(text);
                            }
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(txt))
            {
                return;
            }
            locker = Localization.Locker;
            lock (locker)
            {
                Dictionary<string, string> languageCache = Localization.LanguageCache;
                if ((languageCache == null || !languageCache.ContainsKey(txt)) && !Localization.cached.Contains(txt))
                {
                    Localization.cached.Add(txt);
                    if (ZyConfig.OpenLog)
                    {
                        using (StreamWriter streamWriter = new StreamWriter(Localization.outPath, true))
                        {
                            streamWriter.WriteLine(txt);
                        }

                        var track = "";
                        if (ZyConfig.OpenTrace)
                        {
                            track = Log.GetStackTraceModelName();

                        }
                        Log.WriteGameLog(string.Format("{0}{1}{0}-----函数调用链------{0}{2}{0}==========", Environment.NewLine, txt, track));
                    }

                }
            }
        }

        /// <summary>
        /// 加载配置文件，及汉化词条
        /// </summary>
        public static void Load()
        {
            if (Localization.isLoad)
            {
                return;
            }
            object locker = Localization.Locker;
            lock (locker)
            {
                if (!Localization.isLoad)
                {
                    Localization.InitConf();
                    Localization.isLoad = true;
                    Log.Write("加载配置文件成功");
                    if (Localization.lanPath?.Length > 0)
                    {
                        string text2 = Path.Combine(Localization.lanPath, "data.json"); //string.Format("{0}/data.txt", Localization.lanPath);
                        Debug.Log($"文件路径:{text2}");
                        string text3 = Localization.LoadFile(text2);
                        if (text3 != string.Empty)
                        {
                            Log.Write(string.Format("读取本地化文件成功;路径：{0};", text2));
                            Localization.InitCached(text3);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        private static void InitConf()
        {
            Localization.lanPath = ZyConfig.LocalLan;
            Localization.isOut = ZyConfig.OpenLan;
        }

        /// <summary>
        /// 根据key获取本地化词条钟的信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetLocalizationString(string key)
        {
            return Localization.GetLocalizationString(key, false);
        }

        /// <summary>
        /// 初始化本地汉化数据，目前是加载json文件
        /// </summary>
        /// <param name="str"></param>
        private static void InitCached(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                Localization.LanguageCache = JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
                Log.Write("加载本地化数据成功");
            }
        }

        /// <summary>
        /// 加载指定文件，并且以Encoding.UTF8为字符集加载文件内容
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static string LoadFile(string file)
        {
            //var dllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //Debug.LogError($"dllPath:{dllPath}");

            string path = Path.Combine(Localization.baseDir, file);// string.Format("{0}{1}", Localization.baseDir, file);
            Debug.Log($"filePath:{path}");
            if (File.Exists(path))
            {
                Debug.Log("文件存在");
                return File.ReadAllText(path, Encoding.UTF8);
            }
            return string.Empty;
        }


        static Localization()
        {
            if (File.Exists(Localization.outPath))
            {
                File.Delete(Localization.outPath);
            }
        }

        /// <summary>
        /// 根据key获取本地化词条并写入缓存
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string GetLocalizationStringAndWrite(string txt)
        {
            if (string.IsNullOrEmpty(txt))
            {
                return txt;
            }
            return Localization.GetLocalizationStringAndWrite(txt, false);
        }


        public static string GetLocalizationString(string key, bool isToLow)
        {
            if (string.IsNullOrEmpty(key))
            {
                return key;
            }
            Localization.LoadLocalType type = isToLow ? Localization.LoadLocalType.ToLower : Localization.LoadLocalType.None;
            return Localization.PrivGetLocalizationString(key, type);
        }


        public static string GetLocalizationStringAndWrite(string txt, bool isToLow)
        {
            if (string.IsNullOrEmpty(txt))
            {
                return txt;
            }
            //Localization.WriteCached(txt);
            return Localization.GetLocalizationString(txt, isToLow);
        }


        public static void MDataReadLocaliation(Dictionary<string, string> dic)
        {
            if (dic == null || dic.Count == 0)
            {
                return;
            }
            Action<string> action = delegate (string key)
            {
                if (dic.ContainsKey(key))
                {
                    string text = dic[key];
                    if (key == "name" && !string.IsNullOrEmpty(text))
                    {
                        text = Localization.ToUperFirstChar(text);
                    }
                    dic[key] = Localization.GetLocalizationStringAndWrite(text);
                }
            };
            action("name");
            action("bluetext");
            action("note");
            action("sfx");
            action("logmessage");
            action("description");
            action("buttontext");
            action("uselink");
        }


        private static string PrivGetLocalizationString(string key, Localization.LoadLocalType type)
        {
            if (!ZyConfig.OpenLocaliz)
            {
                return key;
            }
            if (string.IsNullOrEmpty(key) || Localization.LanguageCache.Count == 0)
            {
                return key;
            }
            if (!Localization.IsReadLocali(key))
            {
                return key;
            }
            string result = key;
            key = Localization.ComvertKey(key, type);

            if (Localization.LanguageCache.TryGetValue(key, out var _result) && _result?.Length > 0)
            {
                return _result;
            }
            WriteCached(key);
            return result;
        }


        internal static string GetLocalizationStringAndWrite2(string txt, Localization.LoadLocalType type)
        {
            string text = Localization.PrivGetLocalizationString(txt, type);
            if (text == txt)
            {
                if (type == Localization.LoadLocalType.None)
                {
                    //Localization.WriteCached(txt);
                    return text;
                }
                //Localization.WriteCached(Localization.ComvertKey(txt, type));
            }
            return text;
        }

        static Regex IgnoreString = new Regex(@"(^\d+$)|");

        private static bool IsReadLocali(string txt)
        {

            Match match = Localization.RE_COMMAND.Match(txt);
            if (match != null && match.Success)
            {
                string value = match.Groups[1].Value;
                if (Enumerable.Contains<string>(Localization.ignoreList, value.ToLower()))
                {
                    return false;
                }
            }
            return true;
        }


        public static string ToUperFirstChar(string txt)
        {
            char[] array = txt.ToCharArray();
            bool flag = true;
            int i = 0;
            int num = array.Length;
            while (i < num)
            {
                char c = array[i];
                if (char.IsWhiteSpace(c))
                {
                    flag = true;
                }
                else if (flag)
                {
                    flag = false;
                    array[i] = char.ToUpper(c);
                }
                i++;
            }
            return new string(array);
        }


        public static string MadlibStringFix(string strOriginal, MCommand cmd, int index)
        {
            string text = string.Empty;
            if (cmd == null)
            {
                text = cmd.MadlibStringFixOrigin(strOriginal);
            }
            else
            {
                MCommand.Op op = cmd.op;
                if (index == 0 && (op == MCommand.Op.ADDPROPERTY || op == MCommand.Op.ADDXP || op == MCommand.Op.ADDSTATE || op == MCommand.Op.ADDFLAG || op == MCommand.Op.GOTO || op == MCommand.Op.DELFLAG || op == MCommand.Op.DELPERMAFLAG || op == MCommand.Op.ANIMATE || op == MCommand.Op.PLAYSOUND || op == MCommand.Op.MOVEELEMENT || op == MCommand.Op.TELEPORTELEMENT || op == MCommand.Op.RUNSCRIPT || op == MCommand.Op.WAA))
                {
                    text = cmd.MadlibStringFixOrigin(strOriginal);
//#if DEBUG
//                    if (text == strOriginal)
//                    {
//                        Log.Write($"当前输入:{text}；与输出一致\nTracing:{Log.GetStackTraceModelName()}\n=================\n", true);
//                    }
//#endif
                }
                else
                {
                    text = MCommand.MadlibString(strOriginal);
                }
            }
            //Log.WriteGameLog(string.Format("strOriginal:{0};cmd:{1},index:{2},resc={4}\r\n{3}\r\n\r\n", new object[]
            //{
            //strOriginal,
            //((cmd != null) ? cmd.op.ToString() : null) ?? "",
            //index,
            //Log.GetStackTraceModelName(),
            //text
            //}));
            return text;
        }

        public static string ModelManagerParseScriptsFix(string str, int i, MCommand.Op op)
        {
            if (i > 1 || ((op == MCommand.Op.SAY || op == MCommand.Op.LEFTSPEAK || op == MCommand.Op.RIGHTSAY || op == MCommand.Op.SAYCENTER || op == MCommand.Op.SAYCOLUMNS || op == MCommand.Op.LEFTSAY || op == MCommand.Op.RIGHTSAY || op == MCommand.Op.LEFTSPEAK || op == MCommand.Op.RIGHTSPEAK || op == MCommand.Op.POPTEXT) && i > 0))
            {
                //Localization.WriteCached(str);
                str = Localization.GetLocalizationStringAndWrite(str);
            }
            return str;
        }

        //专门为Command.SetPropertyFromArgs方法做的一个适配方法
        public static string CommandSetPropertyFromArgsFix(string text, MCommand.Op op)
        {
            if (op == MCommand.Op.ADDPROPERTY || op == MCommand.Op.ADDSTATE || op == MCommand.Op.GRAYOPTION || op == MCommand.Op.PUSHRANDOM)
            {
                return Localization.GetLocalizationStringAndWrite(text);
            }
            return text;
        }

        //public static string GetItemTextWithPlayerItem(MItem item)
        //{
        //    StringBuilder stringBuilder = new StringBuilder();
        //    stringBuilder.AppendLine(item.StrFullDescriptionNoName());
        //    MItem mitem = MPlayer.instance.ItemGetGear(item.slot);
        //    if (mitem != null && item.invid != mitem.invid)
        //    {
        //        stringBuilder.Append("\n--------------------\n<b><color=blue>");
        //        stringBuilder.Append(Localization.GetLocalizationString("[EQUIPPED STATS]"));
        //        stringBuilder.AppendLine("</color></b>\n");
        //        string value = mitem.StrFullDescriptionStoreNoSellValue();
        //        stringBuilder.Append(value);
        //    }
        //    return stringBuilder.ToString();
        //}


        public static string RepliceLinkString(string txt)
        {
            if (!string.IsNullOrEmpty(txt))
            {
                Match match = Localization.LinkRegx.Match(txt);
                if (match.Success)
                {
                    string localizationStringAndWrite = Localization.GetLocalizationStringAndWrite(match.Groups[1].Value);
                    return string.Format("literal:{0} {1}", localizationStringAndWrite, match.Groups[2].Value);
                }
            }
            return txt;
        }


        public static string ReplaceWolText(string txt)
        {
            if (!string.IsNullOrEmpty(txt))
            {
                return Localization.GetLocalizationStringAndWrite2(txt, Localization.LoadLocalType.ToUpper);
            }
            return txt;
        }


        private static string ComvertKey(string key, Localization.LoadLocalType type)
        {
            string result = key;
            if (type == Localization.LoadLocalType.ToLower)
            {
                result = key.ToLower();
            }
            else if (type == Localization.LoadLocalType.ToUpper)
            {
                result = key.ToUpper();
            }
            else if (type == Localization.LoadLocalType.ToFirstChar)
            {
                result = Localization.ToUperFirstChar(key);
            }
            return result;
        }

        /// <summary>
        /// 重新加载资源
        /// </summary>
        public static void Reload()
        {
            if (ZyConfig.AutoLoad)
            {
                lock (Locker)
                {
                    isLoad = false;
                    LanguageCache.Clear();
                    cached.Clear();
                    Log.ClearLogFiles();
                    if (File.Exists(Localization.outPath))
                    {
                        File.Delete(Localization.outPath);
                    }

                    Load();
                }
            }


            //if (ZyConfig.AutoLoad)
            //{
            //    if (Localization.isFirstReload)
            //    {
            //        Localization.isFirstReload = false;
            //        return;
            //    }
            //    string text = Localization.lanPath;
            //    if (text != null && text.Length > 0)
            //    {
            //        string text2 = Path.Combine(Localization.lanPath, "data.txt");// string.Format("{0}/data.txt", Localization.lanPath);
            //        string text3 = Localization.LoadFile(text2);
            //        if (text3 != string.Empty)
            //        {
            //            Log.Write(string.Format("再次读取本地化文件成功;路径：{0};", text2));
            //            Localization.InitCached(text3);
            //        }
            //    }
            //}
        }


        public static string GetLocalizationStringAndWriteWithBucket(string txt, string bucketName)
        {
            string text;
            if (!string.IsNullOrEmpty(bucketName) && Enumerable.Contains<string>(Localization.duplexingList, bucketName))
            {
                text = Localization.PrivGetLocalizationString(txt, Localization.LoadLocalType.ToUpper);
                if (text != txt)
                {
                    string key = string.Format("{0}.{1}", bucketName, text);
                    if (Localization.duplexingCached.ContainsKey(key))
                    {
                        Localization.duplexingCached[key] = txt;
                    }
                    else
                    {
                        Localization.duplexingCached.Add(key, txt);
                    }
                }
            }
            else
            {
                text = Localization.GetLocalizationStringAndWrite(txt);
            }
            return text;
        }

        /// <summary>
        /// 主要解决汉化词条中出现和命令一直的词，从而影响游戏。 
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public static string ResolveLocalizationDuplex(string txt, string bucketName)
        {
            string text = txt;
            if (!string.IsNullOrEmpty(bucketName) && Enumerable.Contains<string>(Localization.duplexingList, bucketName))
            {
                string key = string.Format("{0}.{1}", bucketName, txt);
                if (Localization.duplexingCached.ContainsKey(key))
                {
                    text = Localization.duplexingCached[key];
                }
            }
            /*
             
============ResolveLocalizationDuplex===========
key=prompt_dials_month.八月;result = August
             */
            Log.Write(string.Format("============ResolveLocalizationDuplex===========\r\nkey={0};result = {1}", bucketName + "." + txt, text));
            return text;
        }


        internal enum LoadLocalType : short
        {

            None,

            ToUpper,

            ToLower,

            ToFirstChar
        }


        public class KeyValuesBuffer
        {

            public void Stored(string key, string val)
            {
                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(val))
                {
                    return;
                }
                if (this._cached.ContainsKey(key))
                {
                    this._cached[key] = val;
                    return;
                }
                this._cached.Add(key, val);
            }


            public string LoadValBykey(string key)
            {
                if (string.IsNullOrEmpty(key))
                {
                    return "";
                }
                if (this._cached.ContainsKey(key))
                {
                    return this._cached[key];
                }
                return "";
            }


            private Dictionary<string, string> _cached = new Dictionary<string, string>();
        }

    }
}
