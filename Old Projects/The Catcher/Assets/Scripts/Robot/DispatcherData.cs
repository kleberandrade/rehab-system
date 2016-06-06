using System;

public class DispatcherData
{
    public int Control { get; set; }
    public double Stiffness { get; set; }
    public double Position { get; set; }
    public double Velocity { get; set; }
    public double Acceleration { get; set; }
    public Byte[] Buffer { get; set; }

    public DispatcherData(int bufferSize)
    {
        Buffer = new Byte[bufferSize];
    }
    public void Serialize()
    {
        Array.Copy(BitConverter.GetBytes(Control), 0, Buffer, 0, sizeof(int));
        Array.Copy(BitConverter.GetBytes(Position), 0, Buffer, 4, sizeof(double));
        Array.Copy(BitConverter.GetBytes(Stiffness), 0, Buffer, 12, sizeof(double));
        Array.Copy(BitConverter.GetBytes(Velocity), 0, Buffer, 20, sizeof(double));
        Array.Copy(BitConverter.GetBytes(Acceleration), 0, Buffer, 28, sizeof(double));
    }
}