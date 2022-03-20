using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionServiceServer.Utils
{
    public static class ByteLengthUtils
    {
        public static byte[] GetLengthArray(int size)
        {
            return new byte[] 
            {
                (byte) (size >> 24),
                (byte) (size >> 16),
                (byte) (size >> 8),
                (byte) size
            };
        }

        public static int GetLengthInteger(byte[] length)
        {
            return (length[0] << 24) | (length[1] << 16) | (length[2] << 8) | length[3];
        }

    }
}
