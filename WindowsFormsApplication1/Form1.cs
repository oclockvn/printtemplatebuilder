using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        List<string> templates = new List<string>();


        public Form1()
        {
            InitializeComponent();

            /*
            templates.Add("datetime: <*>[r]" + "fuck" + "[/r]</*>");
            templates.Add("");
            templates.Add("name: <*>phan tien quang</*>. age: 24. <*>[r]developer[/r]</*>");
            templates.Add("---");
            templates.Add("");
            templates.Add("email:\t\t[r]oclockvn@gmail.com[/r]");
            templates.Add("skype: <*>[r]oclockvn[/r]</*>");
            templates.Add("address: <*>90/5</*> yen the - w2 - tan binh district");
             * */
            using (var reader = new StreamReader("template.txt"))
            {
                while (!reader.EndOfStream)
                {
                    templates.Add(reader.ReadLine());
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var f = new Font("Arial", 10);
            var f2 = new Font("Arial", 16);
            var h = f.GetHeight();
            var h2 = f2.GetHeight();
            var dh = h2 - h;
            var b = Brushes.Black;
            float o = 0;
            float line = 0;
            var p = new Pen(b);
            var pageW = 400;
            var pageH = 400;
            // Rectangle r = new Rectangle((float)0, (float)0, (float)pageW, (float)h2);

            g.DrawRectangle(p, 0, 0, 400, 400);
            
            RectangleF rf = new RectangleF(0, 0, pageW, h2);
            StringFormat sf = new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Far };

            // var pattern = "<l>(.*?)</l>";
            var pattern = "<\\*>(.*?)</\\*>";
            var alignmentPattern = "\\[r\\](.*)\\[/r\\]";

            //g.DrawString(DateTime.Now.ToString(), f, b, rf, sf);
            //g.DrawRectangle(p, (float)0, (float)0, (float)pageW, h2);

            //line = 2*h2;
            foreach (var s in templates)
            {
                var parts = new List<PrintPart>();

                /*
                 * first of all, split this line of text into multiple part
                 * so we can know wheather it's a large or normal text
                 * 
                 * how do we know? yes, look at the pattern above
                 * so, if a text has pattern <l>large me</l>, the text "large me" will be used large font
                 * 
                 * */
                var arr = Regex.Split(s, pattern);
                if (arr != null && arr.Length > 0)
                {
                    foreach (var ar in arr)
                    {
                        // check this text is right align or not
                        var isRight = Regex.IsMatch(ar, alignmentPattern);
                        var ar_none = ar.Replace("[r]", string.Empty).Replace("[/r]", string.Empty);
                        var s_none = s.Replace("[r]", string.Empty).Replace("[/r]", string.Empty);

                        // check this text is large or not
                        var isLarge = Regex.IsMatch(s_none, "<\\*>" + ar_none + "</\\*>");
                        
                        var text = ar;
                        if (isRight)
                        {
                            text = Regex.Match(ar, alignmentPattern).Groups[1].Value;
                            parts.Add(new PrintPart() { Text = text, IsLarge = isLarge, RightAlign = isRight });
                        }
                        else
                        {
                            parts.Add(new PrintPart() { Text = text, IsLarge = isLarge, RightAlign = isRight });
                        }
                    }
                }

                /*
                 * this is interesting
                 * 
                 * we had parts of text after split above
                 * 
                 * */                
                if (parts.Count > 0)
                {
                    // size for next part to render
                    // SizeF size = new SizeF(0, 0);
                    float width = 0;
                    foreach (var part in parts)
                    {
                        var dy = part.IsLarge ? line : (line + dh / 2);
                        var fn = part.IsLarge ? f2 : f;
                        var format = new StringFormat() { Alignment = StringAlignment.Near };

                        // measure size for next text
                        var size = g.MeasureString(part.Text, fn);

                        if (part.RightAlign)
                        {
                            RectangleF layout = new RectangleF(0, dy, 400, h2);
                            g.DrawString(part.Text, fn, b, layout, new StringFormat() { Alignment = StringAlignment.Far });
                        }
                        else
                        {
                            g.DrawString(part.Text, fn, b, width, dy, format);
                        }

                        //g.DrawRectangle(p, width, dy, size.Width, size.Height);
                        width += size.Width;                        
                    }
                }
                // line += hasLarge ? h2 : h;
                line += h2;
            }

            base.OnPaint(e);
        }
    }

    public enum PrintAlignments
    {
        Left,
        Center, 
        Right
    }

    public class PrintPart
    {
        public string Text { get; set; }
        public bool IsLarge { get; set; }
        public bool RightAlign { get; set; }
        public bool Center { get; set; }
    }
}
