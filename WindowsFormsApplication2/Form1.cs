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

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        List<string> templates = new List<string>();
        public Form1()
        {
            InitializeComponent();

            using (var reader = new StreamReader("template.txt"))
            {
                while (!reader.EndOfStream)
                {
                    templates.Add(reader.ReadLine());
                }
            }
        }

        /// <summary>
        /// what do we do here?
        /// 
        /// well, parse template into printpart
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var font = new Font("Arial", 10);
            var font2 = new Font("Arial", 16);
            var lineHight = font2.GetHeight(g);

            var brush = Brushes.Black;
            var pen = new Pen(brush);
            float vertial = 0;


            List<PrintPart> parts = new List<PrintPart>();
            foreach (var template in templates)
            {
                if (template.Contains("(T.4)"))
                {

                }
                var tparts = TemplateParsing(template);
                if (tparts != null && tparts.Count > 0)
                //parts.AddRange(tparts);
                {
                    float width = 0;
                    float height = 0;
                    foreach (var part in tparts)
                    {
                        var fn = part.IsLarge ? font2 : font;
                        var format = new StringFormat()
                        {
                            Alignment = part.IsCenter ? StringAlignment.Center : StringAlignment.Near,
                            Trimming = StringTrimming.EllipsisPath,
                            LineAlignment = StringAlignment.Center
                        };

                        var size = g.MeasureString(part.Text, fn);
                        var rech = Math.Round(Math.Max(size.Width, 400) * 1.0 / 400);
                        if (rech > 1)
                        {

                        }
                        height = (float)rech * lineHight + 2;
                        // height = lineHight;
                        // RectangleF rf = new RectangleF(0, vertial, part.IsCenter ? 400 : size.Width, height);
                        RectangleF rf = new RectangleF(0, vertial, 400, height);

                        if (part.IsCenter)
                        {
                            g.DrawString(part.Text, fn, brush, rf, format);
                        }
                        else if (part.IsRight)
                        {
                            format.Alignment = StringAlignment.Far;
                            g.DrawString(part.Text, fn, brush, rf, format);
                        }
                        else
                        {
                            //RectangleF rf2 = new RectangleF(width, vertial, size.Width, height);
                            //format.LineAlignment = StringAlignment.Center;
                            //format.Alignment = StringAlignment.Near;
                            // g.DrawString(part.Text, fn, brush, rf2, format);
                            // rf.Width = size.Width;
                            RectangleF rf2 = new RectangleF(width, vertial, 400, height);
                            g.DrawString(part.Text, fn, brush, rf2, format);
                        }
                        //g.DrawRectangle(pen, width, vertial, Math.Min(size.Width,400), height);
                        Pen pp = new Pen(Color.FromArgb(20, 0, 0, 0));
                        g.DrawLine(pp, 0, vertial, 400, vertial);

                        width += size.Width;
                    }
                    vertial += height;
                    // vertial += vertial;
                }
            }

            base.OnPaint(e);
        }

        private string templateCenter = "\\[c\\](.*)\\[/c\\]";
        private string templateRight = "\\[r\\](.*)\\[/r\\]";
        private string templateLarge = "\\[\\*\\](.*)\\[/\\*\\]";

        public List<PrintPart> TemplateParsing(string template)
        {
            var parts = new List<PrintPart>();

            if (Regex.IsMatch(template, templateCenter))
            {
                var text = Regex.Match(template, templateCenter).Groups[1].Value;
                var isLarge = Regex.IsMatch(template, templateLarge);
                if (isLarge)
                {
                    text = Regex.Match(text, templateLarge).Groups[1].Value;
                }
                var part = new PrintPart() { Text = text, IsCenter = true, IsLarge = isLarge };
                parts.Add(part);
            }
            else
            {
                var rights = Regex.Split(template, templateRight);
                if (rights.Length > 0)
                {
                    foreach (var right in rights)
                    {
                        var template2 = template.Replace("[*]", string.Empty).Replace("[/*]", string.Empty);
                        var right2 = right.Replace("[*]", string.Empty).Replace("[/*]", string.Empty);
                        // var hasRight = Regex.IsMatch(template2, "\\[r\\]" + right2 + "\\[/r\\]");
                        var hasRight = template2.Contains("[r]" + right2 + "[/r]");
                        var tparts = LargeParsing(right, hasRight);
                        if (tparts != null && tparts.Count > 0)
                            parts.AddRange(tparts);
                    }
                }
            }

            return parts;
        }

        public List<PrintPart> LargeParsing(string template, bool right)
        {
            if (string.IsNullOrEmpty(template))
                return new List<PrintPart>() { new PrintPart() { Text = "" } };

            var larges = Regex.Split(template, templateLarge);
            if (larges == null || larges.Length == 0)
                return null;

            var parts = new List<PrintPart>();
            foreach (var large in larges)
            {
                // 
                // var isLarge = Regex.IsMatch(template, "\\[\\*\\]" + large + "\\[/\\*\\]");
                var isLarge = template.Contains("[*]" + large + "[/*]");
                var part = new PrintPart() { Text = large, IsLarge = isLarge, IsCenter = false, IsRight = right };
                parts.Add(part);
            }


            return parts;
        }
    }

    public class PrintPart
    {
        public string Text { get; set; }
        public bool IsLarge { get; set; }
        public bool IsCenter { get; set; }
        public bool IsRight { get; set; }
    }

}
