using Stormancer.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormancer
{

    public class NullLogger : ILogger
    {

        public static NullLogger Instance = new NullLogger();

		public void Log(Stormancer.Diagnostics.LogLevel logLevel, string category, string message, object context = null)
		{
			
		}

		public void Trace(string message, params object[] p)
        {

        }

        public void Error(Exception ex)
        {

        }

        public void Error(string format, params object[] p)
        {

        }

        public void Info(string format, params object[] p)
        {

        }


        public void Debug(string message, params object[] p)
        {

        }
    }

    public class DebugLogger : ILogger
    {
        private DebugLogger() { }

        public static readonly DebugLogger Instance = new DebugLogger();

        public void Log(LogLevel logLevel, string category, string message, object context = null) {
            switch(logLevel) {
                case LogLevel.Fatal:
                case LogLevel.Error:
                    UnityEngine.Debug.LogError(logLevel.ToString() + ": " + category + ": " + message + ": " + context?.ToString());
                    break;
                case LogLevel.Warn:
                    UnityEngine.Debug.LogWarning(logLevel.ToString() + ": " + category + ": " + message + ": " + context?.ToString());
                    break;
                default:
                    UnityEngine.Debug.Log(logLevel.ToString() + ": " + category + ": " + message + ": " + context?.ToString());
                    break;
            }
        }

        public void Trace(string message, params object[] p)
        {
            Log(LogLevel.Trace, "client", string.Format(message, p));
        }

        public void Debug(string message, params object[] p)
        {
			Log(LogLevel.Debug, "client", string.Format(message, p));
        }

        public void Error(Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
        }

        public void Error(string format, params object[] p)
        {
			Log(LogLevel.Error, "client", string.Format(format, p));
        }

        public void Info(string format, params object[] p)
        {
			Log(LogLevel.Info, "client", string.Format(format, p));
        }
    }

}
