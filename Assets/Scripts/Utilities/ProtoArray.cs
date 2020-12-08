using Photon.Pun;

public class ProtoArray<T>
{
    public static T[,] ToArray(T[] a)
    {
        int MaxSize = (int)PhotonNetwork.CurrentRoom.CustomProperties["maxSize"];

        T[,] array = new T[MaxSize, MaxSize];
        for (int i = 0; i < MaxSize; i++)
        {
            for (int j = 0; j < MaxSize; j++)
            {
                array[i, j] = a[i * MaxSize + j];
            }
        }

        return array;
    }

}