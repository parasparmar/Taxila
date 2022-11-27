using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using FolderSize;

namespace Library
{
    internal class Reader
    {
        
        private static int _recursionLevel;

        private static int _directoriesProcessed = 0;

        private static DirectoryData _directoryData;
        public void GetAllFiles(string dirToRead)
        { 
            try
            {
                _directoryData = new DirectoryData(SortDirection.desc);

                DirectorySize(SortDirection.desc, new DirectoryInfo(dirToRead), _directoryData);

                Console.WriteLine(Environment.NewLine);

                PrintDirectoryData(2, _directoryData);

                PrintProgress();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        private static void PrintDirectoryData(int maxRecursionLevel, DirectoryData directoryData)
        {
            if (_recursionLevel < maxRecursionLevel)
            {
                _recursionLevel++;

                string output = null;

                for (int i = 0; i < _recursionLevel; i++)
                {
                    output += "|__";
                }

                double directorySizeMb = Math.Round(directoryData.SizeBytes / (double)(1024 * 1024), 2);

                output += $"{directoryData.Name} : {directorySizeMb} MB";

                Console.WriteLine(output);

                foreach (var subDirectoryData in directoryData.DirectoryDatas)
                {
                    PrintDirectoryData(maxRecursionLevel, subDirectoryData);
                }

                _recursionLevel--;
            }
        }

        private static long DirectorySize(SortDirection sortDirection, DirectoryInfo directoryInfo, DirectoryData directoryData)
        {
            long directorySizeBytes = 0;

            directoryData.Name = directoryInfo.Name;

            try
            {
                // Add file sizes for current directory

                FileInfo[] fileInfos = directoryInfo.GetFiles();

                foreach (FileInfo fileInfo in fileInfos)
                {
                    directorySizeBytes += fileInfo.Length;
                }

                directoryData.SizeBytes += directorySizeBytes;

                // Recursively add subdirectory sizes

                DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();

                foreach (DirectoryInfo di in subDirectories)
                {
                    var subDirectoryData = new DirectoryData(sortDirection);

                    directoryData.DirectoryDatas.Add(subDirectoryData);

                    directorySizeBytes += DirectorySize(sortDirection, di, subDirectoryData);
                }

                directoryData.SizeBytes = directorySizeBytes;
            }
            catch (UnauthorizedAccessException uaex)
            {
                directoryData.Name += " (Unable to calculate size - Unauthorised)";
            }
            catch (Exception ex)
            {
                directoryData.Name += " (Unable to calculate size - Error)";
            }

            _directoriesProcessed++;

            if (_directoriesProcessed % 10 == 0)
            {
                PrintProgress();
            }

            return directorySizeBytes;
        }

        private static void PrintProgress()
        {
            Console.SetCursorPosition(32, 0);

            Console.Write(_directoriesProcessed);
        }

    }
}
