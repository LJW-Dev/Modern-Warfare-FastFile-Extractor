using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace MWBDiff
{
    public static class BDiff
    {
        [DllImport("dll\\MWBDiff.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern bool bdiff(
            byte[] sourceBuffer, long sourceBufferSize, 
            byte[] patchBuffer, long patchBufferSize,
            byte[] outBuffer, long outBufferSize);

        public static byte[] patchData(byte[] sourceBuffer, byte[] patchBuffer, long outBufferSize)
        {
            byte[] outBuffer = new byte[outBufferSize];

            if (bdiff(sourceBuffer, sourceBuffer.LongLength, patchBuffer, patchBuffer.LongLength, outBuffer, outBufferSize))
            {
                return outBuffer;
            }
            else
            {
                return null;
            }
        }
    }
}
