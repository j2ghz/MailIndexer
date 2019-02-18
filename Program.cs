﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShellProgressBar;

namespace MailIndexer
{
    class Program
    {
        private struct TermLocation
        {
            public string Path;
            public int LineNumber;
        }
        static IEnumerable<FileInfo> Crawl(DirectoryInfo dir)
        {
            foreach (var file in dir.EnumerateFiles())
            {
                yield return file;
            }

            foreach (var d in dir.EnumerateDirectories())
            {
                foreach (var file in Crawl(d))
                {
                    yield return file;
                }
            }
        }
        static void Main(string[] args)
        {
            var root = new DirectoryInfo("/home/jghz/maildir/");
            var files = Crawl(root).ToList();
            var index = new SortedDictionary<string, List<TermLocation>>();
            using (var progresss = new ProgressBar(files.Count, "Reading files"))
            {
                foreach (var file in files)
                {

                    using (var sr = file.OpenText())
                    {
                        var lineNumber = 0;
                        while (!sr.EndOfStream)
                        {
                            var loc = new TermLocation() { Path = file.FullName, LineNumber = lineNumber++ };
                            var terms = sr.ReadLine().Split(' ');
                            foreach (var term in terms)
                            {
                                if (index.ContainsKey(term))
                                {
                                    index[term].Add(loc);
                                }
                                else
                                {
                                    index.Add(term, new List<TermLocation>() { loc });
                                }
                            }
                        }
                    }

                    progresss.Tick();
                }
            }
            Console.WriteLine(index.Count);
            while (true)
            {
                var search = Console.ReadLine();
                if (index.TryGetValue(search, out var locs))
                {
                    foreach (var loc in locs)
                    {
                        Console.WriteLine($"{loc.Path}@{loc.LineNumber}");
                    }
                }
                else
                {
                    foreach (var (term, locS) in index)
                    {
                        if (term.Contains(search))
                        {
                            foreach (var loc in locS)
                            {
                                Console.WriteLine($"{loc.Path}@{loc.LineNumber}");
                            }
                        }
                    }
                }
            }
        }
    }
}
