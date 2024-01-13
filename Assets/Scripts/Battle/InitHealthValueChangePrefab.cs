using System.ComponentModel;
using System.Globalization;
using System;
using UnityEngine;
using UnityEngine.UI;
using Color = System.Drawing.Color;
using System.Collections;
using System.Drawing;

/// <summary>
/// 挂在生命值变动的图标上
/// </summary>
public class InitHealthValueChangePrefab : MonoBehaviour
{
    public Text valueText;

    public void Init(string value, string color)
    {
        valueText.text = value;
        Color color1 = FromHtml(color);
        valueText.color = new(color1.R, color1.G, color1.B);
    }

    private static Hashtable htmlSysColorTable;

    public static Color FromHtml(string htmlColor)
    {
        Color result = Color.Empty;
        if (htmlColor == null || htmlColor.Length == 0)
        {
            return result;
        }

        if (htmlColor[0] == '#' && (htmlColor.Length == 7 || htmlColor.Length == 4))
        {
            if (htmlColor.Length == 7)
            {
                result = Color.FromArgb(Convert.ToInt32(htmlColor.Substring(1, 2), 16), Convert.ToInt32(htmlColor.Substring(3, 2), 16), Convert.ToInt32(htmlColor.Substring(5, 2), 16));
            }
            else
            {
                string text = char.ToString(htmlColor[1]);
                string text2 = char.ToString(htmlColor[2]);
                string text3 = char.ToString(htmlColor[3]);
                result = Color.FromArgb(Convert.ToInt32(text + text, 16), Convert.ToInt32(text2 + text2, 16), Convert.ToInt32(text3 + text3, 16));
            }
        }

        if (result.IsEmpty && string.Equals(htmlColor, "LightGrey", StringComparison.OrdinalIgnoreCase))
        {
            result = Color.LightGray;
        }

        if (result.IsEmpty)
        {
            if (htmlSysColorTable == null)
            {
                InitializeHtmlSysColorTable();
            }

            object obj = htmlSysColorTable[htmlColor.ToLower(CultureInfo.InvariantCulture)];
            if (obj != null)
            {
                result = (Color)obj;
            }
        }

        if (result.IsEmpty)
        {
            result = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(htmlColor);
        }

        return result;
    }

    private static void InitializeHtmlSysColorTable()
    {
        htmlSysColorTable = new Hashtable(26);
        htmlSysColorTable["activeborder"] = Color.FromKnownColor(KnownColor.ActiveBorder);
        htmlSysColorTable["activecaption"] = Color.FromKnownColor(KnownColor.ActiveCaption);
        htmlSysColorTable["appworkspace"] = Color.FromKnownColor(KnownColor.AppWorkspace);
        htmlSysColorTable["background"] = Color.FromKnownColor(KnownColor.Desktop);
        htmlSysColorTable["buttonface"] = Color.FromKnownColor(KnownColor.Control);
        htmlSysColorTable["buttonhighlight"] = Color.FromKnownColor(KnownColor.ControlLightLight);
        htmlSysColorTable["buttonshadow"] = Color.FromKnownColor(KnownColor.ControlDark);
        htmlSysColorTable["buttontext"] = Color.FromKnownColor(KnownColor.ControlText);
        htmlSysColorTable["captiontext"] = Color.FromKnownColor(KnownColor.ActiveCaptionText);
        htmlSysColorTable["graytext"] = Color.FromKnownColor(KnownColor.GrayText);
        htmlSysColorTable["highlight"] = Color.FromKnownColor(KnownColor.Highlight);
        htmlSysColorTable["highlighttext"] = Color.FromKnownColor(KnownColor.HighlightText);
        htmlSysColorTable["inactiveborder"] = Color.FromKnownColor(KnownColor.InactiveBorder);
        htmlSysColorTable["inactivecaption"] = Color.FromKnownColor(KnownColor.InactiveCaption);
        htmlSysColorTable["inactivecaptiontext"] = Color.FromKnownColor(KnownColor.InactiveCaptionText);
        htmlSysColorTable["infobackground"] = Color.FromKnownColor(KnownColor.Info);
        htmlSysColorTable["infotext"] = Color.FromKnownColor(KnownColor.InfoText);
        htmlSysColorTable["menu"] = Color.FromKnownColor(KnownColor.Menu);
        htmlSysColorTable["menutext"] = Color.FromKnownColor(KnownColor.MenuText);
        htmlSysColorTable["scrollbar"] = Color.FromKnownColor(KnownColor.ScrollBar);
        htmlSysColorTable["threeddarkshadow"] = Color.FromKnownColor(KnownColor.ControlDarkDark);
        htmlSysColorTable["threedface"] = Color.FromKnownColor(KnownColor.Control);
        htmlSysColorTable["threedhighlight"] = Color.FromKnownColor(KnownColor.ControlLight);
        htmlSysColorTable["threedlightshadow"] = Color.FromKnownColor(KnownColor.ControlLightLight);
        htmlSysColorTable["window"] = Color.FromKnownColor(KnownColor.Window);
        htmlSysColorTable["windowframe"] = Color.FromKnownColor(KnownColor.WindowFrame);
        htmlSysColorTable["windowtext"] = Color.FromKnownColor(KnownColor.WindowText);
    }
}
