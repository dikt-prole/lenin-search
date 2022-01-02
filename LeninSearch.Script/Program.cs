using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using LeninSearch.Script.Scripts;

namespace LeninSearch.Script
{
    class Program
    {
        static void Main(string[] args)
        {
            var type = typeof(IScript);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.FullName.Contains("IScript"))
                .ToList();
            var scripts = new List<IScript>();
            foreach (var t in types)
            {
                scripts.Add(t.Assembly.CreateInstance(t.FullName) as IScript);
            }

            while (true)
            {
                for (var i = 0; i < scripts.Count; i++)
                {
                    Console.WriteLine($"{i}. {scripts[i].Id}");
                }

                var scriptIndex = -1;
                while (!int.TryParse(Console.ReadLine(), out scriptIndex) || scripts.Count - 1 < scriptIndex)
                {
                    Console.WriteLine("Invalid script index");
                }

                var script = scripts[scriptIndex];

                Console.WriteLine($"Enter script arguments ({script.Arguments}) or empty line to continue");

                var input = new List<string>();
                while (true)
                {
                    var arg = Console.ReadLine();
                    if (string.IsNullOrEmpty(arg)) break;
                    input.Add(arg);
                }

                Console.OutputEncoding = Encoding.UTF8;
                Console.WriteLine($"Started script '{script.Id}' at {DateTime.Now}");

                try
                {
                    script.Execute(input.ToArray());
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                }

                Console.WriteLine($"Script '{script.Id}' done at {DateTime.Now}");
                Console.ReadLine();
            }
        }
    }
}
