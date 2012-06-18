using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SolutionZip
{
    class SolutionZip
    {
        static void Main(string[] args)
        {
            string rootFolder = ".\\";
            string archiveName = "Archive.zip";

            if (args.Length > 0)
            {
                rootFolder = args[0];
            }
            if (args.Length > 1)
            {
                archiveName = args[1];
            }
            List<string> exceptions = new List<string>();
            exceptions.Add(@".user");
            exceptions.Add(@".suo");
            exceptions.Add(@"\bin");
            exceptions.Add(@"\obj");
            exceptions.Add(@"\packages");

            int filesAdded = CreateArchive(rootFolder, 
                exceptions, archiveName);
            Console.WriteLine(String.Format(" {0} file(s) added ", 
                filesAdded));
            Console.ReadLine();
        }

        public static int CreateArchive(string folder, 
                IList<string> exceptions, string archiveName)
        {
            int filesCount = 0;
            string folderFullPath = Path.GetFullPath(folder);
            string archivePath = Path.Combine(folderFullPath, archiveName);
            if (File.Exists(archivePath))
            {
                Console.WriteLine(
                    string.Format(@"File '{0}' already exists. Overwrite (y/n): ", 
                        archiveName));
                string read = Console.ReadLine();
                if (read.ToLower() == "y")
                {
                    File.Delete(archivePath);
                }
                else
                {
                    Console.WriteLine(string.Format(@"Archive {0} already exists. 
                        Aborting!", archivePath));
                    return 0;
                }
            }
            IEnumerable<string> files = Directory.EnumerateFiles(folder,
                    "*.*", SearchOption.AllDirectories);
            using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
            {                
                foreach (string file in files)
                {
                    if (!Excluded(file, exceptions))
                    {
                        try
                        {
                            var addFile = Path.GetFullPath(file);
                            if (addFile != archivePath)
                            {
                                addFile = addFile.Substring(folderFullPath.Length);
                                Console.WriteLine("Adding " + addFile);
                                archive.CreateEntryFromFile(file, addFile);
                                filesCount++;
                            }
                        }
                        catch (IOException ex)
                        {
                            Console.WriteLine(@"Failed to add {0} due to error : 
                            {1} \n Ignoring it!", file, ex.Message);
                        }
                    }
                }
            }
            return filesCount;
        }

        private static bool Excluded(string file, IList<string> exceptions)
        {
            List<String> folderNames = (from folder in exceptions
                                        where folder.StartsWith(@"\") 
                                            || folder.StartsWith(@"/")
                                        select folder).ToList<string>();
            if (!exceptions.Contains(Path.GetExtension(file)))
            {
                foreach (string folderException in folderNames)
                {
                    if(Path.GetDirectoryName(file).Contains(folderException))
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
    }
}
