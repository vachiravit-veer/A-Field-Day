using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineDiceRoller : MonoBehaviour
{
    private Sprite[] diceSides;
    private SpriteRenderer rend;
    private bool isRolling = false;
    // Start is called before the first frame update
 
    OnlineGameLogic _gameLogic;
    PlayerController _playerController;
    GUIController _gui;
    OnlineStateManager _stateManager;

    void Start()
    {
         _gameLogic = FindObjectOfType<OnlineGameLogic>();
        _playerController = FindObjectOfType<PlayerController>();
        _gui = FindObjectOfType<GUIController>();
        _stateManager = FindObjectOfType<OnlineStateManager>();
        rend = GetComponent<SpriteRenderer>();
        // Load dice sides sprites to array from DiceSides subfolder of Resources folder
        diceSides = Resources.LoadAll<Sprite>("Dice1/");

    }

    public void OnClick_ShowCardInventory()
    {
        _gui.showCardInventory();
    }
    public void OnClick_OneMove()
    {

        if (!_stateManager.IsRolling)
        {
            _gui.HidePlayerController();
            _stateManager.IsRolling = true;
            _gameLogic.FindLegalOneMove(-1, -1,true);
            //hide controller
            CreateHighlight.Instance.HighlightAllowedMoves(_gameLogic.PossibleMove);
            _playerController._isAllowedClick = true;
        }
    }

    public void OnClick_RollDice()
    {
        if (!_stateManager.IsRolling)
            StartCoroutine(RollTheDice());
    }
    private IEnumerator RollTheDice()
    {
        _stateManager.IsRolling = true;

        int randomDiceSide = 0;

        List<int> temp = _gameLogic.FindDir(-1, -1);
        var rnd = temp[Random.Range(0, temp.Count)];
 
 
        _gameLogic.FindLegalMove(true,rnd,6);
 
     
        for (int i = 0; i <= 10; i++)
        {
            // Pick up random value from 0 to 5 (All inclusive)
            randomDiceSide = Random.Range(0, 5);
            // Set sprite to upper face of dice from array according to random value
            rend.gameObject.GetComponent<Image>().sprite = diceSides[randomDiceSide];

            // Pause before next itteration
            yield return new WaitForSeconds(0.05f);
        }
        switch (rnd)
        {
            case 7:
                rend.gameObject.GetComponent<Image>().sprite = diceSides[0];
                break;
            case 6:
                rend.gameObject.GetComponent<Image>().sprite = diceSides[1];
                break;
            case 5:
                rend.gameObject.GetComponent<Image>().sprite = diceSides[2];
                break;
            case 4:
                rend.gameObject.GetComponent<Image>().sprite = diceSides[3];
                break;
            case 3:
                rend.gameObject.GetComponent<Image>().sprite = diceSides[4];
                break;
            case 2:
                rend.gameObject.GetComponent<Image>().sprite = diceSides[7];
                break;
            case 1:
                rend.gameObject.GetComponent<Image>().sprite = diceSides[5];
                break;
            case 0:
                rend.gameObject.GetComponent<Image>().sprite = diceSides[6];
                break;
        }
        if(_gameLogic.PossibleMove.Count==0)
        {
            _stateManager.EndTurn();
            yield break;

            //end turn
        }
        CreateHighlight.Instance.HighlightAllowedMoves(_gameLogic.PossibleMove);
        //hide controller
        _playerController._isAllowedClick = true;
        Debug.Log("Done Rolling");
        // Allow Click
        
    }

    public void OnClick_ShowSpecialTile()
    {
        _gameLogic.FindAllUnavaliable();
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["grass"] == 0)
        {
         
            StartCoroutine(_gui.ShowMessage("คุณไม่มีหมากพิเศษ"));
            //show message no tile
            return;
        }

        if(_gameLogic.PossibleMove.Count==0)
        {
            StartCoroutine(_gui.ShowMessage("ไม่พบช่องว่าง"));
            //show message no unavaliable
            return;
        }

        _gui.ShowSpecialInventory();

    }

 

}
