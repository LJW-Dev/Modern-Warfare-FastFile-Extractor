using System.Text;
using static MW2_FF_Extractor.Structs;

namespace MW2_FF_Extractor
{
    public static class Decompressor
    {
        // Decompresses FastFiles. returns -1 if it is an original FastFile, if it's a patch FastFile return the size of the patched FastFile
        public static long Decompress(string path, string outPath)
        {
            BinaryReader reader = Utils.OpenBinaryReader(path);
            if (reader == null)
                throw new Exception(String.Format("Unable to open file {0}.", path));

            BinaryWriter writer = Utils.OpenBinaryWriter(outPath);
            if (writer == null)
                throw new Exception(String.Format("Unable to create file {0}.", outPath));

            // Get FF type
            byte[] FFMagic = reader.ReadBytes(0x08);
            FastFileType FFType;

            if (Utils.CompareArray(FFMagic, FFHeaderMagic))
                FFType = FastFileType.REGULAR;
            else if (Utils.CompareArray(FFMagic, FFPatchHeaderMagic))
                FFType = FastFileType.PATCH;
            else
                throw new Exception("Invalid FF magic.");

            long DiffSize = -1;
            if (FFType == FastFileType.PATCH)
            {
                // Check if patch FF is empty
                reader.BaseStream.Position = 0x28;
                if (reader.ReadInt64() == 0) //Read patch FF compressed size
                    return DiffSize;

                // Get size of FF after patching
                reader.BaseStream.Position = 0x140;
                DiffSize = reader.ReadInt64();
            }

            // Read FF name
            if(FFType == FastFileType.REGULAR)
                reader.BaseStream.Position = 0x208;
            else
                reader.BaseStream.Position = 0x318;
            string FastFileName = Encoding.UTF8.GetString(reader.ReadBytes(0x40)).Replace("\x00", String.Empty);

            // Read the block header
            if (FFType == FastFileType.REGULAR)
                reader.BaseStream.Position = 0x80DC;
            else
                reader.BaseStream.Position = 0x81EC;
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

            reader.Close();
            writer.Close();
            
            // diff the files if there is a patch file
            string FFPatchPath = path.Replace(".ff", ".fp");
            if (DiffSize == -1 && File.Exists(FFPatchPath))
            {
                long outSize = Decompress(FFPatchPath, FFPatchPath + ".tmp");

                if(outSize > 0) 
                {
                    byte[] patchedFF = MWBDiff.BDiff.patchData(File.ReadAllBytes(outPath), File.ReadAllBytes(FFPatchPath + ".tmp"), outSize);

                    if(patchedFF == null)
                        throw new Exception("patchData returned null! outSize: " + outSize);

                    File.WriteAllBytes(outPath, patchedFF); // overwrite already written non patched FF
                    File.Delete(FFPatchPath + ".tmp");
                }
            }
            
            return DiffSize;
        }
    }
}
