using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerListingsMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private PlayerListings _playerListing;
    [SerializeField]
    private Transform _content;
    [SerializeField]
    private Text _readyUpText;
    [SerializeField]
    private GameObject _readyUpButton;
    [SerializeField]
    private GameObject _startButton;

    private List<PlayerListings> _listings = new List<PlayerListings>();
    private RoomCanvases _roomCanvases;
    private bool _ready = false;
    
    public override void OnEnable()
    {
        if (PhotonNetwork.IsMasterClient)
            _readyUpButton.SetActive(false);
        else
            _startButton.SetActive(false);

        base.OnEnable();
        SetReadyUp(false);
        GetCurrentRoomPlayers();
    }
    public override void OnDisable()
    {
        if (PhotonNetwork.IsMasterClient)
            _readyUpButton.SetActive(true);
        else
            _startButton.SetActive(true);

        base.OnDisable();
        for(int i=0;i<_listings.Count;i++)
            Destroy(_listings[i].gameObject);


        _listings.Clear();
    }

    private void SetReadyUp(bool state)
    {
        _ready = state;
        if (_ready)
            _readyUpText.text = "Ready";
        else
            _readyUpText.text = "Not Ready";
    }

    public void FirstInitialize(RoomCanvases canvases)
    {
        _roomCanvases = canvases;
    }

    private void GetCurrentRoomPlayers()
    {
        if (!PhotonNetwork.IsConnected)
            return;
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;

        foreach(KeyValuePair<int,Player> playerInfo in PhotonNetwork.CurrentRoom.Players.OrderBy(x=>x.Value.ActorNumber))
        {
            AddPlayerListing(playerInfo.Value);
        }

    }

    private void AddPlayerListing(Player player)
    {
        int index = _listings.FindIndex(x => x.Player == player);
        if (index != -1)
        {
            _listings[index].SetPlayerInfo(player);
            _listings[index].AnimateRevealItem();
        }
        else
        { 
            PlayerListings listing = Instantiate(_playerListing, _content);
            if (listing != null)
            {
                listing.SetPlayerInfo(player);
                listing.AnimateRevealItem();
                _listings.Add(listing);
            }
        }
    }


    public override void OnMasterClientSwitched(Player newMasterClient)
    {
       _roomCanvases.CurrentRoomCanvas.LeaveRoomMenu.OnClick_LeaveRoom();
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListing(newPlayer);

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
       
        int index = _listings.FindIndex(x => x.Player == otherPlayer);
        if (index != -1)
        {
            //Destroy(_listings[index].gameObject);
            _listings[index].AnimateRemoveItem();
            _listings.RemoveAt(index);
        }
        
        

    }

    public void OnClick_StartGame()
    {
        
        if (PhotonNetwork.IsMasterClient)
        {
            /*for(int i = 0 ;i<_listings.Count;i++)
            {
                if(_listings[i].Player != PhotonNetwork.LocalPlayer)
                {
                    if (!_listings[i].Ready)
                        return;
                }
            }*/
        
            CreatePlayersProp();

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            /* if ((int)PhotonNetwork.CurrentRoom.CustomProperties["maxSize"] == 6)
                 PhotonNetwork.LoadLevel(1);
             else
                 PhotonNetwork.LoadLevel(2);*/



            base.photonView.RPC("RPC_LoadGameScene", RpcTarget.All);


        }

    }

    private void CreatePlayersProp()
    {
        int i = 0;
        Hashtable hashtable = new Hashtable();       
        int[] cards = new int[] { 1, 2 ,3,4,5,6,7,8,10,11,12};
        
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            hashtable["cards"] = cards.OrderBy(arg => Guid.NewGuid()).Take(3).ToArray();
            switch (i)
            {
                case 0:
                    hashtable["customID"] = i;
                    hashtable["color"] = "red";
                    hashtable["x"] = 0;
                    hashtable["y"] = 0;
                    break;
                case 1:
                    hashtable["customID"] = i;
                    hashtable["color"] = "yellow";
                    //ToDo Fetch this from create room settings
                    if ((int)PhotonNetwork.CurrentRoom.CustomProperties["maxSize"] == 8)
                    {
                        hashtable["x"] = 0;
                        hashtable["y"] = 7;

                    }
                    else
                    {
                        hashtable["x"] = 0;
                        hashtable["y"] = 5;
                    }
                    break;
                case 2:
                    hashtable["customID"] = i;
                    hashtable["color"] = "black";
                    if ((int)PhotonNetwork.CurrentRoom.CustomProperties["maxSize"] == 8)
                    {
                        hashtable["x"] = 7;
                        hashtable["y"] = 0;

                    }
                    else
                    {
                        hashtable["x"] = 5;
                        hashtable["y"] = 0;
                    }
                    break;
                case 3:
                    hashtable["customID"] = i;
                    hashtable["color"] = "purple";
                    if ((int)PhotonNetwork.CurrentRoom.CustomProperties["maxSize"] == 8)
                    {
                        hashtable["x"] = 7;
                        hashtable["y"] = 7;

                    }
                    else
                    {
                        hashtable["x"] = 5;
                        hashtable["y"] = 5;
                    }
                    break;

            }

            hashtable["home"] = 0;
            hashtable["rice"] = 0;
            hashtable["tree"] = 0;
            hashtable["water"] = 0;
            hashtable["protected"] = false;
            hashtable["grass"] = 5;
            p.SetCustomProperties(hashtable);
            hashtable.Clear();
            i++;
            Debug.Log("set Player" + p.NickName + "id" + p.CustomProperties["customID"]);

        }
    }

    public void OnClick_ReadyUp()
    {
       // Debug.Log("Client is " + PhotonNetwork.IsMasterClient);
        if(!PhotonNetwork.IsMasterClient)
        {
            SetReadyUp(!_ready);
            base.photonView.RPC("RPC_ChangedReadyState", RpcTarget.MasterClient,PhotonNetwork.LocalPlayer,_ready);
            base.photonView.RPC("RPC_SetChangedReadyState", RpcTarget.All, PhotonNetwork.LocalPlayer, _ready);
        }
    }

    [PunRPC]
    private void RPC_ChangedReadyState(Player player,bool ready)
    {

        int index = _listings.FindIndex(x => x.Player == player);
        if (index != -1)
        {
            _listings[index].Ready = ready;
            _listings[index].SetReadyText(ready);
        }

    }

    [PunRPC]
    private void RPC_SetChangedReadyState(Player player, bool ready)
    {

        int index = _listings.FindIndex(x => x.Player == player);
        if (index != -1)
        {
            _listings[index].SetReadyText(ready);
        }

    }


    [PunRPC]
    private void RPC_LoadGameScene()
    {
        LoadingScreen.Instance.Show(SceneManager.LoadSceneAsync("Scene_Online"));
    }

}
