using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Wol.Localization
{
    public class ZyConfig
    {
        /// <summary>
        /// 转换的语言包 对应配置文件 Language 字段
        /// </summary>
        public static string LocalLan { get; private set; }
        /// <summary>
        /// 是否输出汉化运行日志 对应配置文件open_log字段
        /// </summary>
        public static bool OpenLog { get; private set; } = false;
        /// <summary>
        /// 是否输出转换日志 对应配置文件 open_lan_out 字段
        /// </summary>
        public static bool OpenLan { get; private set; } = false;
        /// <summary>
        /// 开启跟踪日志 open_trace
        /// </summary>
        public static bool OpenTrace { get; private set; } = false;

        public static bool OpenGameLog { get; private set; } = false;
        /// <summary>
        /// 是否开启本地化
        /// </summary>
        public static bool OpenLocaliz { get; private set; } = false;

        public static bool AutoLoad { get; private set; } = false;

        static ZyConfig()
        {


            string path = Path.Combine(Localization.baseDir, "conf.json"); //Directory.GetCurrentDirectory() + "/Localization/conf.json";
            Debug.Log($"配置文件地址：{path}");
            if (File.Exists(path))
            {
                Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path, Encoding.UTF8));
                if (dictionary != null && dictionary.Count > 0)
                {
                    string empty = string.Empty;
                    if (dictionary.TryGetValue("Language", out empty))
                    {
                        LocalLan = empty;
                    }
                    if (dictionary.TryGetValue("open_lan_out", out empty) && (empty == "true" || empty == "1"))
                    {
                        OpenLan = true;
                    }
                    if (dictionary.TryGetValue("open_log", out empty) && (empty == "true" || empty == "1"))
                    {
                        OpenLog = true;
                    }
                    if (dictionary.TryGetValue("open_trace", out empty) && (empty == "true" || empty == "1"))
                    {
                        OpenTrace = true;
                    }
                    if (dictionary.TryGetValue("open_game_log", out empty) && (empty == "true" || empty == "1"))
                    {
                        OpenGameLog = true;
                    }
                    if (dictionary.TryGetValue("open", out empty) && (empty == "true" || empty == "1"))
                    {
                        OpenLocaliz = true;
                    }
                    if (dictionary.TryGetValue("open_load_tran", out empty) && (empty == "true" || empty == "1"))
                    {
                        AutoLoad = true;
                    }
                }
            }
        }
    }
}
