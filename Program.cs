using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using pckkey.Core;
using pckkey.Core.ArchiveEngine;
using pckkey.Interfaces;
using static pckkey.Core.Events;

namespace pckkey
{
    internal class Program
    {
        public static List<ArchiveKey> Keys = new List<ArchiveKey>();
        private static readonly object lockObject = new object();
        private static Dictionary<string, int> fileCursorPositions = new Dictionary<string, int>();
        private static int initialCursorTop;

        static void Main(string[] args)
        {
            Console.WriteLine("PCK Key Changer - Haly");
            Console.WriteLine("GitHub: https://github.com/halysondev/");

            // Prompt the user to enter the old keys, with default values if not provided
            Console.WriteLine("Enter the old key 1 (leave empty for default -1466731422):");
            string oldKey1Input = Console.ReadLine();
            int oldKey1 = string.IsNullOrWhiteSpace(oldKey1Input) ? -1466731422 : int.Parse(oldKey1Input);

            Console.WriteLine("Enter the old key 2 (leave empty for default -240896429):");
            string oldKey2Input = Console.ReadLine();
            int oldKey2 = string.IsNullOrWhiteSpace(oldKey2Input) ? -240896429 : int.Parse(oldKey2Input);

            Keys.Add(new ArchiveKey
            {
                Name = "Perfect World",
                KEY_1 = oldKey1,
                KEY_2 = oldKey2,
                ASIG_1 = -33685778,
                ASIG_2 = -267534609,
                FSIG_1 = 1305093103,
                FSIG_2 = 1453361591
            });

            // Prompt the user to enter the directory where the .pck files are located
            Console.WriteLine("Enter the folder path where the .pck files are located (leave empty to use current directory):");
            string folderPath = Console.ReadLine();

            // Use current directory if no path is provided
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                folderPath = Directory.GetCurrentDirectory();
            }

            // Prompt the user to enter the new keys and compression level
            Console.WriteLine("Enter the new key 1:");
            string newKey1Input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newKey1Input))
            {
                Console.WriteLine("New key 1 is required. Exiting...");
                return;
            }

            int newKey1 = int.Parse(newKey1Input);

            Console.WriteLine("Enter the new key 2:");
            string newKey2Input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newKey2Input))
            {
                Console.WriteLine("New key 2 is required. Exiting...");
                return;
            }

            int newKey2 = int.Parse(newKey2Input);

            Console.WriteLine("Enter the compression level (1-9):");
            int compressionLevel = int.Parse(Console.ReadLine());

            // Get the initial cursor position
            initialCursorTop = Console.CursorTop;

            // Get the list of .pck files in the specified directory and subdirectories
            string[] pckFiles = Directory.GetFiles(folderPath, "*.pck", SearchOption.AllDirectories);

            // Process files in parallel
            Parallel.ForEach(pckFiles, (pckFile, state, index) =>
            {
                string fileName = Path.GetFileName(pckFile);
                int progressMax = 0;
                int progressCurrent = 0;
                int filePosition;

                lock (lockObject)
                {
                    // Calculate the cursor position for the file
                    filePosition = initialCursorTop + (int)index;
                    fileCursorPositions[fileName] = filePosition;
                    Console.SetCursorPosition(0, filePosition);
                    Console.WriteLine($"Processing file: {fileName}");
                }

                using (ArchiveManager archive = new ArchiveManager(pckFile, Keys[0]))
                {
                    // Subscribe to progress events
                    archive.SetProgressMax += (max) =>
                    {
                        progressMax = max;
                        UpdateProgress(fileName, progressCurrent, progressMax);
                    };
                    archive.SetProgress += (progress) =>
                    {
                        progressCurrent = progress;
                        UpdateProgress(fileName, progressCurrent, progressMax);
                    };
                    archive.SetProgressNext += () =>
                    {
                        progressCurrent++;
                        UpdateProgress(fileName, progressCurrent, progressMax);
                    };

                    //lock (lockObject)
                    //{
                        archive.ReadFileTable();
                    //}

                    archive.ChangeKeys(newKey1, newKey2, compressionLevel);

                    archive.Dispose();
                }
            });

            Console.WriteLine("Processing completed.");
            Console.ReadLine();
        }

        static void UpdateProgress(string fileName, int progressCurrent, int progressMax)
        {
            lock (lockObject)
            {
                if (progressMax > 0 && progressCurrent > 0)
                {
                    int filePosition = fileCursorPositions[fileName];
                    // Move the cursor to the stored position for the file and update the progress
                    Console.SetCursorPosition(0, filePosition);
                    Console.WriteLine($"Progress for {fileName}: {progressCurrent}/{progressMax}");
                }
            }
        }
    }
}
