using COD_FF_Extractor.Decompression;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace COD_FF_Extractor
{
    internal class Program
    {
        static string outputPath = "output";

        enum GAME_VERSION
        {
            WARZONE,
            MW2
        }

        static void checkRequiredFiles()
        {
            if (!File.Exists("dll\\oo2core_8_win64.dll"))
                throw new Exception("dll\\oo2core_8_win64.dll not found!");

            if (!File.Exists("dll\\MWBDiff.dll"))
                throw new Exception("dll\\MWBDiff.dll not found!");

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
        }

        static void decompressUsingGameVersion(string path, string outPath, GAME_VERSION gameVersion)
        {
            if (gameVersion == GAME_VERSION.WARZONE)
                Warzone.decompress(path, outPath);
            else if (gameVersion == GAME_VERSION.MW2)
                MW2.decompress(path, outPath);
        }

        static int getDiffResultSizeUsingGameVersion(string patchFilePath, GAME_VERSION gameVersion)
        {
            if (gameVersion == GAME_VERSION.WARZONE)
                return Warzone.getDiffResultSizeFromFile(patchFilePath);
            else if (gameVersion == GAME_VERSION.MW2)
                return MW2.getDiffResultSizeFromFile(patchFilePath);
            else 
                return 0;
        }

        static void decompressFile(string filePath, GAME_VERSION gameVersion)
        {
            string fileName = Path.GetFileName(filePath);

            Console.WriteLine("Decompressing {0}...", fileName);

            string resultPath = Path.Combine(outputPath, fileName + ".decomp");
            decompressUsingGameVersion(filePath, resultPath, gameVersion);

            string diffFilePath = filePath.Replace(".ff", ".fp");
            string diffFileName = Path.GetFileName(diffFilePath);
            if (File.Exists(diffFilePath))
            {
                Console.WriteLine("Applying patch file...");

                string diffResultPath = Path.Combine(outputPath, diffFileName + ".diff");
                decompressUsingGameVersion(diffFilePath, diffResultPath, gameVersion);

                if (!Utils.IsFileEmpty(diffResultPath))
                {
                    int fileSizeAfterPatching = getDiffResultSizeUsingGameVersion(diffFilePath, gameVersion);

                    byte[] patchedData = MWBDiff.BDiff.patchData(File.ReadAllBytes(resultPath), File.ReadAllBytes(diffResultPath), fileSizeAfterPatching);

                    File.WriteAllBytes(resultPath, patchedData);
                }
                
                File.Delete(diffResultPath);
            }


            Console.WriteLine("Sucessful!");
        }

        static void Main(string[] args)
        {
            checkRequiredFiles();

            if (args.Length != 2)
            {
                Console.WriteLine("Incorrect usage! Usage:");
                Console.WriteLine("COD FF Extractor.exe <GAME> <PATH>");
                Console.WriteLine("GAME can be: \"MW2\" or \"WZ\"");
                Console.WriteLine("PATH is a path to a FastFile file or a folder of FastFiles");
                return;
            }

            string gameVersionStr = args[0];
            string path = args[1];

            GAME_VERSION gameVersion;
            if (gameVersionStr.Equals("WZ"))
                gameVersion = GAME_VERSION.WARZONE;
            else if (gameVersionStr.Equals("MW2"))
                gameVersion = GAME_VERSION.WARZONE;
            else
                throw new Exception(string.Format("Unknown game type {0}!", gameVersionStr));

            if (Utils.IsDirectory(path))
            {
                foreach (string file in Directory.GetFiles(path, "*.ff"))
                {
                    decompressFile(file, gameVersion);
                }
            }
            else
            {
                if (args[0].EndsWith(".ff"))
                    decompressFile(path, gameVersion);
                else
                    throw new Exception("File must end with .ff!");
            }





            
        }
    }
}

//if (args.Length != 2)
//{
//    Console.WriteLine("Incorrect argument count!");
//    Console.WriteLine("usage is: MW2 FF Extractor.exe <path to folder or .ff>");
//    return;
//}
//
//if(Utils.IsDirectory(args[0]))
//{
//    foreach (string file in Directory.GetFiles(args[0], "*.ff"))
//    {
//        decompressFile(file);
//    }
//}
//else
//{
//    if(args[0].EndsWith(".ff"))
//        decompressFile(args[0]);
//    else
//        throw new Exception("File must end with .ff!");
//}