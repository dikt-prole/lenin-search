using System;
using System.IO;
using StackExchange.Redis;

namespace LeninSearch.RedisRefresh
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter redis connection string:");

            var connectionString = Console.ReadLine();

            Console.WriteLine("Enter lsi path:");

            var lsiPath = Console.ReadLine();

            Console.WriteLine("Enter main.json path:");

            var mainJsonPath = Console.ReadLine();

            var redis = ConnectionMultiplexer.Connect(connectionString);

            var db = redis.GetDatabase(1);

            Console.WriteLine("Connected to redis");

            foreach (var path in Directory.GetFiles(lsiPath))
            {
                if (path.EndsWith(".dic"))
                {
                    PutStringToRedis(db, path);
                }
                else
                {
                    PutBytesToRedis(db, path);
                }
            }

            PutBytesToRedis(db, mainJsonPath);
        }

        private static void PutBytesToRedis(IDatabase db, string path)
        {
            var bytes = File.ReadAllBytes(path);

            Console.WriteLine(path);

            db.StringSet(Path.GetFileName(path), bytes);
        }

        private static void PutStringToRedis(IDatabase db, string path)
        {
            var text = File.ReadAllText(path);

            Console.WriteLine(path);

            db.StringSet(Path.GetFileName(path), text);
        }
    }
}
