
using System;
using System.Collections.Generic; 
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace XFGameFramework
{
  
    public class LogUtil
    {

        #region 静态字段

        private static StringBuilder TempStringBuilder = new StringBuilder();
        private static Queue<string> WaitToWriteFileLogs = new Queue<string>();
        private static bool WritingFile = false;
        private static Task WriteToFileTask = null;
        private static bool isStopWriteToFile = false;
        private static StreamWriter writer = null;
        #endregion

        #region 静态属性

        /// <summary>
        /// 日志文件所在的目录
        /// </summary>
        public static string LogFileDirectory => string.Format("{0}/Logs", Application.temporaryCachePath);

        /// <summary>
        /// 当前进程写入的日志文件路径
        /// </summary>
        public static string LogFilePath
        {
            get; private set;
        }

        #endregion

        #region 静态事件

        /// <summary>
        /// 当有异常时触发(Debug.unityLogger.logEnabled需为打开状态，否则该事件不会触发)
        /// </summary>
        public static Action OnException;
        #endregion

        #region 静态方法

        static LogUtil()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            Init();

#if !LOG_UTIL_WRITE_TO_FILE_DISABLE
            LogFilePath = string.Format("{0}/player_log_{1}.txt", LogFileDirectory, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            Debug.LogFormat("日志文件路径:{0}", LogFilePath);
#endif
        }

        private static void Init()
        {
            Application.logMessageReceived += OnReceiveLog;
            CheckLogFileDate();
        }

        public static void Log(string message, params object[] args)
        {

#if !LOG_UTIL_WRITE_TO_CONSOLE_DISABLE
            if (args == null || args.Length == 0)
                Debug.Log(message);
            else
                Debug.LogFormat(message, args);
#endif

#if !LOG_UTIL_WRITE_TO_FILE_DISABLE
            if (isStopWriteToFile) return;
            TempStringBuilder.Clear();
            TempStringBuilder.Append("[").Append(DateTime.Now.ToString()).Append("]").Append("[Log]").Append(string.Format(message,args)).AppendLine();
            ReadyWriteToFile(TempStringBuilder.ToString());
#endif

        }

        public static void LogWarning(string message, params object[] args)
        {

#if !LOG_UTIL_WRITE_TO_CONSOLE_DISABLE
            if (args == null || args.Length == 0)
                Debug.LogWarning(message);
            else
                Debug.LogWarningFormat(message, args);
#endif

#if !LOG_UTIL_WRITE_TO_FILE_DISABLE
            if (isStopWriteToFile) return;
            TempStringBuilder.Clear();
            TempStringBuilder.Append("[").Append(DateTime.Now.ToString()).Append("]").Append("[Warning]").Append(string.Format(message, args)).AppendLine();
            ReadyWriteToFile(TempStringBuilder.ToString());
#endif
        }

        public static void LogError(string message, params object[] args)
        {

#if !LOG_UTIL_WRITE_TO_CONSOLE_DISABLE
            if (args == null || args.Length == 0)
                Debug.LogError(message);
            else
                Debug.LogErrorFormat(message, args);
#endif

#if !LOG_UTIL_WRITE_TO_FILE_DISABLE
            if (isStopWriteToFile) return;
            TempStringBuilder.Clear();
            TempStringBuilder.Append("[").Append(DateTime.Now.ToString()).Append("]").Append("[Error]").Append(string.Format(message, args)).AppendLine();
            ReadyWriteToFile(TempStringBuilder.ToString());
#endif

        }

        public static void StopWriteToFile()
        {
            try
            {
                isStopWriteToFile = true;
                WaitToWriteFileLogs.Clear();
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
            }
            catch (Exception)
            {
            }

        }


        /// <summary>
        /// 检测日志文件是否超过三天，如果超过三天就清理掉
        /// </summary>
        private static void CheckLogFileDate()
        {
            if (!Directory.Exists(LogFileDirectory)) return;

            // 异步检测 防止影响游戏启动时间 
            Task.Run(() =>
            {
                string[] files = Directory.GetFiles(LogFileDirectory);
                for (int i = 0; i < files.Length; i++)
                {
                    try
                    {
                        DateTime createTime = File.GetCreationTime(files[i]);
                        if ((DateTime.Now - createTime).Days > 3)
                        {
                            File.Delete(files[i]);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            });
        }

        private static void OnReceiveLog(string condition, string stackTrace, LogType type)
        {
            // 这里只关心报错信息
            if (type != LogType.Exception) return;

#if !LOG_UTIL_WRITE_TO_FILE_DISABLE
            if (isStopWriteToFile) return;
            TempStringBuilder.Clear();
            TempStringBuilder.Append("[").Append(DateTime.Now.ToString()).Append("]").Append("[").Append(type.ToString()).Append("]").Append(condition).AppendLine();
            TempStringBuilder.AppendLine(stackTrace);
            ReadyWriteToFile(TempStringBuilder.ToString());
#endif

#if !UNITY_EDITOR
            OnException?.Invoke(); // 如果抛异常并且非Editor模式，触发事件
#endif

        }

        private static void StartWriteToFile()
        {
            if (WritingFile) return;
            WritingFile = true;
            // 需要监听编辑器状态 关掉任务 TODO
            WriteToFileTask = Task.Run(() =>
            { 
                try
                {
                    string dir = Path.GetDirectoryName(LogFilePath);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    writer = File.AppendText(LogFilePath);
                    while (WaitToWriteFileLogs.Count > 0)
                    {
                        writer.WriteLine(WaitToWriteFileLogs.Dequeue());
                        writer.Flush();
                    }
                    writer.Close();
                }
                catch (Exception)
                {
                }

                writer = null;
                WritingFile = false;
                WriteToFileTask = null;
            });
        }

        private static void ReadyWriteToFile(string message)
        {
            if (isStopWriteToFile) return;
#if !LOG_UTIL_WRITE_TO_FILE_DISABLE

            try
            {
                // 写入文件
                WaitToWriteFileLogs.Enqueue(message);
                // 开始写入
                StartWriteToFile();
            }
            catch (Exception)
            {
            }
#endif 
        }
        #endregion


#if UNITY_EDITOR 
        private static bool opening = false; 
        /// <summary>
        /// 获取当前日志窗口选中的日志的堆栈信息
        /// </summary>
        /// <returns></returns>
        private static string GetStackTrace()
        {
            var consoleWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");

            //获取窗口实例
            var fileInfo = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            var consoleIns = fileInfo?.GetValue(null);
            if (consoleIns == null) return null;
            if (EditorWindow.focusedWindow == (EditorWindow)consoleIns)
            {
                fileInfo = consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                var activeText = fileInfo?.GetValue(consoleIns).ToString();
                return activeText;
            }

            return null;
        }

        [UnityEditor.Callbacks.OnOpenAsset()]
        private static bool OnOpenAsset(int insId, int line)
        {
            if (opening) return false;
             
            UnityEngine.Object obj = EditorUtility.InstanceIDToObject(insId);
            var stackTrace = GetStackTrace();

            if (string.IsNullOrEmpty(stackTrace)) return false;

            if (obj.name == "LogUtil" && obj.GetType() == typeof(MonoScript))
            {
                var matches = System.Text.RegularExpressions.Regex.Match(stackTrace, @"\(at (.+)\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                var pathLine = "";
                while (matches.Success)
                {
                    pathLine = matches.Groups[1].Value;
                    if (!pathLine.Contains("/LogUtil.cs"))
                    {
                        var splitIndex = pathLine.LastIndexOf(":");
                        var path = pathLine.Substring(0, splitIndex);
                        line = Convert.ToInt32(pathLine.Substring(splitIndex + 1));
                        //UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(p, line);
                        opening = true;
                        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path), line);
                        opening = false;
                        return true;
                    }

                    matches = matches.NextMatch();
                }
            }

            return false;
        }
#endif
    } 
}


