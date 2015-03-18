using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace DataRelational
{
    /// <summary>
    /// Provides a mechanism for logging
    /// </summary>
    public static class Logging
    {
        private static List<Action<String>> _LoggingActions = new List<Action<string>>();

        /// <summary>
        /// Adds an action to be invoked when a log is entered
        /// </summary>
        /// <param name="Action">The action to add</param>
        public static void AddLoggingAction(Action<string> Action)
        {
            _LoggingActions.Add(Action);
        }

        internal static void Debug(string log)
        {
            if (DataManager.Settings.Logging.Debug)
            {
                string previousMethod = GetPreviousMethodName(MethodBase.GetCurrentMethod());
                Write(string.Format("Debug: {0}, Method: {1}, {2}{3}", DateTime.Now, previousMethod, log, Environment.NewLine));
            }
        }

        internal static void Information(string log)
        {
            if (DataManager.Settings.Logging.Information)
            {
                string previousMethod = GetPreviousMethodName(MethodBase.GetCurrentMethod());
                Write(string.Format("Information: {0}, Method: {1}, {2}{3}", DateTime.Now, previousMethod, log, Environment.NewLine));
            }
        }

        internal static void Warning(string log)
        {
            if (DataManager.Settings.Logging.Warning)
            {
                string previousMethod = GetPreviousMethodName(MethodBase.GetCurrentMethod());
                Write(string.Format("Warning: {0}, Method: {1}, {2}{3}", DateTime.Now, previousMethod, log, Environment.NewLine));
            }
        }

        /// <summary>
        /// Creates a log line based on an exception
        /// </summary>
        /// <param name="ex"></param>
        public static void Exception(Exception ex)
        {
            if (DataManager.Settings.Logging.Exception)
            {
                if (!string.IsNullOrEmpty(ex.StackTrace))
                    Write(string.Format("Exception: {0}, {1}{2}{3}", DateTime.Now, ex.Message, Environment.NewLine, ex.StackTrace));
                else
                    Write(string.Format("Exception: {0}, {1}{2}", DateTime.Now, ex.Message, Environment.NewLine));
            }
        }

        private static void Write(string str)
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataRelational.log"), true))
            {
                writer.Write(str);
                writer.Close();
            }

            _LoggingActions.ForEach(a => a(str));
        }

        private static string GetPreviousMethodName(MethodBase currentMethod)
        {
            string methodName = string.Empty;

            try
            {
                StackTrace sTrace = new System.Diagnostics.StackTrace(true);

                for (Int32 frameCount = 0; frameCount < sTrace.FrameCount; frameCount++)
                {
                    StackFrame sFrame = sTrace.GetFrame(frameCount);
                    System.Reflection.MethodBase thisMethod = sFrame.GetMethod();

                    if (thisMethod == currentMethod)
                    {
                        if (frameCount + 1 <= sTrace.FrameCount)
                        {
                            StackFrame prevFrame = sTrace.GetFrame(frameCount + 1);
                            System.Reflection.MethodBase prevMethod = prevFrame.GetMethod();
                            methodName = prevMethod.ReflectedType + "." + prevMethod.Name;
                        }
                        break;
                    }
                }
            }

            catch (Exception)
            {
                return string.Empty;
            }

            return methodName;
        }
    }
}
