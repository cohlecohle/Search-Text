using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace lab3
{
    class Program
    {
        static string KeyWord;
        static StreamWriter writer;
        static Tree tree;
        static ParallelWork parallelWork;

        static void Main(string[] args)
        {
            var DataPath = ReadDataPath();

            tree = new Tree();

            parallelWork = new ParallelWork(DataPath, tree);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            parallelWork.CreateTree();

            stopwatch.Stop();
            ShowTime(stopwatch);

            SearchLoop();
        }

        static void ShowTime(Stopwatch stopwatch)
        {
            var ts = stopwatch.Elapsed;
            Console.WriteLine(
                "{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10
            );
        }

        static void SearchLoop()
        {
            while (true)
            {
                Console.Write("Key word: ");
                KeyWord = Console.ReadLine();
                KeyWord = KeyWord.ToLower();

                if (KeyWord == "")
                {
                    Console.WriteLine("Invalid key word. Try again...");
                    continue;
                }

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var wordInfo = tree.GetInfo(KeyWord);

                writer = new StreamWriter("output.txt");

                foreach (var record in wordInfo)
                {
                    writer.WriteLine($"{parallelWork.GetFiles()[record.Item1].Name}; {record.Item2}; {record.Item3}");
                }

                writer.Close();

                stopwatch.Stop();
                ShowTime(stopwatch);

                Console.WriteLine("Instances found: " + wordInfo.Count);
            }
        }

        static string ReadDataPath()
        {
            Console.Write("Path to data: ");
            return Console.ReadLine();
        }
    }
}
