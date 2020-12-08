using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InventorySlot : MonoBehaviour
{
    [SerializeField]
    private Transform[] _slots;

    private bool Selected=false;
    private int SelectedType;
    // Start is called before the first frame update

    OnlineGameLogic _gameLogic;
    private Player _localplayer;
    void Start()
    {
        _localplayer = PhotonNetwork.LocalPlayer;
        _gameLogic = FindObjectOfType<OnlineGameLogic>();
        _slots[0].GetChild(0).GetComponent<Button>().onClick.AddListener(selectedHome);
        _slots[1].GetChild(0).GetComponent<Button>().onClick.AddListener(selectedRice);
        _slots[2].GetChild(0).GetComponent<Button>().onClick.AddListener(selectedTree);
        _slots[3].GetChild(0).GetComponent<Button>().onClick.AddListener(selectedWater);
        _slots[4].GetComponent<Button>().onClick.AddListener(GoBack);
        RefreshUI();
    }

    // Check if Tile = 0
    //
    void Update()
    {
        if (!Selected)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("Boards"))
            {
                if (_gameLogic.PossibleMove.Contains(hit.transform.gameObject))
                {
                    
                    _gameLogic.SetAvaliableAndTexture(hit.transform.name);
                    UpdateScore(SelectedType);
                    Selected = false;
                }

            }

        }


    }


    private void UpdateScore(int t)
    {
        Hashtable ht = new Hashtable();
        switch (t)
        {
            case 0:
                ht["home"] = (int)_localplayer.CustomProperties["home"] - 1;
                break;
            case 1:
                ht["rice"] = (int)_localplayer.CustomProperties["rice"] - 1;
                break;
            case 2:
                ht["tree"] = (int)_localplayer.CustomProperties["tree"] - 1;
                break;
            case 3:
                ht["water"] = (int)_localplayer.CustomProperties["water"] - 1;
                break;
        }

        _localplayer.SetCustomProperties(ht);

        RefreshUI();

    }

    private void selectedHome()
    {
        if ((int)_localplayer.CustomProperties["home"] == 0)
            return;
        Selected = true;
        SelectedType = 0;

    }

    private void selectedRice()
    {
        if ((int)_localplayer.CustomProperties["rice"] == 0)
            return;

        Selected = true;
            SelectedType = 1;


        Debug.Log("click select rice");

    }
    private void selectedTree()
    {
        if ((int)_localplayer.CustomProperties["tree"] == 0)
            return;

        Selected = true;
        SelectedType = 2;
        
    }
    private void selectedWater()
    {
        if ((int)_localplayer.CustomProperties["water"] == 0)
            return;


         Selected = true;
        SelectedType = 3;
        
    }


    private void GoBack()
    {      
        _gameLogic.EndCheckAtStart();
    }

    private void RefreshUI()
    {
       _slots[0].GetChild(0).GetChild(1).GetComponent<Text>().text = ((int)_localplayer.CustomProperties["home"]).ToString();
       _slots[1].GetChild(0).GetChild(1).GetComponent<Text>().text = ((int)_localplayer.CustomProperties["rice"]).ToString();
       _slots[2].GetChild(0).GetChild(1).GetComponent<Text>().text = ((int)_localplayer.CustomProperties["tree"]).ToString();
       _slots[3].GetChild(0).GetChild(1).GetComponent<Text>().text = ((int)_localplayer.CustomProperties["water"]).ToString();

    }


}
