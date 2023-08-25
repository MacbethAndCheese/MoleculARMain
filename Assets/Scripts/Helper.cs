using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Helper
{
    public static string GetUntilOrEmpty(this string text, string stopAt = "-")
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            int charLocation = text.IndexOf(stopAt, System.StringComparison.Ordinal);

            if (charLocation > 0)
            {
                return text.Substring(0, charLocation);
            }
        }

        return string.Empty;
    }
}
