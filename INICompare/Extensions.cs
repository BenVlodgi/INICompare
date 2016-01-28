using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Documents;

namespace INICompare
{
    public static class Extensions
    {
        public static void AppendLine(this Paragraph paragraph, string value = "", Brush background = null, Brush foreground = null, bool bold = false, bool italic = false, bool underline = false)
        {
            paragraph.Append(value + NewLine, background, foreground, bold, italic, underline);
        }

        //private const string NewLine = "\u2028";
        private const string NewLine = "\r\n";

        public static void Append(this Paragraph paragraph, string value = "", Brush background = null, Brush foreground = null, bool bold = false, bool italic = false, bool underline = false)
        {
            Inline run = new Run(value);
            if (background != null) run.Background = background;
            if (foreground != null) run.Foreground = foreground;
            if (bold) run = new Bold(run);
            if (italic) run = new Italic(run);
            if (underline) run = new Underline(run);
            paragraph.Inlines.Add(run);
        }

    }
}
