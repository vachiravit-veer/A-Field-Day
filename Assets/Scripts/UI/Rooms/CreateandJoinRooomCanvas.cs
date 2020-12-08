using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateandJoinRooomCanvas : MonoBehaviour
{
    [SerializeField]
    private CreateRoomMenu _createRoomMenu;
    [SerializeField]
    private RoomCanvases _roomCanvases;
    public void FirstInitialize(RoomCanvases canvases)
    {
        _roomCanvases = canvases;
        _createRoomMenu.FirstInitialize(canvases);
    
    }



}
