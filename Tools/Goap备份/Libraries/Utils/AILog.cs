using System.Text;
using UnityEngine;

/// <summary>
///	AI Log
/// </summary>
public static class AILog
{
    #region Public Methods

    public static void Warning(params object[] aArgs)
    {
        Debug.LogWarningFormat("AI Warning: {0}",Message(aArgs));
    }

    public static void Assert(bool aCondition, string aMessage, params object[] aArgs)
    {
        if (aCondition)
        {
            var args = new object[aArgs.Length + 1];
            args[0] = string.Concat("AI Assert Failed! ", aMessage);
            for (int i = 0, n = aArgs.Length; i < n; i++)
            {
                args[i + 1] = aArgs[i];
            }
            string str = Message(args);
            Debug.Assert(!aCondition, str);
            Debug.Break();
        }
    }

    #endregion

    #region Private Methods

    private static string Message(params object[] aArgs)
    {
        string result = null;
        if (aArgs[0] is string && CountOfBrackets((string)aArgs[0]) == aArgs.Length - 1)
        {
            var args = new object[aArgs.Length - 1];
            for (int i = 1, n = aArgs.Length; i < n; i++)
            {
                args[i - 1] = aArgs[i];
            }
            result = string.Format((string)aArgs[0], args);
        }

        if (result == null)
        {
            var sb = new StringBuilder();
            for (int i = 0, n = aArgs.Length; i < n; i++)
            {
                if (aArgs[i] != null)
                {
                    sb.Append(aArgs[i].ToString());
                }
                else
                {
                    sb.Append("Null");
                }
                sb.Append(" ");
            }
            result = sb.ToString();
        }

        return result;
    }

    private static int CountOfBrackets(string aStr)
    {
        int count = 0;
        bool opened = false;
        for (int i = 0, n = aStr.Length; i < n; i++)
        {
            switch (aStr[i])
            {
                case '{':
                    opened = true;
                    break;

                case '}':
                    if (opened)
                    {
                        count++;
                        opened = false;
                    }
                    break;
            }
        }

        return count;
    }

    #endregion
}
