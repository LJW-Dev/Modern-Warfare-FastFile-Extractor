using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MW2_FF_Extractor
{
    internal class Program
    {
        static void checkRequiredFiles()
        {
            if (!File.Exists("dll\\oo2core_8_win64.dll"))
                throw new Exception("dll\\oo2core_8_win64.dll not found!");

            if(!File.Exists("dll\\MWBDiff.dll"))
                throw new Exception("dll\\MWBDiff.dll not found!");
        }

        static void decompressFile(string filePath)
        {
            Console.WriteLine("Decompressing {0}", filePath);
            
            Decompressor.Decompress(filePath, filePath + ".decomp");

            Console.WriteLine("Decompression successful!");
        }

        static void Main(string[] args)
        {
            checkRequiredFiles();

            if (args.Length != 1)
            {
                Console.WriteLine("Incorrect argument count!");
                Console.WriteLine("usage is: MW2 FF Extractor.exe <path to folder or .ff>");
                return;
            }

            if(Utils.IsDirectory(args[0]))
            {
                foreach (string file in Directory.GetFiles(args[0], "*.ff"))
                {
                    decompressFile(file);
                }
            }
            else
            {
                if(args[0].EndsWith(".ff"))
                    decompressFile(args[0]);
                else
                    throw new Exception("File must end with .ff!");
            }
            
        }
    }
}