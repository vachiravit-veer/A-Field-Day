using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using System.Linq;
using Photon.Realtime;

public class ChangeTexture : MonoBehaviour , IOnEventCallback
{

    [SerializeField]
    private Material _new_mats, _def_mats;

    private readonly byte ChangeTiletTexture = 0;

    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == ChangeTiletTexture)
        {
            object[] data = (object[])photonEvent.CustomData;

            string targetName = (string)data[0];
            int type = (int)data[1];
            GameObject go = GameObject.FindGameObjectsWithTag("Boards").First(x => x.name == targetName);

            MeshRenderer m_renderer = go.GetComponent<MeshRenderer>();
            switch (type)
            {
                case 0:
                    m_renderer.material = _def_mats;
                    break;
                case 1:
                    m_renderer.material = _new_mats;
                    break;
            }



        }
    }


}
