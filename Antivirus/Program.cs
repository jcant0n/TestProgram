using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Antivirus
{
    class Program
    {
        static List<FileInfo> files = new List<FileInfo>();  // List that will hold the files and subfiles in path
        static List<DirectoryInfo> folders = new List<DirectoryInfo>(); // List that hold direcotries that cannot be accessed

        static void Main(string[] args)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                try
                {
                    if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                    {
                        //Console.WriteLine(drive.Name);
                        //Console.WriteLine(drive.DriveType);
                        //DirectoryInfo dir = new DirectoryInfo(@"C:\Users\javie_000\Desktop");
                        FullDirList(drive.RootDirectory, Settings.Default.Pattern);
                    }

                }
                catch (Exception ex)
                {
                    //Console.WriteLine("##############################################");
                }
            }

            var applicationPath = Path.GetPathRoot(Environment.CurrentDirectory);

            var now = DateTime.Now.ToUniversalTime();
            string folderName = string.Format("{0}{1}{2}_{3}{4}{5}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            var folder = Path.Combine(applicationPath, folderName);
            if (!Directory.Exists(folder))
            {
                try
                {
                    DirectoryInfo di = Directory.CreateDirectory(folder);
                    di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error");
                    return;
                }
            }

            Console.WriteLine("");
            float index = 1;
            foreach (var file in files)
            {
                string percentage = string.Format("\r{0:00}%", (index / (float)files.Count) * 100f);
                Console.Write(percentage);

                var destination = Path.Combine(folder, file.Name);

                try
                {
                    if (File.Exists(destination))
                    {
                        do
                        {
                            string newName = Path.GetFileNameWithoutExtension(destination);
                            newName += "0";

                            string extension = Path.GetExtension(destination);
                            string finalName = string.Format("{0}{1}", newName, extension);

                            destination = Path.Combine(folder, finalName);
                        }
                        while (File.Exists(destination));
                    }

                    File.Copy(file.FullName, destination);
                }
                catch (Exception ex)
                {
                }

                index++;
            }

            Console.WriteLine("\nDone");
            Console.Read();
        }

        static void FullDirList(DirectoryInfo dir, string searchPattern)
        {
            //Console.WriteLine("Directory {0}", dir.FullName);
            // list the files

            string[] patterns = searchPattern.Split(';');

            foreach (var pattern in patterns)
            {
                try
                {
                    foreach (FileInfo f in dir.GetFiles(pattern))
                    {
                        //Console.WriteLine("File {0}", f.FullName);
                        Console.Write(".");
                        files.Add(f);
                    }
                }
                catch
                {
                    //Console.WriteLine("Directory {0}  \n could not be accessed!!!!", dir.FullName);
                    return;  // We alredy got an error trying to access dir so dont try to access it again
                }
            }

            // process each directory
            // If I have been able to see the files in the directory I should also be able 
            // to look at its directories so I dont think I should place this in a try catch block
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                folders.Add(d);
                FullDirList(d, searchPattern);
            }
        }
    }
}
