using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListingsMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private RoomListings _roomListing;
    [SerializeField]
    private Transform _content;

    private List<RoomListings> _listings = new List<RoomListings>();
    private RoomCanvases _roomCanvases;

    public void FirstInitialize(RoomCanvases canvases)
    {
        _roomCanvases = canvases;
    }
    public override void OnJoinedRoom()
    {
        _roomCanvases.CurrentRoomCanvas.Show();
        _content.DestryChildren();
        _listings.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                 if(index !=-1)
                {
                    Destroy(_listings[index].gameObject);
                    _listings.RemoveAt(index);
                }
            }
            else
            {
                int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);

                if (index == -1)
                {
                    RoomListings listing = Instantiate(_roomListing, _content);
                    if (_listings != null)
                    { 
                        listing.SetRoomInfo(info);
                        _listings.Add(listing);
                    }
                }
                else
                {
                    //
                }
            }
        }
    }


}
