using Library;
using static System.Console;

const string dirToRead = "B:\\001 Video Lectures";

Console.WriteLine("Working. Directories processed: ");
Reader r = new Reader();
r.GetAllFiles(dirToRead);

