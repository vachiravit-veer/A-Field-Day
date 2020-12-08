using ExitGames.Client.Photon;
using ProtoBuf;
using System.IO;

public static class ProtobufManager 
{
 
    public static byte[] Serialize<T>(T obj)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            Serializer.Serialize(stream, obj);
            return stream.ToArray();
        }
    }


    public static T Deserialize<T>(byte[] data) 
    {
        using (MemoryStream stream = new MemoryStream(data))
        {
            return Serializer.Deserialize<T>(stream);
        }
    }

}

