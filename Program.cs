using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShellProgressBar;

namespace MailIndexer
{
    class Program
    {
        static IEnumerable<FileInfo> Crawl(DirectoryInfo dir){
            foreach (var file in dir.EnumerateFiles()){
                yield return file;
            }
            
            foreach (var d in dir.EnumerateDirectories()){
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
            using(var progresss= new ProgressBar(files.Count,"Reading files")){
                foreach (var file in files)
                {
                    var text = file.OpenText().ReadToEnd();
                    progresss.Tick();
                }
            }
        }
    }
}
