using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = "name: <l>phan tien quang</l>. age: <l>24</l>. <l>developer</l> asd";
            var parts = new List<PrintPart>();

            var arr = Regex.Split(s, "<l>(.*?)</l>");
            foreach (var ar in arr)
            {
                var isLarge = Regex.IsMatch(s, "<l>" + ar + "</l>");
                parts.Add(new PrintPart() { Text = ar, IsLarge = isLarge });
            }

            foreach (var part in parts)
            {
                Console.WriteLine("{0} is large: {1}", part.Text, part.IsLarge);
            }

            Console.ReadLine();
        }
    }

    public class PrintPart
    {
        public string Text { get; set; }
        public bool IsLarge { get; set; }
    }
}
