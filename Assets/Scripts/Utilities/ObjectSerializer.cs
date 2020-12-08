

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class ObjectSerializer
{
    public static byte[] Serialize(object customobject)
    {
        BinaryFormatter binaryF = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryF.Serialize(memoryStream, customobject);

            return memoryStream.ToArray();
        }
    }

    public static object Deserialize(byte[] dataStream)
    {
      
        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryF = new BinaryFormatter();

            memoryStream.Write(dataStream, 0, dataStream.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var obj = binaryF.Deserialize(memoryStream);
            return obj;
        }
    }

}
