class BitShift
{
    private byte[] _bytes;

    public BitShift()
    {
        _bytes = new byte[0];
    }

    public BitShift Shift(byte[] target, int length, int startBit)
    {
        var newBytes = new byte[length];
        for (int i = 0; i < length; i++)
        {
            newBytes[i] = (byte)((target[startBit / 8 + i] >> (startBit % 8)) & (0xff >> (8 - length)));
        }
        var temp = new byte[_bytes.Length + newBytes.Length];
        Buffer.BlockCopy(_bytes, 0, temp, 0, _bytes.Length);
        Buffer.BlockCopy(newBytes, 0, temp, _bytes.Length, newBytes.Length);
        _bytes = temp;
        return this;
    }

    public byte[] GetBytes()
    {
        return _bytes;
    }

    public void Clear()
    {
        _bytes = new byte[0];
    }
}
class BitUnpacker
{
    private byte[] _bytes;
    private int _startBit;

    public BitUnpacker(byte[] bytes)
    {
        _bytes = bytes;
        _startBit = 0;
    }

    public T Unpack<T>(int length) where T : struct
    {
        var size = System.Runtime.InteropServices.Marshal.SizeOf<T>();
        var buffer = new byte[size];
        var endBit = _startBit + length;
        var srcIndex = _startBit / 8;
        var destIndex = 0;
        var bitOffset = _startBit % 8;
        var remainingBits = length;

        while (remainingBits > 0)
        {
            var bitsToCopy = System.Math.Min(8 - bitOffset, remainingBits);
            var mask = (byte)((1 << bitsToCopy) - 1);
            buffer[destIndex++] = (byte)((_bytes[srcIndex++] >> bitOffset) & mask);
            remainingBits -= bitsToCopy;
            bitOffset = 0;
        }
        _startBit = endBit;
        var handle = System.Runtime.InteropServices.GCHandle.Alloc(buffer, System.Runtime.InteropServices.GCHandleType.Pinned);
        var result = System.Runtime.InteropServices.Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
        handle.Free();
        return result;
    }

    public void Clear()
    {
        _bytes = null;
        _startBit = 0;
    }
}
