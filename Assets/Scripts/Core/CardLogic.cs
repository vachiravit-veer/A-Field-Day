using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardLogic : MonoBehaviour
{
    [SerializeField]
    private int UseCount=0;
    public static bool RevealCard = false;
    public static bool PickUpCard = false;
    PlayerController _playerController;
    OnlineGameLogic _gameLogic;
    OnlineStateManager _stateManager;
    GUIController _gui;
    private List<int[]> _allplayersPos;

    private int MaxSize;
    private void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _gameLogic = FindObjectOfType<OnlineGameLogic>();
        _stateManager = FindObjectOfType<OnlineStateManager>();
        _gui = FindObjectOfType<GUIController>();

        MaxSize = (int)PhotonNetwork.CurrentRoom.CustomProperties["maxSize"];

    }

    public void OnClick_useCard(int cardID,GameObject bt)
    {
        bool hasToCheckMove = false;

        switch (cardID)
        {
            case 1:
                Card1_5(3);
                hasToCheckMove = true;
                break;
            case 2:
                Card2();
                break;
            case 3:
               // Card3();
                break;
            case 4:
                Card4();
                hasToCheckMove = true;
                break;
            case 5:
                Card1_5(6);
                hasToCheckMove = true;
                break;
            case 6:
                Card6();
                hasToCheckMove = true;
                break;
            case 7:
                Card7_12();
                break;
            case 8:
                Card8();
                hasToCheckMove = true;
                break;
            case 10:
                Card10();
                hasToCheckMove = true;
                break;
            case 11:
                Card11();
                hasToCheckMove = true;
                break;
            case 12:
                Card7_12();
                break;
        }

        if(hasToCheckMove && _gameLogic.PossibleMove.Count == 0)
        {
            Debug.Log("Use Card But has no Legal Move");
            return;
        }
        if(hasToCheckMove)
        {
            _playerController._isAllowedClick = true;
            CreateHighlight.Instance.HighlightAllowedMoves(_gameLogic.PossibleMove);
        }
        _gui.hideCardInventory();
        bt.SetActive(false);
        
    }


    public void Card1_5(int max)
    {
        List<int> tempDir = _gameLogic.FindDir(-1,-1);
        _gameLogic.GetAllPlayersPosition(false);
        _gameLogic.PossibleMove.Clear();
        foreach(int i in tempDir)
        {
            _gameLogic.FindLegalMove(false, i, max);
        }

    }

    public void Card2()
    {
        _gameLogic.PopulateTempTile();
        _gameLogic.FindLegalOneMove(-1, -1,true);
        //Create Highight On possible move
        _gameLogic.PossibleMove.ForEach(x => CreateHighlight.Instance.HighlightAllowedMove(x, _gameLogic.F(x.name)));

        //set bool  able to click
        PickUpCard = true;
        _playerController._isAllowedClick = true;
        // Check only same color can pickup
        // + score
        //end turn
    }
    public void card3()
    {
        ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
        ht["protected"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
    }

    public void Card4()
    {
        List<int> tempDir = _gameLogic.FindDir(-1,-1);
        _gameLogic.GetAllPlayersPosition(false);
        int x = (int)PhotonNetwork.LocalPlayer.CustomProperties["x"];
        int y = (int)PhotonNetwork.LocalPlayer.CustomProperties["y"];
        _gameLogic.PossibleMove.Clear();
        tempDir.ForEach(i => Card4_Logic(i, x, y));   

    }

    public void Card6()
    {
        List<int> tempDir = _gameLogic.FindDir(-1, -1);
        _gameLogic.GetAllPlayersPosition(false);
        _gameLogic.PossibleMove.Clear();
        //call find Dir First
        if(tempDir.Contains(4))
            _gameLogic.FindLegalMove(false,4,6);
        if (tempDir.Contains(5))
            _gameLogic.FindLegalMove(false,5,6);
        if (tempDir.Contains(6))
            _gameLogic.FindLegalMove(false, 6,6);
        if (tempDir.Contains(7))
            _gameLogic.FindLegalMove(false, 7,6);

    }

    public void Card7_12()
    {
        Debug.Log("card7 start");

        _gameLogic.PopulateTempTile();
        _gameLogic.GetAllPlayersPosition(false);
        RevealCard = true;
        _playerController._isAllowedClick = true;
        //set bool 
        //create tiles on selected

    }

    public void Card8()
    {
        _gameLogic.PopulateTempTile();
        _gameLogic.GetAllPlayersPosition(false);

        int x = (int)PhotonNetwork.LocalPlayer.CustomProperties["x"];
        int y = (int)PhotonNetwork.LocalPlayer.CustomProperties["y"];
        List<int> tempDir = _gameLogic.FindDir(-1, -1);
      
        _gameLogic.PossibleMove.Clear();
 
        tempDir.Where(i=>i>=4).ToList().ForEach(i => CheckTileAvaliable_Player(i, x, y, true));
 

    }

    public void Card10()
    {
        _gameLogic.PopulateTempTile();
        _gameLogic.GetAllPlayersPosition(false);

        int x = (int)PhotonNetwork.LocalPlayer.CustomProperties["x"];
        int y = (int)PhotonNetwork.LocalPlayer.CustomProperties["y"];
        List<int> tempDir = _gameLogic.FindDir(-1, -1);
        _gameLogic.PossibleMove.Clear();
        tempDir.ForEach(i => CheckTileAvaliable_Player(i,x, y, true));

    }

    public void Card11()
    {
        _gameLogic.PopulateTempTile();
        _gameLogic.GetAllPlayersPosition(false);

        int x = (int)PhotonNetwork.LocalPlayer.CustomProperties["x"];
        int y = (int)PhotonNetwork.LocalPlayer.CustomProperties["y"];
        List<int> tempDir = _gameLogic.FindDir(-1, -1);
        _gameLogic.PossibleMove.Clear();
        tempDir.ForEach(i => CheckTileAvaliable_Player(i, x, y, false));

    }

    private void Card4_Logic(int dir, int x, int y)
    {
        // need player position
        switch (dir)
        {
            case 0:
                //false 
                if ( _gameLogic.FindIsPlayerInPosition(x, y + 1) || _gameLogic.tempTile[x, y + 1].isAvaliable)
                {
                    break;
                }
                for (int j = y + 2; j < MaxSize; j++)
                {
                    if (_gameLogic.FindIsPlayerInPosition(x, j) || !_gameLogic.tempTile[x,j].isAvaliable)
                    {
                        break;
                    }
                    _gameLogic.PossibleMove.Add(SpawnTiles.TilesName[x, j]);
                }
                break;
            case 1:
                if (_gameLogic.FindIsPlayerInPosition(x, y - 1) || _gameLogic.tempTile[x, y - 1].isAvaliable)
                {
                    break;
                }
                for (int j = y - 2; j > -1; j--)
                {
                    if (_gameLogic.FindIsPlayerInPosition(x, j) || !_gameLogic.tempTile[x, j].isAvaliable)
                    {
                        break;
                    }

                    _gameLogic.PossibleMove.Add(SpawnTiles.TilesName[x, y]);
                }
                break;
            case 2:
                if (_gameLogic.FindIsPlayerInPosition(x+1, y) || _gameLogic.tempTile[x + 1, y].isAvaliable)
                {
                    break;
                }
                for (int i = x + 2; i < MaxSize; i++)
                {
                    if (_gameLogic.FindIsPlayerInPosition(i, y) || !_gameLogic.tempTile[i, y].isAvaliable)
                    {
                        break;
                    }

                    _gameLogic.PossibleMove.Add(SpawnTiles.TilesName[i, y]);
                }
                break;
            case 3:
                if (_gameLogic.FindIsPlayerInPosition(x-1, y) || _gameLogic.tempTile[x - 1, y].isAvaliable)
                {
                    break;
                }
                for (int i = x - 2; i > -1; i--)
                {
                    if (_gameLogic.FindIsPlayerInPosition(i, y) || !_gameLogic.tempTile[i, y].isAvaliable)
                    {
                        break;
                    }

                    _gameLogic.PossibleMove.Add(SpawnTiles.TilesName[i, y]);
                }
                break;
            case 4:
                if (_gameLogic.FindIsPlayerInPosition(x+1, y +1) || _gameLogic.tempTile[x + 1, y + 1].isAvaliable)
                {
                    break;
                }
                for (int i = x + 2, j = y + 2; i < MaxSize && j < MaxSize; i++, j++)
                {
                    if (_gameLogic.FindIsPlayerInPosition(i, j) || !_gameLogic.tempTile[i, j].isAvaliable)
                    {
                        break;
                    }


                    _gameLogic.PossibleMove.Add(SpawnTiles.TilesName[i, j]);
                }
                break;
            case 5:
                if (_gameLogic.FindIsPlayerInPosition(x+1, y - 1) || _gameLogic.tempTile[x + 1, y - 1].isAvaliable)
                {
                    break;
                }
                for (int i = x + 2, j = y - 2; i < MaxSize && j > -1; i++, j--)
                {
                    if (_gameLogic.FindIsPlayerInPosition(i, j) || !_gameLogic.tempTile[i, j].isAvaliable)
                    {
                        break;
                    }

                    _gameLogic.PossibleMove.Add(SpawnTiles.TilesName[i, j]);
                }
                break;
            case 6:
                if (_gameLogic.FindIsPlayerInPosition(x - 1 , y + 1) || _gameLogic.tempTile[x - 1, y + 1].isAvaliable)
                {
                    break;
                }
                for (int i = x - 2, j = y + 2; i > -1 && j < MaxSize; i--, j++)
                {
                    if (_gameLogic.FindIsPlayerInPosition(i, j) || !_gameLogic.tempTile[i, j].isAvaliable)
                    {
                        break;
                    }

                    _gameLogic.PossibleMove.Add(SpawnTiles.TilesName[i, j]);

                }
                break;
            case 7:
                if (_gameLogic.FindIsPlayerInPosition(x-1, y - 1) || _gameLogic.tempTile[x - 1, y - 1].isAvaliable)
                {
                    break;
                }
                for (int i = x - 2, j = y - 2; i > -1 && j > -1; i--, j--)
                {
                    if (_gameLogic.FindIsPlayerInPosition(i, j) || !_gameLogic.tempTile[i, j].isAvaliable)
                    {
                        break;
                    }

                    _gameLogic.PossibleMove.Add(SpawnTiles.TilesName[i, j]);
                }
                break;
        }


    }

    private void CheckTileAvaliable_Player(int i,int x,int y,bool checkPlayer)
    {
       
        // need player position
        switch (i)
        {
            case 0:
                //false 
                if(checkPlayer && _gameLogic.FindIsPlayerInPosition(x,y+1))
                {
                    break;
                }
                if (checkPlayer && _gameLogic.tempTile[x, y + 1].isAvaliable)
                {
                    break;
                }
                if (!checkPlayer && !_gameLogic.FindIsPlayerInPosition(x, y + 1))
                {
                    break;
                }
                if (y + 2 < MaxSize && !_gameLogic.FindIsPlayerInPosition(x, y + 2) && _gameLogic.tempTile[x, y + 2].isAvaliable)
                {
                    _gameLogic.PossibleMove.Add(SpawnTiles.TilesName[x, y + 2]);
                }
                break;
            case 1:
                if (checkPlayer && _gameLogic.FindIsPlayerInPosition(x, y - 1))
                {
                    break;
                }
                if (checkPlayer && _gameLogic.tempTile[x, y - 1].isAvaliable)
                {
                    break;
                }
                if (!checkPlayer && !_gameLogic.FindIsPlayerInPosition(x, y - 1))
                {
                    break;
                }
                if (y - 2 > -1 && !_gameLogic.FindIsPlayerInPosition(x, y - 2) && _gameLogic.tempTile[x, y - 2].isAvaliable)
                {
                    _gameLogic.PossibleMove.Add(SpawnTiles.TilesName[x, y - 2]);
                }
                break;
            case 2:
                if (checkPlayer && _gameLogic.FindIsPlayerInPosition(x+1, y ))
                {
                    break;
                }
                if (checkPlayer && _gameLogic.tempTile[x+1, y ].isAvaliable)
                {
                    break;
                }
                if (!checkPlayer && !_gameLogic.FindIsPlayerInPosition(x+1, y ))
                {
                    break;
                }
                if (x + 2 < MaxSize && !_gameLogic.FindIsPlayerInPosition( x+2, y ) && _gameLogic.tempTile[x+2, y].isAvaliable)
                {
                    _gameLogic.PossibleMove.Add(SpawnTiles.TilesName[x + 2, y]);
                }
                break;
            case 3:
                if (checkPlayer && _gameLogic.FindIsPlayerInPosition(x - 1 , y))
                {
                    break;
                }
                if (checkPlayer && _gameLogic.tempTile[x-1, y ].isAvaliable)
                {
                    break;
                }
                if (!checkPlayer && !_gameLogic.FindIsPlayerInPosition(x-1, y ))
                {
                    break;
                }
                if (x - 2 > -1 && !_gameLogic.FindIsPlayerInPosition(x-2, y) && _gameLogic.tempTile[x-2, y].isAvaliable)
                {
                    _gameLogic.PossibleMove.Add(SpawnTiles.TilesName[x - 2, y]);
                }
                break;
            case 4:
                if (checkPlayer && _gameLogic.FindIsPlayerInPosition(x + 1 , y + 1))
                {
                    break;
                }
                if (checkPlayer && _gameLogic.tempTile[x + 1, y + 1].isAvaliable)
                {
                    break;
                }
                if (!checkPlayer && !_gameLogic.FindIsPlayerInPosition(x + 1, y + 1))
                {
                    break;
                }
                if (x + 2 < MaxSize && y + 2 < MaxSize && !_gameLogic.FindIsPlayerInPosition(x+2, y + 2) && _gameLogic.tempTile[x+2, y + 2].isAvaliable)
                {
                    _gameLogic.PossibleMove.Add(SpawnTiles.TilesName[x + 2, y + 2]);
                }
                break;
            case 5:
                if (checkPlayer && _gameLogic.FindIsPlayerInPosition(x + 1, y - 1))
                {
                    break;
                }
                if (checkPlayer && _gameLogic.tempTile[x + 1, y - 1].isAvaliable)
                {
                    break;
                }
                if (!checkPlayer && !_gameLogic.FindIsPlayerInPosition(x + 1, y - 1))
                {
                    break;
                }
                if (x + 2 < MaxSize && y - 2 > -1 && !_gameLogic.FindIsPlayerInPosition(x+2, y - 2) && _gameLogic.tempTile[x+2, y - 2].isAvaliable)
                {
                    _gameLogic.PossibleMove.Add(SpawnTiles.TilesName[x + 2, y - 2]);
                }
                break;
            case 6:
                if (checkPlayer && _gameLogic.FindIsPlayerInPosition(x - 1, y + 1))
                {
                    break;
                }
                if (checkPlayer && _gameLogic.tempTile[x - 1, y + 1].isAvaliable)
                {
                    break;
                }
                if (!checkPlayer && !_gameLogic.FindIsPlayerInPosition(x - 1, y + 1))
                {
                    break;
                }
                if (x - 2 > -1 && y + 2 < MaxSize && !_gameLogic.FindIsPlayerInPosition(x-2, y + 2) && _gameLogic.tempTile[x-2, y + 2].isAvaliable)
                {
                    _gameLogic.PossibleMove.Add(SpawnTiles.TilesName[x - 2, y + 2]);
                }
                break;
            case 7:
                if (checkPlayer && _gameLogic.FindIsPlayerInPosition(x -  1, y - 1))
                {
                    break;
                }
                if (checkPlayer && _gameLogic.tempTile[x - 1, y - 1].isAvaliable)
                {
                    break;
                }
                if (!checkPlayer && !_gameLogic.FindIsPlayerInPosition(x - 1, y - 1))
                {
                    break;
                }
                if (x - 2 > -1 && y - 2 > -1 && !_gameLogic.FindIsPlayerInPosition(x-2, y - 2) && _gameLogic.tempTile[x -2, y - 2].isAvaliable)
                {
                    _gameLogic.PossibleMove.Add(SpawnTiles.TilesName[x - 2, y - 2]);
                }
                break;
        }


    }
    public void OnClick_RevealTiles(GameObject Obj)
    {
        Tuple<int, int> temp = FindTile(Obj.name);
        //is any players in position
        if(_gameLogic.FindIsPlayerInPosition(temp.Item1, temp.Item2))
        {
            Debug.Log("There is Player on CLick");
            return;
        }
        //is tile avaliable 
        if(!_gameLogic.tempTile[temp.Item1,temp.Item2].isAvaliable || (_gameLogic.tempTile[temp.Item1, temp.Item2].isAvaliable && _gameLogic.tempTile[temp.Item1, temp.Item2].color== string.Empty && _gameLogic.tempTile[temp.Item1, temp.Item2].Tiletype == string.Empty))
        {
            return;
        }

        CreateHighlight.Instance.HighlightAllowedMove(Obj, _gameLogic.tempTile[temp.Item1, temp.Item2].color, _gameLogic.tempTile[temp.Item1, temp.Item2].Tiletype);
        UseCount++;

        if(UseCount==3)
        {
            UseCount = 0;
            RevealCard = false;
            _playerController._isAllowedClick = false;
            //call EndTurn / clear Highlights
            StartCoroutine(EndTurnWithDelay());
        }

    }

    public void OnClick_PickupTile(GameObject Obj)
    {
        Tuple<string, string> tempCnT = _gameLogic.F(Obj.name);
        Tuple<int, int> tempPos = FindTile(Obj.name);
        if (tempCnT.Item1 != (string)PhotonNetwork.LocalPlayer.CustomProperties["color"])
        {
            Debug.Log("Not same color as player");
            return;
        }
        PickUpCard = false;
        _playerController._isAllowedClick = false;
        Debug.Log("Pickup Tiletype " + tempCnT.Item2);
        //update player and room prop
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable[tempCnT.Item2] = (int)PhotonNetwork.LocalPlayer.CustomProperties[tempCnT.Item2] + 1;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);

        _gameLogic.tempTile[tempPos.Item1, tempPos.Item2].color = string.Empty;
        _gameLogic.tempTile[tempPos.Item1, tempPos.Item2].Tiletype = string.Empty;
        _gameLogic.tempTile[tempPos.Item1, tempPos.Item2].isAvaliable = false;
        //find obj that color is same as player

        _gameLogic.UpdateRoomData();

        StartCoroutine(EndTurnWithDelay());
    }

    private IEnumerator EndTurnWithDelay()
    {
        yield return new WaitForSeconds(2f);
        CreateHighlight.Instance.DestroyHighlight();
        yield return new WaitForSeconds(1f);
        _stateManager.EndTurn();

    }

    private Tuple<int, int> FindTile(string n)
    {
        for (int i = 0; i < MaxSize; i++)
        {
            for (int j = 0; j < MaxSize; j++)
            {
                if (SpawnTiles.TilesName[i, j].name == n)
                {
                    return Tuple.Create(i, j);
                }
            }
        }
        return null;
    }

    


}
