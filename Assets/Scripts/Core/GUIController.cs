using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainCanvas,_Controller,_PickUpPanel,_PickupCanvas,_Inventory,_swapText,_Winner,_TileInventory, _SpecialTileInventory,_Messages;
    [SerializeField]
    private Sprite[] _specialTiles,Red,Yellow,_players,_black,_purple;


    public void ShowPlayerController()
    {
        _Controller.SetActive(true);
    }

    public void HidePlayerController()
    {
        _Controller.SetActive(false);
    }

    public void ShowPickUpPanel(string c,string t)
    {
        StartCoroutine(ShowPopUp(c,t));
    }

    private IEnumerator ShowPopUp(string color, string Tiletype)
    {
       // Debug.Log("Found color " + color + "Type" + Tiletype);
        switch (color)
        {
            case "":
                switch (Tiletype)
                {
                    case "portal":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = _specialTiles[0];
                        break;
                    case "swapPosition":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = _specialTiles[1];
                        break;
                    case "swapCharacter":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = _specialTiles[2];
                        break;
                    case "wind":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = _specialTiles[3];
                        break;
                    case "rain":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = _specialTiles[4];
                        break;
 
                }
                break;
            case "red":
                switch (Tiletype)
                {
                    case "home":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = Red[0];
                        break;
                    case "rice":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = Red[1];
                        break;
                    case "tree":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = Red[2];
                        break;
                    case "water":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = Red[3];
                        break;
                    case "start":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = Red[4];
                        break;

                }
                break;
            case "yellow":
                switch (Tiletype)
                {
                    case "home":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = Yellow[0];
                        break;
                    case "rice":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = Yellow[1];
                        break;
                    case "tree":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = Yellow[2];
                        break;
                    case "water":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = Yellow[3];
                        break;
                    case "start":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = Yellow[4];
                        break;

                }
                break;
            case "black":
                switch (Tiletype)
                {
                    case "home":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = _black[0];
                        break;
                    case "rice":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = _black[1];
                        break;
                    case "tree":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = _black[2];
                        break;
                    case "water":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = _black[3];
                        break;
                    case "start":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = _black[4];
                        break;

                }
                break;
            case "purple":
                switch (Tiletype)
                {
                    case "home":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = _purple[0];
                        break;
                    case "rice":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = _purple[1];
                        break;
                    case "tree":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = _purple[2];
                        break;
                    case "water":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = _purple[3];
                        break;
                    case "start":
                        _PickUpPanel.transform.Find("Image").GetComponent<Image>().sprite = _purple[4];
                        break;

                }
                break;

        }

        _PickupCanvas.SetActive(true);
        yield return new WaitForSeconds(1f);
        _PickupCanvas.SetActive(false);
        
  

    }

    public void showCardInventory()
    {

        if (_Inventory.GetComponentsInChildren<Transform>().GetLength(0) == 1)
        {
            Debug.Log("No Card left");
            return;

        }
        else
        {
            HidePlayerController();
            _Inventory.SetActive(true);
        }
    }
    public void hideCardInventory()
    {
        _Inventory.SetActive(false);
        ShowPlayerController();

    }

    public void ShowSwapText()
    {
        _swapText.SetActive(true);
    }
    public void HideSwapText()
    {
        _swapText.SetActive(false);
    }

    public void ShowTileInventory()
    {
        _Controller.SetActive(false);
        _TileInventory.SetActive(true);
    }

    public void HideTileInventory()
    {
        _TileInventory.SetActive(false);
    }

    public void ShowSpecialInventory()
    {
        _Controller.SetActive(false);
        _SpecialTileInventory.SetActive(true);
    }

    public void HideSpecialInventory()
    {
        _SpecialTileInventory.SetActive(false);
        _Controller.SetActive(true);
    }

    public void HideSpecialInventoryWaitForEndTurn()
    {
        _SpecialTileInventory.SetActive(false);
    }

    public void ShowWinner(int id)
    {
        _mainCanvas.SetActive(false);
        _Winner.SetActive(true);
        _Winner.transform.GetChild(0).GetChild(3).GetComponent<Image>().sprite = _players[id];

    }

    public void OnLose()
    {
        _swapText.GetComponent<Text>().text = "คุณไม่ได้ไปต่อ";
        _swapText.SetActive(true);
        
        Invoke("LeaveRoom", 3);
    }

    public IEnumerator ShowMessage(string msg)
    {
        _Messages.GetComponent<Text>().text = msg;
        _Messages.SetActive(true);
        yield return new WaitForSeconds(1f);
        _Messages.SetActive(false);

    }


    public void LeaveRoom()
    {
       // PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
      //  PhotonNetwork.LeaveLobby();
    }

}
