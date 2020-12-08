using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCanvases : MonoBehaviour
{


    [SerializeField]
    private CreateandJoinRooomCanvas _createandJoinRooomCanvas;

    public CreateandJoinRooomCanvas CreateandJoinRooomCanvas { get { return _createandJoinRooomCanvas; } }

    [SerializeField]
    private CurrentRoomCanvas _currentRoomCanvas;
    public CurrentRoomCanvas CurrentRoomCanvas { get { return _currentRoomCanvas; } }

    private void Awake()
    {
        FirstInitialize();
    }

    private void FirstInitialize()
    {

        CreateandJoinRooomCanvas.FirstInitialize(this);
        CurrentRoomCanvas.FirstInitialize(this);

    }
}
