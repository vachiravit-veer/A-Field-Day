using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class CreateRoomMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Dropdown _SelectBoard;

    private RoomCanvases _roomCanvases;
    
    public void FirstInitialize(RoomCanvases canvases)
    {
        _roomCanvases = canvases;
    }

    public void OnClick_CreateRoom()
    {
 
        if (!PhotonNetwork.IsConnectedAndReady)
            return;
        if (PhotonNetwork.IsMasterClient)
            return;
       // if (!PhotonNetwork.InLobby)
         //   return;

      
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        options.CleanupCacheOnLeave = false;
        if (_SelectBoard.value == 0)
            options.CustomRoomProperties = new Hashtable() { { "maxSize", 6 } };
        else
            options.CustomRoomProperties = new Hashtable() { { "maxSize", 8 } };

        PhotonNetwork.CreateRoom(null,options,TypedLobby.Default);

    }

    public void OnClick_JoinRandom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Create room successfully");
        _roomCanvases.CurrentRoomCanvas.Show();
    }

    public override void OnJoinedRoom()
    {
        _roomCanvases.CurrentRoomCanvas.Show();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("ROom create failed" + message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join Failed" + message );
        OnClick_CreateRoom();
        //There's no Room Create Room Instead
    }


}
