using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sol.Localization
{
    public class Log
    {

        public static void Write(string info)
        {
            Write(info, false);
        }

        public static void WriteLog(Func<string> fun, bool isForce = false)
        {
            var info = fun?.Invoke() ?? "";
            Write(info, isForce);
        }

        public static string GetStackTraceModelName()
        {
            if (!ZyConfig.OpenTrace)
            {
                return string.Empty;
            }
            StackFrame[] frames = new StackTrace().GetFrames();
            string text = "ResponseWrite,ResponseWriteError,";
            string text2 = string.Empty;
            string text3 = string.Empty;
            int num = 1;
            while (num < frames.Length && -1 != frames[num].GetILOffset())
            {
                MethodBase method = frames[num].GetMethod();
                text3 = string.Format("{0}.{1}()", method.DeclaringType, method.Name);
                if (!text.Contains(text3))
                {
                    text2 = string.Format("{0}->{1}", text3, text2);
                }
                num++;
            }
            return text2.TrimEnd(new char[]
            {
                '-',
                '>'
            });
        }


        public static void Write(string info, bool isForce)
        {
            if (isForce || ZyConfig.OpenLog)
            {
                using (StreamWriter streamWriter = new StreamWriter(Log.getPath, true))
                {
                    streamWriter.WriteLine(info);
                }
            }
        }


        public static void WriteGameLog(string log,bool appentTrace = false)
        {
            if (ZyConfig.OpenGameLog)
            {
                using (StreamWriter streamWriter = new StreamWriter(Log.gameLogPath, true))
                {
                    streamWriter.WriteLine(log);
                    if (appentTrace)
                    {
                        streamWriter.WriteLine("=========================");
                        streamWriter.WriteLine(GetStackTraceModelName());
                    }
                }
            }
        }


        static Log()
        {
            ClearLogFiles();
        }


        private static string gameLogPath;
        /// <summary>
        /// 删除本地日志文件
        /// </summary>
        public static void ClearLogFiles()
        {
            Log.gameLogPath = Localization.baseDir.TrimEnd(new char[]
               {
                '/'
               }) + "/game_log.txt";
            if (File.Exists(Log.gameLogPath))
            {
                File.Delete(Log.gameLogPath);
            }
            if (File.Exists(Log.getPath))
            {
                File.Delete(Log.getPath);
            }
        }

        private static string getPath = Localization.baseDir.TrimEnd(new char[]
        {
            '/'
        }) + "/testLog.txt";
    }
}
