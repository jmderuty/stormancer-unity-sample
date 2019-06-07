using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stormancer
{

    public static class UnityLogColor
    {
        public static UnityEngine.Color MessageReceived = UnityEngine.Color.cyan;
        public static UnityEngine.Color MessageSent = new UnityEngine.Color(0.4f, 1.0f, 0.6f);

        public static void Log(string textToLog, UnityEngine.Color color)
        {
            string txt = "<color=#" + UnityEngine.ColorUtility.ToHtmlStringRGB(color) + ">" + textToLog + "</color>";
            UnityEngine.Debug.Log(txt);
        }

        public static void Log(string textToLog)
        {
            Log(textToLog, UnityEngine.Color.white);
        }

        public static void LogError(string textToLog, UnityEngine.Color color)
        {
            string txt = "<color=#" + UnityEngine.ColorUtility.ToHtmlStringRGB(color) + ">" + textToLog + "</color>";
            UnityEngine.Debug.LogError(txt);
        }

        public static void LogError(string textToLog)
        {
            LogError(textToLog, UnityEngine.Color.red);
        }

        public static void LogWarning(string textToLog, UnityEngine.Color color)
        {
            string txt = "<color=#" + UnityEngine.ColorUtility.ToHtmlStringRGB(color) + ">" + textToLog + "</color>";
            UnityEngine.Debug.LogWarning(txt);
        }

        public static void LogWarning(string textToLog)
        {
            LogWarning(textToLog, new UnityEngine.Color(1F, 0.65F, 0.0F, 1F));
        }
    }

}