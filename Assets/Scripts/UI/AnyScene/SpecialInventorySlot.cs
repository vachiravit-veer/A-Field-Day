using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SpecialInventorySlot : MonoBehaviour
{
    [SerializeField]
    private Transform[] _slots;

    private bool Selected = false;
    private int SelectedType;
    // Start is called before the first frame update

    OnlineGameLogic _gameLogic;
    GUIController _gui;
    PlayerController _controller;
    private Player _localplayer;

    private bool _Started = true;
    void Start()
    {
        _localplayer = PhotonNetwork.LocalPlayer;
        _gameLogic = FindObjectOfType<OnlineGameLogic>();
        _gui = FindObjectOfType<GUIController>();
        _controller = FindObjectOfType<PlayerController>();
        _slots[0].GetChild(0).GetComponent<Button>().onClick.AddListener(selectedGrass);
        _slots[2].GetComponent<Button>().onClick.AddListener(GoBack);
        RefreshUI();
        _Started = false;
    }


    private void OnEnable()
    {
        if (_Started)
            return;
       RefreshUI();
    }
 

    private void selectedGrass()
    {
        if ((int)_localplayer.CustomProperties["grass"] == 0)
            return;

        _controller._isAllowedClick = true;
        OnlineGameLogic.OnClick_SpecialTile = true;
    }


    private void GoBack()
    {
        _gui.HideSpecialInventory();
    }

    private void RefreshUI()
    {
        _slots[0].GetChild(0).GetChild(1).GetComponent<Text>().text = ((int)_localplayer.CustomProperties["grass"]).ToString();

    }


}
