using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace NyxCli
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            string command = args[0].ToLower();

            if (command == "convert" && args.Length >= 4)
            {
                HandleConversion(args[1], args[3]);
            }
            else if (command == "package" && args.Length >= 2)
            {
                HandlePackaging(args[1]);
            }
        }

        static void HandleConversion(string sourcePath, string targetExtension)
        {
            if (!File.Exists(sourcePath))
            {
                return;
            }

            string directory = Path.GetDirectoryName(sourcePath);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(sourcePath);
            
            if (!targetExtension.StartsWith("."))
            {
                targetExtension = "." + targetExtension;
            }

            string destinationPath = Path.Combine(directory ?? "", fileNameWithoutExt + targetExtension.ToLower());

            File.Copy(sourcePath, destinationPath, true);
        }

        static void HandlePackaging(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            var manifest = new NyxManifest
            {
                ProjectName = Path.GetFileName(Path.GetFullPath(directoryPath)),
                Videos = new List<string>(),
                Audio = new List<string>(),
                Images = new List<string>()
            };

            string[] files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                string ext = Path.GetExtension(file).ToLower();
                string relativePath = Path.GetRelativePath(directoryPath, file).Replace('\\', '/');

                if (ext == ".nyxvid")
                {
                    manifest.Videos.Add(relativePath);
                }
                else if (ext == ".nyxaud")
                {
                    manifest.Audio.Add(relativePath);
                }
                else if (ext == ".nyximg")
                {
                    manifest.Images.Add(relativePath);
                }
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(manifest, options);
            string outputPath = Path.Combine(directoryPath, "project.nyxcode");

            File.WriteAllText(outputPath, jsonString);
        }
    }

    public class NyxManifest
    {
        public string ProjectName { get; set; }
        public List<string> Videos { get; set; }
        public List<string> Audio { get; set; }
        public List<string> Images { get; set; }
    }
}
