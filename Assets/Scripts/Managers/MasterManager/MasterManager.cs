using Photon.Pun;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(menuName ="Singletons/MasterManger")]
public class MasterManager : SingletonScriptableObject <MasterManager>
{
    [SerializeField]
    private GameSettings gameSettings;

    public static GameSettings GameSettings { get { return Instance.gameSettings; } }
    [SerializeField]
    private List<NetworkedPrefab> _networkedPrefabs = new List<NetworkedPrefab>();

    public static GameObject NetworkInstanitate(GameObject obj,Vector3 position,Quaternion rotation)
    {
        foreach(NetworkedPrefab networkedPrefab in Instance._networkedPrefabs)
        {
            if (networkedPrefab.Prefab == obj)
            {
                if (networkedPrefab.Path != string.Empty)
                { 
                    GameObject Result = PhotonNetwork.Instantiate(networkedPrefab.Path, position, rotation);
                    return Result;
                }
                else
                {
                    Debug.LogError("path is empty for gameobject " + networkedPrefab.Prefab);
                    return null;


                }
            }

        }

        return null;
    }

    public static void SceneNetworkInstanitate(GameObject obj, Vector3 position, Quaternion rotation)
    {
        foreach (NetworkedPrefab networkedPrefab in Instance._networkedPrefabs)
        {
            if (networkedPrefab.Prefab == obj)
            {
                if (networkedPrefab.Path != string.Empty)
                {
                    PhotonNetwork.InstantiateSceneObject(networkedPrefab.Path, position, rotation,0,null);
                    return;
                }
                else
                {
                    Debug.LogError("path is empty for gameobject " + networkedPrefab.Prefab);
                    return;
                }
            }

        }
        return;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void PopulateNetworkedPrefabs()
    {
#if UNITY_EDITOR

        Instance._networkedPrefabs.Clear();

        GameObject[] results = Resources.LoadAll<GameObject>("");
        //maybe  GameObject.FindGameObjectsWithTag("prefab");
        for (int i=0; i<results.Length;i++)
        {
            if(results[i].GetComponent<PhotonView>() != null)
            {
                string path = AssetDatabase.GetAssetPath(results[i]);
                Instance._networkedPrefabs.Add(new NetworkedPrefab(results[i], path));
            }
        }

  
#endif
    }


}
