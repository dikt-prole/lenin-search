using System;
using System.Linq;
using System.Text;
using LeninSearch.Script.Scripts;

namespace LeninSearch.Script
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) return;

            var scriptId = args[0];

            var input = args.Skip(1).ToArray();

            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine($"Started script '{scriptId}' at {DateTime.Now}");

            var type = typeof(IScript);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.FullName.Contains("IScript"))
                .ToList();

            foreach (var t in types)
            {
                var script = t.Assembly.CreateInstance(t.FullName) as IScript;

                if (script.Id == scriptId)
                {
                    script.Execute(input);
                    break;
                }
            }

            Console.WriteLine($"Script '{scriptId}' done at {DateTime.Now}");
            Console.ReadLine();
        }
    }
}
