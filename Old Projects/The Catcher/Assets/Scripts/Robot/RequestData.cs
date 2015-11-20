using System;

public class RequestData
{
    public int Status { get; set; }
    public double Position { get; set; }

    public Byte[] Buffer { get; set; }

    public RequestData(int bufferSize)
    {
        Buffer = new Byte[bufferSize];
    }

    public void Deserialize()
    {
        Status = BitConverter.ToInt32(Buffer, 0);
        Position = BitConverter.ToDouble(Buffer, 4);
    }
}
