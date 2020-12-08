using Photon.Pun;
using UnityEngine;

public class CreatePlayerCharacter : MonoBehaviour
{

    public GameObject[] test;
    private GameObject go;

    private void Start()
    {
        if (!PhotonNetwork.InRoom)
            return;


        int maxSize = (int)PhotonNetwork.CurrentRoom.CustomProperties["maxSize"];

        switch (maxSize)
        {
            case 6:
                switch ((string)PhotonNetwork.LocalPlayer.CustomProperties["color"])
                {
                    case "red":
                        go = PhotonNetwork.Instantiate(test[0].name, new Vector3(2.33f, 2.5f, 2.62f), Quaternion.identity, 0);
                        break;
                    case "yellow":
                        go = PhotonNetwork.Instantiate(test[1].name, new Vector3(-2.67f, 2.5f, 2.62f), Quaternion.identity, 0);
                        break;
                    case "black":
                        go = PhotonNetwork.Instantiate(test[2].name, new Vector3(2.33f, 2.5f, -2.38f), Quaternion.identity, 0);
                        break;
                    case "purple":
                        go = PhotonNetwork.Instantiate(test[3].name, new Vector3(-2.67f, 2.5f, -2.38f), Quaternion.identity, 0);
                        break;

                }
                break;
            case 8:
                switch ((string)PhotonNetwork.LocalPlayer.CustomProperties["color"])
                {
                    case "red":
                        go = PhotonNetwork.Instantiate(test[0].name, new Vector3(3.27f, 2.5f, 4.33f), Quaternion.identity, 0);
                        break;
                    case "yellow":
                        go = PhotonNetwork.Instantiate(test[1].name, new Vector3(-3.7f, 2.5f, 4.33f), Quaternion.identity, 0);
                        break;
                    case "black":
                        go = PhotonNetwork.Instantiate(test[2].name, new Vector3(3.27f, 2.5f, -2.82f), Quaternion.identity, 0);
                        break;
                    case "purple":
                        go = PhotonNetwork.Instantiate(test[3].name, new Vector3(-3.7f, 2.5f, -2.82f), Quaternion.identity, 0);
                        break;

                }

                break;

        }

        go.GetComponent<PhotonView>().Owner.TagObject = go;


       /* foreach (KeyValuePair<int, Player> p in PhotonNetwork.CurrentRoom.Players)
        {
            if (p.Value.IsLocal)
            {
                GameObject go = new GameObject();
                // GameObject go = new GameObject();
                switch ((string)p.Value.CustomProperties["color"])
                {
                    case "red":
                        go = PhotonNetwork.Instantiate(test[0].name, new Vector3(2.33f, 2.5f, 2.62f), Quaternion.identity, 0);
                        break;
                    case "yellow":
                        go = PhotonNetwork.Instantiate(test[1].name, new Vector3(-2.67f, 2.5f, 2.62f), Quaternion.identity, 0);
                        break;
                    case "black":
                        go = PhotonNetwork.Instantiate(test[2].name, new Vector3(2.33f, 2.5f, -2.38f), Quaternion.identity, 0);
                        break;
                    case "purple":
                        go = PhotonNetwork.Instantiate(test[3].name, new Vector3(-2.67f, 2.5f, -2.38f), Quaternion.identity, 0);
                        break;

                }
                go.GetComponent<PhotonView>().Owner.TagObject = go;


            }
        }
        */


    }



}
