using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Unzip
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new CommandLine();
            parser.Parse(args);

            if (parser.Arguments.Count > 0)
            {
                string filePath = String.Empty;
                string destination = String.Empty;

                try
                {
                    // User must specify a fully qualified path
                    if (parser.Arguments.ContainsKey("file"))
                    {
                        filePath = parser.Arguments["file"][0];
                        // Check if file exist
                        if (!File.Exists(filePath))
                        {
                            Console.WriteLine("Cannot find" + filePath + "Exiting");
                            usage();
                            System.Environment.Exit(1);
                        }
                    }

                    if (parser.Arguments.ContainsKey("destination"))
                    {
                        destination = parser.Arguments["destination"][0];
                        // Check if directory exist
                        if (!Directory.Exists(destination))
                        {
                            Console.WriteLine("Cannot find" + destination + "Exiting");
                            usage();
                            System.Environment.Exit(1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error getting arguments. " + ex.Message);
                    usage();
                    System.Environment.Exit(1);
                }

                try
                {
                    using (ZipArchive archive = ZipFile.OpenRead(filePath))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            entry.ExtractToFile(Path.Combine(destination, entry.FullName));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to extract " + filePath + " to " + destination + ". " + ex.Message); 
                }

            }

        }

        private static void usage() => Console.WriteLine("unzip -file zipfile -destination folder");
    }
}