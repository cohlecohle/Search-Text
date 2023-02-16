using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace lab3
{
    class CustomFile
    {
        StreamReader reader;
        public string Name { get; }
        public int Index { get; }

        public CustomFile(string fileName, int index)
        {
            Index = index;
            Name = fileName;
        }

        public void StartReader()
        {
            reader = new StreamReader(Name);
        } 

        public bool IsReaderEnded()
        {
            return reader.EndOfStream;
        }

        public bool IsReaderStarted()
        {
            return reader != null;
        }

        public string ReadFragment()
        {
            string fragment = "";

            while (fragment.Length <= 1000 && !reader.EndOfStream)
            {
                fragment += Convert.ToChar(reader.Read());
            }

            while (true)
            {
                var peek = reader.Peek();
                if (!((peek >= Convert.ToInt32('a') && peek <= Convert.ToInt32('z')) ||
                      (peek >= Convert.ToInt32('A') && peek <= Convert.ToInt32('Z'))))
                {
                    break;
                }
                fragment += Convert.ToChar(reader.Read());
            }

            reader.Read();

            return fragment;
        }
    }

    class ParallelWork
    {
        List<CustomFile> Files;
        string DataPath;
        int nThreads;
        string[] StopWords;
        Tree tree;
        List<Thread> Threads;
        object locker;

        public ParallelWork(string dataPath, Tree tree)
        {
            nThreads = 4;
            Threads = new List<Thread>(nThreads);

            DataPath = dataPath;

            Files = new List<CustomFile>();
            var fileNames = Directory.GetFiles(@"E:\labs\parallel\lab3\lab3\bin\Debug\netcoreapp3.1\" + DataPath);
            for(int i = 0; i < fileNames.Length; i++)
            {
                Files.Add(new CustomFile(fileNames[i], i));
            }

            this.tree = tree;

            string[] stopWords = {
                "myself", "our", "ours", "ourselves", "you",
                "your", "yours", "yourself", "yourselves",
                "him", "his", "himself", "she", "her", "hers",
                "herself", "its", "itself", "they",
                "them", "their", "theirs", "themselves", "what",
                "which", "who", "whom", "this", "that",
                "these", "those", "are", "was", "were", "been",
                "being", "have", "has", "had", "having",
                "does", "did", "doing", "the", "and", "but",
                "because", "until", "while", "for", "with",
                "about", "against", "between", "into", "through",
                "during", "before", "after", "above", "below",
                "from", "down", "out", "off", "over", "under",
                "again", "further", "then", "once", "here",
                "there", "when", "where", "why", "how", "all",
                "any", "both", "each", "few", "more", "most",
                "other", "some", "such", "nor", "not", "only",
                "own", "same", "than", "too", "very", "can",
                "will", "just", "don", "should", "now"
            };
            StopWords = stopWords;

            locker = new Object();
        }

        public void CreateTree()
        {
            foreach(var file in Files)
            {
                while (true)
                {
                    if (!file.IsReaderStarted())
                    {
                        file.StartReader();
                    }

                    if (file.IsReaderEnded())
                    {
                        break;
                    }

                    for (int i = 0; i < nThreads; i++)
                    {
                        var fragment = file.ReadFragment();
                        var th = new Thread(() => ProcFragment(ref fragment, file.Index));
                        Threads.Add(th);
                        th.Start();
                    }
                    foreach (var th in Threads)
                    {
                        while (th.IsAlive) ;
                    }
                    Threads.Clear();
                }
            }
        }

        void ThreadProc()
        {
            foreach(var file in Files)
            {
                int Row = 1, Column = 1;
                string fragment = "";

                while(true)
                {

                    lock(locker)
                    {
                        fragment = file.ReadFragment();
                    }

                    ProcFragment(ref fragment, file.Index);
                }
            }
        }

        string CleanWord(string word)
        {
            word = word.ToLower();
            word = System.Text.RegularExpressions.Regex.Replace(word, @"[^a-z]", "");
            return word;
        }

        void ProcFragment(ref string fragment, int fileIndex)
        {
            int Row = 1;
            int Column = 1;

            //if(fragment.Length == 0)
            //{
            //    return;
            //}
            var wordReader = new StringReader(fragment);

            while (wordReader.Peek() != -1)
            {
                string word;
                int dif;

                word = ReadWord(wordReader, ref Row, ref Column);
                dif = word.Length;
                word = CleanWord(word);
                if (!(word.Length <= 2 || Array.Exists(StopWords, element => element == word)))
                {
                    lock (locker)
                    {
                        tree.InsertWord(word, fileIndex, Row, Column);
                    }
                }

                Column += (dif + 1);
            }

            wordReader.Close();
        }

        string ReadWord(StringReader reader, ref int row, ref int col)
        {
            string word = "";

            while (true)
            {
                var peek = reader.Peek();

                if (peek == Convert.ToInt32('\n'))
                {
                    row++;
                    col = 0;
                    break;
                }

                if (!((peek >= Convert.ToInt32('a') && peek <= Convert.ToInt32('z')) ||
                      (peek >= Convert.ToInt32('A') && peek <= Convert.ToInt32('Z'))))
                {
                    break;
                }

                word += Convert.ToChar(reader.Read());
            }
            reader.Read();

            return word;
        }

        public ref List<CustomFile> GetFiles()
        {
            return ref Files;
        }
    }
}
