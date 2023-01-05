using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace OodleSharp
{
    public static class Oodle
    {
        [DllImport("dll\\oo2core_8_win64.dll")]
        private static extern int OodleLZ_Decompress(byte[] compressedBuffer, long compressedBufferSize, byte[] outputBuffer, long outputBufferSize,
            uint fuzzSafe, uint checkCRC, ulong verbosity, uint decBufBase, uint decBufSize, uint fpCallback, uint callbackUserData,
            uint decoderMemory, uint decoderMemorySize, uint threadPhase);

        public static byte[] Decompress(byte[] compressedBuffer, int compressedLength, int decompressedSize)
        {
            byte[] decompressedBuffer = new byte[decompressedSize];
            int decompressedCount = OodleLZ_Decompress(compressedBuffer, compressedLength, decompressedBuffer, decompressedSize, 1, 0, 0, 0, 0, 0, 0, 0, 0, 3);

            if (decompressedCount == decompressedSize)
            {
                return decompressedBuffer;
            }
            else
            {
                return null;
            }
        }
    }
}
