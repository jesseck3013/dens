namespace dens.Core;

public static class Utils
{
    public static ushort ToUInt16(byte[] data)
    {
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(data);
        }

        return BitConverter.ToUInt16(data);
    }

    public static ushort ToUInt16(byte octect1, byte octect2)
    {
        var data = new byte[2] { octect1, octect2 };

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(data);
        }

        return BitConverter.ToUInt16(data);
    }

    public static uint ToUInt32(byte octect1, byte octect2, byte octect3, byte octect4)
    {
        var data = new byte[4] { octect1, octect2, octect3, octect4 };

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(data);
        }

        return BitConverter.ToUInt32(data);
    }

    public static byte[] GetBytes(ushort number)
    {
        var data = BitConverter.GetBytes(number);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(data);
        }

        return data;
    }
}
