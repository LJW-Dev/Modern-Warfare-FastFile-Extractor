using COD_FF_Extractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static COD_FF_Extractor.Structs;

namespace COD_FF_Extractor.Decompression
{
    class MW2
    {
        private static void decompressBlocks(BinaryReader reader, BinaryWriter writer)
        {
            BlockHeader headerBlock = new BlockHeader();
            headerBlock.decompressedDataLen = reader.ReadInt32();
            reader.BaseStream.Position += 0x3;
            headerBlock.compressionType = reader.ReadByte();

            long decompressedDataCount = 0;
            int loopCount = 1;
            Block block = new Block();
            while (decompressedDataCount < headerBlock.decompressedDataLen)
            {
#if DEBUG
                Console.WriteLine("Block Offset: {0:X}, total: {1:X}", reader.BaseStream.Position, decompressedDataCount);
#endif

                if (loopCount == 512)
                {
                    // Skip an RSA block
                    reader.ReadBytes(0x4000);
                    loopCount = 0;
                }

                block.compressedLen = (reader.ReadInt32() + 3) & 0xFFFFFFC; // calc allignment
                block.decompressedLen = reader.ReadInt32();
                reader.BaseStream.Position += 0x4;

                byte[] compressedData = reader.ReadBytes(block.compressedLen);
                byte[] decompressedData;
                switch (headerBlock.compressionType)
                {
                    // Decompress None
                    case 1:
                        decompressedData = compressedData;
                        break;

                    // unknown
                    case 4:
                    case 5:
                        throw new Exception("unimplemented compression type!");

                    // Decompress Oodle
                    case 6:
                    case 7:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                        decompressedData = OodleSharp.Oodle.Decompress(compressedData, block.compressedLen, block.decompressedLen);
                        break;

                    default:
                        throw new Exception("Unknown type of compression!");
                }

                if (decompressedData == null)
                    throw new Exception("Decompressor returned null!");

                writer.Write(decompressedData);

                decompressedDataCount += block.decompressedLen;
                loopCount++;
            }
        }

        private static void decompressRegularFF(BinaryReader reader, BinaryWriter writer)
        {
            // move to start of the signed fast file
            long fastFileStart = 0xDC;
            reader.BaseStream.Position = fastFileStart;

            // skip SHA256 hashes
            reader.BaseStream.Position += 0x8000;

            decompressBlocks(reader, writer);
        }

        private static void decompressPatchFF(BinaryReader reader, BinaryWriter writer)
        {
            reader.BaseStream.Position = 0x28;
            if (reader.ReadInt32() == 0) // check if it's an empty patch file, exit if it is
                return;

            // move to start of the signed fast file
            long fastFileStart = 0x1EC;
            reader.BaseStream.Position = fastFileStart;

            // skip SHA256 hashes
            reader.BaseStream.Position += 0x8000;

            decompressBlocks(reader, writer);
        }

        public static void decompress(string path, string outPath)
        {
            BinaryReader reader = Utils.OpenBinaryReader(path);
            if (reader == null)
                throw new Exception(String.Format("Unable to open file {0}.", path));

            BinaryWriter writer = Utils.OpenBinaryWriter(outPath);
            if (writer == null)
                throw new Exception(String.Format("Unable to create file {0}.", outPath));

            byte[] FFMagic = reader.ReadBytes(0x08);

            if (Utils.CompareArray(FFMagic, FFHeaderMagic))
            {
                decompressRegularFF(reader, writer);
            }
            else if (Utils.CompareArray(FFMagic, FFPatchHeaderMagic))
            {
                decompressPatchFF(reader, writer);
            }
            else
                throw new Exception("Invalid FF magic.");

            reader.Close();
            writer.Close();
        }

        // gets the resulting size of a fastfile when patched with a patch fastfile
        // patchFilePath must be the path to a patch file
        public static int getDiffResultSizeFromFile(string patchFilePath)
        {
            BinaryReader reader = Utils.OpenBinaryReader(patchFilePath);
            if (reader == null)
                throw new Exception(String.Format("Unable to open file {0}.", patchFilePath));

            reader.BaseStream.Position = 0x140;
            int diffResultSize = reader.ReadInt32();

            reader.Close();

            return diffResultSize;
        }
    }
}
