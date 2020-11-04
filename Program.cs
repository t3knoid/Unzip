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
                string filter = String.Empty;

                try
                {

                    if (parser.Arguments.ContainsKey("help") || parser.Arguments.ContainsKey("h"))
                    {
                        usage();
                        System.Environment.Exit(1);
                    }

                    // User must specify a fully qualified path
                    if (parser.Arguments.ContainsKey("file") || parser.Arguments.ContainsKey("f"))
                    {
                        filePath = parser.Arguments["file"][0];
                        // Check if file exist
                        if (!File.Exists(filePath))
                        {
                            Console.WriteLine("Cannot find " + filePath + "Exiting");
                            usage();
                            System.Environment.Exit(1);
                        }
                    }

                    if (parser.Arguments.ContainsKey("destination") || parser.Arguments.ContainsKey("d"))
                    {
                        destination = parser.Arguments["destination"][0];
                        // Check if directory exist
                        if (!Directory.Exists(destination))
                        {
                            Console.WriteLine("Cannot find " + destination + "Exiting");
                            usage();
                            System.Environment.Exit(1);
                        }
                    }

                    if (parser.Arguments.ContainsKey("filter") || parser.Arguments.ContainsKey("fi"))
                    {
                        filter = parser.Arguments["filter"][0];
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error getting arguments. " + ex.Message);
                    usage();
                    System.Environment.Exit(1);
                }

                if (!String.IsNullOrEmpty(filter)) // if a filter is specified
                {
                    try
                    {
                        using (ZipArchive archive = ZipFile.OpenRead(filePath))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                if (entry.FullName.EndsWith(filter, StringComparison.OrdinalIgnoreCase))
                                {
                                    // Gets the full path to ensure that relative segments are removed.
                                    string destinationPath = Path.GetFullPath(Path.Combine(destination, entry.Name));

                                    // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                                    // are case-insensitive.
                                    if (destinationPath.StartsWith(destination, StringComparison.Ordinal))
                                        entry.ExtractToFile(destinationPath);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to extract " + filePath + " to " + destination + "using a filter " + filter + ". " + ex.Message);
                    }
                }
                else
                {
                    try
                    {
                        using (ZipArchive archive = ZipFile.Open(filePath, ZipArchiveMode.Read))
                        {
                            archive.ExtractToDirectory(destination);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to extract " + filePath + " to " + destination + ". " + ex.Message);
                    }
                }
            }
        }
        private static void usage()
        {
            Console.WriteLine("usage: unzip -file zipfile -destination folder [-filter filter]\n\n");
            Console.WriteLine("       -help or -h       Shows this usage information");
            Console.WriteLine("       -file [-f] zipfile     Specifies the zip file to extract");
            Console.WriteLine("       -destination [-d]      Specifies the folder to extract to");
            Console.WriteLine("       -filter [-fi]          Specifies an optional filename filter to extract a specific file\n");
            Console.WriteLine("Examples:\n");
            Console.WriteLine("unzip -file foo.zip -destination c:\\temp                    Extracts all contents of foo.zip into c:\\temp");
            Console.WriteLine("unzip -file foo.zip -destination c:\\temp -filter myfoo.exe  Extracts any file named my foo.exe"  );
        }
    }
}