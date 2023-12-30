using System.Reflection.PortableExecutable;
using System.Text;
using static COD_FF_Extractor.Structs;

namespace COD_FF_Extractor
{
    public static class Structs
    {
        //IWffa100
        public static readonly byte[] FFHeaderMagic = new byte[] { 0x49, 0x57, 0x66, 0x66, 0x61, 0x31, 0x30, 0x30 };

        //IWffd100
        public static readonly byte[] FFPatchHeaderMagic = new byte[] { 0x49, 0x57, 0x66, 0x66, 0x64, 0x31, 0x30, 0x30 };

        public enum FastFileType
        {
            REGULAR,
            PATCH
        }
        public struct FastFileHeader
        {
            public byte[] Magic;
            public int Version;
            public byte isCompressed;
            public byte isEncrypted;
        }

        public struct BlockHeader
        {
            public int decompressedDataLen;
            public byte compressionType;
        }

        public struct Block
        {
            public int compressedLen;
            public int decompressedLen;
        }
    }
}
