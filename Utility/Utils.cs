using System;
using System.IO;

namespace COD_FF_Extractor
{
    public static class Utils
    {
        public static BinaryReader OpenBinaryReader(string path)
        {
            try
            {
                BinaryReader reader = new BinaryReader(File.OpenRead(path));
                return reader;
            }
            catch (IOException)
            {
                return null;
            }
        }

        public static BinaryWriter OpenBinaryWriter(string path)
        {
            try
            {
                BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create, FileAccess.Write));
                return writer;
            }
            catch (IOException)
            {
                return null;
            }
        }

        public static bool CompareArray(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                    return false;
            }

            return true;
        }

        public static bool IsDirectory(string path)
        {
            return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public static bool IsFileEmpty(string path)
        {
            return new FileInfo(path).Length == 0;
        }
    }
}
