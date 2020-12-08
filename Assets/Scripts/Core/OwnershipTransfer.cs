using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OwnershipTransfer : MonoBehaviourPun , IPunOwnershipCallbacks
{
    // Start is called before the first frame update
    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

     void OnMouseDown()
    {
        base.photonView.RequestOwnership();
        Debug.Log("Requesting ownership");
    }
    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != base.photonView)
            return;
        //targetView.Owner
        Debug.Log("transfer Owner to " + requestingPlayer.NickName);

        base.photonView.TransferOwnership(requestingPlayer);
       
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView != base.photonView)
            return;
    }


}
