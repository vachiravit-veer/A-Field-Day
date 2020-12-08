using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;
using ProtoBuf.Meta;

public class OnlineGameLogic : MonoBehaviour
{

    private List<int> RandomList;
    protected Player _localPlayer;
    public List<GameObject> PossibleMove;

    protected string[] _tempX;
    protected string[] _tempY;


    public Entity[,] tempTile;

    protected int x, y;

    public List<int[]> _playersPos;
    protected List<int[]> _portalList = new List<int[]>();
    public static bool needToTeleport = false;
    public static bool Onclick_swapPlayer = false;
    public static bool OnClick_SpecialTile = false;
    protected GUIController _gui;
    protected OnlineStateManager _stateManager;
    protected PlayerController _Controller;
    private bool IsTurnStarted = false;
    protected int swapmode;

    protected GameObject[] _PlayersGO;

    protected int MaxSize;

    private Vector3[] _preposition;
    private string[] _preName;
    private int[,] _preMaxtrixPosition;
    private PhotonView photonView;
    private void Awake()
    {
        _preposition = new Vector3[] { new Vector3(3.25f, 2.55f, 4.44f), new Vector3(-3.7f, 2.55f, 4.44f), new Vector3(3.27f, 2.55f, -2.82f), new Vector3(-3.7f, 2.55f, -2.82f) };
        _preName = new string[] { "block_1", "block_8", "block_57", "block_64" };
        _preMaxtrixPosition = new int[,] { { 0,0} , { 0, 7 }, { 7, 0 },{ 7,7 } };
    }

    private void Start()
    {
        _stateManager = FindObjectOfType<OnlineStateManager>();
        _localPlayer = PhotonNetwork.LocalPlayer;
        _gui = FindObjectOfType<GUIController>();
        _Controller = FindObjectOfType<PlayerController>();
        MaxSize = (int)PhotonNetwork.CurrentRoom.CustomProperties["maxSize"];
        photonView = PhotonView.Get(this);


        Screen.orientation = ScreenOrientation.LandscapeLeft;

    }

    // start - check if can it move anywhere Run FindLegalOneMove()
    // UseTile
    //
    // Rearrange State
    // change it to check use tile
    // create Function set avaliable and change texture to orginals

    private int CountPlayerTile()
    {
        return ((int)_localPlayer.CustomProperties["home"] + (int)_localPlayer.CustomProperties["rice"] + (int)_localPlayer.CustomProperties["tree"] + (int)_localPlayer.CustomProperties["water"]);
    }

    public void CheckAtStartTurn()
    {
        if (IsTurnStarted)
            return;

          IsTurnStarted = true;

          FindLegalOneMove(-1, -1,true);
          if(PossibleMove.Count==0)
          {
            if (CountPlayerTile() > 0)
            {
                //check card 
                CheckUnAvaliable();
                PossibleMove.ForEach(x => Debug.Log("Unavaliable " + x.name));
                //use inventory
                _gui.ShowTileInventory();
                return;
            }

            int[] tempcards = (int[])_localPlayer.CustomProperties["cards"];
            if (tempcards.Where(x => x == 4 || x == 8 || x == 10).Any())
            {
                _gui.showCardInventory();
                return;
            }

            //this player lost
            // show gui you lost then kick outta room

            _gui.OnLose();
            _stateManager.EndTurn();

        }
        else
        {
            IsTurnStarted = false;
            _gui.ShowPlayerController();
        }
    }

    public void EndCheckAtStart()
    {
        IsTurnStarted = false;
        _gui.HideTileInventory();
        UpdateRoomData();
        _stateManager.EndTurn();
    }

    public void GetAllPlayersPosition(bool itself)
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            return;
        }

        _playersPos = new List<int[]>();
     
        foreach (KeyValuePair <int,Player> p in PhotonNetwork.CurrentRoom.Players)
        {
            if (p.Value.IsLocal && itself)
                continue;

            int[] temp = new int[2];
            temp[0] = (int)p.Value.CustomProperties["x"];
            temp[1] = (int)p.Value.CustomProperties["y"];

            _playersPos.Add(temp);
        }
 
    }

    public void PopulateTempTile()
    {
        Entity[] result = ProtobufManager.Deserialize<Entity[]>((byte[])PhotonNetwork.CurrentRoom.CustomProperties["TilesData"]);
        tempTile = ProtoArray<Entity>.ToArray(result);
    }

    public bool FindIsPlayerInPosition(int x,int y)
    {
       
        if (_playersPos == null)
            return false;
        if (_playersPos.Count == 0)
            return false;

        foreach(int[] _list in _playersPos)
        {
            //search users row by row
            if (_list[0] == x && _list[1] == y)
            {               
                return true;
            }
 
        }

        return false;
    }

    public List<int> FindDir(int x,int y)
    {

        PopulateTempTile();

        if (x == -1 && y == -1)
        {
            x = (int)_localPlayer.CustomProperties["x"];
            y = (int)_localPlayer.CustomProperties["y"];
        }

     //   Debug.Log("Player pos : " + x + "," + y);

        RandomList = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };

        if (x == 0)
        {
            RandomList.Remove(3);
            RandomList.Remove(6);
            RandomList.Remove(7);

        }
        if (y == 0)
        {
            RandomList.Remove(1);
            RandomList.Remove(7);
            RandomList.Remove(5);
   
        }
        if (x == MaxSize-1)
        {
            RandomList.Remove(2);
            RandomList.Remove(4);
            RandomList.Remove(5);
  
        }
        if (y == MaxSize-1)
        {
            RandomList.Remove(4);
            RandomList.Remove(0);
            RandomList.Remove(6);
 
        }

        return RandomList;
     

    }

    public void FindLegalMove(bool NeedCleanUp, int ran, int max)
    {

        GetAllPlayersPosition(true);
        if(NeedCleanUp)     
            PossibleMove.Clear();

        x = (int)_localPlayer.CustomProperties["x"];
        y = (int)_localPlayer.CustomProperties["y"];
        //set max 4,6
        //set min -4,-1                6 5 4 3         
        //go right
        if (ran == 0)
        {
            //hightligh possible move tiles
            //Possible move
            for (int j = y + 1, min=0; j < MaxSize && min<max; j++,min++)
            {
               // 
                if (!tempTile[x, j].isAvaliable | FindIsPlayerInPosition(x,j))
                {
                    return;
                }
                
                PossibleMove.Add(SpawnTiles.TilesName[x, j]);
             

            }
            return;

        }
        // go left
        if (ran == 1)
        {
            for (int j = y - 1, min = 0; j > -1 && min < max; j--, min++)
            {
                if (!tempTile[x, j].isAvaliable | FindIsPlayerInPosition(x,j))
                {
                    return;
                }
                PossibleMove.Add(SpawnTiles.TilesName[x, j]);
            }
            return;
        }
        //up
        if (ran == 2)
        {
            for (int i = x + 1, min=0; i < MaxSize && min < max; i++,min++)
            {

                if (!tempTile[i, y].isAvaliable | FindIsPlayerInPosition(i, y))
                {
                    return;
                }

                PossibleMove.Add(SpawnTiles.TilesName[i, y]);
            }
            return;
        }
        //down
        if (ran == 3)
        {
            for (int i = x - 1, min = 0; i > -1 && min < max; i--, min++)
            {
                if (!tempTile[i, y].isAvaliable | FindIsPlayerInPosition(i, y))
                {
                    return;
                }
                PossibleMove.Add(SpawnTiles.TilesName[i, y]);
            }
            return;
        }
        // up right
        if (ran == 4)
        {
            for (int i = x + 1, j = y + 1,min=0; i < MaxSize && j < MaxSize && min < max; i++, j++, min++)
            {
                if (!tempTile[i, j].isAvaliable | FindIsPlayerInPosition(i, j))
                {
                    return;
                }
                PossibleMove.Add(SpawnTiles.TilesName[i, j]);

            }
            return;
        }
        //up left
        if (ran == 5)
        {
            for (int i = x + 1, j = y - 1,min=0; i < MaxSize && j > -1 && min < max; i++, j--, min++)
            {
                if (!tempTile[i, j].isAvaliable | FindIsPlayerInPosition(i, j))
                {
                    return;
                }
                PossibleMove.Add(SpawnTiles.TilesName[i, j]);

            }
            return;
        }
        //down right
        if (ran == 6)
        {
            for (int i = x - 1, j = y + 1, min=0; i > -1 && j < MaxSize && min < max; i--, j++, min++)
            {
                if (!tempTile[i, j].isAvaliable | FindIsPlayerInPosition(i, j))
                {
                    return;
                }
                PossibleMove.Add(SpawnTiles.TilesName[i, j]);

            }
            return;
        }

        //down left
        if (ran == 7)
        {
            for (int i = x - 1, j = y - 1, min=0; i > -1 && j > -1 && min < max; i--, j--, min++)
            {
                if (!tempTile[i, j].isAvaliable | FindIsPlayerInPosition(i, j))
                {
                    return;
                }
                PossibleMove.Add(SpawnTiles.TilesName[i, j]);
            }
            return;
        }

     

    }

    public void FindLegalOneMove(int x, int y,bool itself)
    {
        GetAllPlayersPosition(itself);
        PossibleMove.Clear();
        if (x == -1 && y == -1)
        {
            x = (int)_localPlayer.CustomProperties["x"];
            y = (int)_localPlayer.CustomProperties["y"];
        }

        List<int> dir = FindDir(x, y);

        foreach (int i in dir)
        {
            switch (i)
            {
                case 0:
                    if (!tempTile[x, y + 1].isAvaliable | FindIsPlayerInPosition(x, y+1))
                    {
                        continue;
                    }
                    PossibleMove.Add(SpawnTiles.TilesName[x, y + 1]);
                    break;
                case 1:
                    if (!tempTile[x, y - 1].isAvaliable | FindIsPlayerInPosition(x, y -1))
                    {
                        continue;
                    }
                    else
                    {
                        PossibleMove.Add(SpawnTiles.TilesName[x, y - 1]);
                    }
                    break;
                case 2:

                    if (!tempTile[x + 1, y].isAvaliable | FindIsPlayerInPosition(x+1, y))
                    {
                        continue;
                    }
                    else
                    {
                        PossibleMove.Add(SpawnTiles.TilesName[x + 1, y]);
                    }
                    break;
                case 3:
                    if (!tempTile[x - 1, y].isAvaliable | FindIsPlayerInPosition(x-1, y))
                    {
                        continue;
                    }
                    else
                    {
                        PossibleMove.Add(SpawnTiles.TilesName[x - 1, y]);
                    }
                    break;
                case 4:
                    if (!tempTile[x + 1, y + 1].isAvaliable | FindIsPlayerInPosition(x+1, y+1))
                    {
                        continue;
                    }
                    else
                    {
                        PossibleMove.Add(SpawnTiles.TilesName[x + 1, y + 1]);
                    }
                    break;
                case 5:
                    if (!tempTile[x + 1, y - 1].isAvaliable | FindIsPlayerInPosition(x+1, y-1))
                    {
                        continue;
                    }
                    else
                    {
                        PossibleMove.Add(SpawnTiles.TilesName[x + 1, y - 1]);
                    }
                    break;
                case 6:
                    if (!tempTile[x - 1, y + 1].isAvaliable | FindIsPlayerInPosition(x-1, y+1))
                    {
                        continue;
                    }
                    else
                    {
                        PossibleMove.Add(SpawnTiles.TilesName[x - 1, y + 1]);
                    }
                    break;
                case 7:
                    if (!tempTile[x - 1, y - 1].isAvaliable | FindIsPlayerInPosition(x-1, y-1))
                    {
                        continue;
                    }
                    else
                    {
                        PossibleMove.Add(SpawnTiles.TilesName[x - 1, y - 1]);
                    }
                    break;
            }
        }

    }

    public Tuple<string, string> F(string n)
    {
        
        for (int i = 0; i < MaxSize; i++)
        {
            for (int j = 0; j < MaxSize; j++)
            {
                if (SpawnTiles.TilesName[i, j].name == n)
                {
                    return Tuple.Create(tempTile[i, j].color, tempTile[i, j].Tiletype);
                }
            }
        }
        return null;
    }

    public virtual Tuple<int, int, int> SetFound(string n)
    {
        for (int i = 0; i < MaxSize; i++)
        {
            for (int j = 0; j < MaxSize; j++)
            {
                if (SpawnTiles.TilesName[i, j].name == n)
                {

                    Debug.Log("Found " + tempTile[i, j].color + " type " + tempTile[i, j].Tiletype);

                    if (MaxSize == 8)
                    {
                        if (_preName.Contains(n))
                            return Tuple.Create(i, j, 2);
                    }

                    if (tempTile[i, j].color != string.Empty &&  tempTile[i, j].Tiletype != string.Empty)
                       _gui.ShowPickUpPanel(tempTile[i, j].color, tempTile[i, j].Tiletype);
  
                    if (tempTile[i, j].Tiletype == "portal")
                    {
                            
                        if(needToTeleport)
                        {
                            needToTeleport = false;
                            return Tuple.Create(i, j, 0);
                        }
                        if(!_portalList.Where(x=>x[0]==i&&x[1]==j).Any())
                        {
                            Debug.Log("Added" + SpawnTiles.TilesName[i, j].name);
                            photonView.RPC("HighlightSceneObject", RpcTarget.MasterClient, SpawnTiles.TilesName[i, j].transform.position, tempTile[i, j].color, tempTile[i, j].Tiletype);
                            int[] tc = new int[2];
                            tc[0] = i;
                            tc[1] = j;
                            _portalList.Add(tc);
                            Debug.Log("Total " + _portalList.Count);
                                                      
                            photonView.RPC("SetAll", RpcTarget.All, ObjectSerializer.Serialize(_portalList));
                        }

                        if(_portalList.Count==1) { return Tuple.Create(i, j, 0); }

                        if (_portalList.Count > 0)
                        {
                            
                            StartCoroutine(_gui.ShowMessage("คลิกที่หมากตัวตุ่นเพื่อย้ายไปยังตำแหน่งที่ต้องการ"));


                            PossibleMove.Clear();
                            _portalList.Where(s => !(s[0] == i && s[1] == j)).ToList().ForEach(y => PossibleMove.Add(SpawnTiles.TilesName[y[0], y[1]]));
                                
                            PossibleMove.ForEach(x => Debug.Log("item in list" + x.name));
                            needToTeleport = true;
                        }
                        //bool playcanclick and bool to check if portal open
                        _Controller._isAllowedClick = true;
                        //if Only One portal open do nothing
                        //PortalList add

                        return Tuple.Create(i, j, 3);
                    }
                     
                    if (tempTile[i, j].Tiletype == "swapPosition" || tempTile[i, j].Tiletype == "swapCharacter")
                    {
                        CreateHighlight.Instance.HighlightAllowedMove(SpawnTiles.TilesName[i, j], tempTile[i, j].color,tempTile[i, j].Tiletype);

                        if (tempTile[i, j].Tiletype == "swapPosition")
                            swapmode = 0;
                        else
                            swapmode = 1;

                        tempTile[i, j].Tiletype = string.Empty;
                        UpdateRoomData();
                        _gui.ShowSwapText();
                        _Controller._isAllowedClick = true;
                        Onclick_swapPlayer = true;
                        return Tuple.Create(i, j, 3);

                    }

                    if (tempTile[i, j].Tiletype == "wind")
                    {
                      
                        photonView.RPC("HighlightSceneObject", RpcTarget.MasterClient, SpawnTiles.TilesName[i, j].transform.position, tempTile[i, j].color, tempTile[i, j].Tiletype);
                        FindLegalOneMove(i, j,false);
                        tempTile[i, j].Tiletype = string.Empty;
                        PossibleMove.ForEach(x => CreateHighlight.Instance.HighlightAllowedMove(x, F(x.name)));
                        UpdateRoomData();

                        return Tuple.Create(i, j, 2);
                     }
     
                    if (tempTile[i, j].Tiletype == "rain")
                    {
                       // photonView.RPC("HighlightSceneObject", RpcTarget.MasterClient, SpawnTiles.TilesName[i, j].transform.position, tempTile[i, j].color, tempTile[i, j].Tiletype);

                        FindLegalOneMove(i, j, false);
                        PossibleMove.ForEach(x => photonView.RPC("HighlightSceneObject", RpcTarget.MasterClient, x.transform.position, F(x.name).Item1, F(x.name).Item2));
                        tempTile[i, j].Tiletype = string.Empty;

                        return Tuple.Create(i, j, 1);
                    }

                    if (tempTile[i, j].Tiletype == "start")
                    {

                        CreateHighlight.Instance.HighlightAllowedMove(SpawnTiles.TilesName[i, j], tempTile[i, j].color,tempTile[i, j].Tiletype);
                        Debug.Log("Found Go Back " + tempTile[i, j].color + "to start");
                        
                        //Find player that has same color
                        //Find player tag object
                        GoBackAtStart(tempTile[i, j].color);

                        tempTile[i, j].Tiletype = string.Empty;
                        tempTile[i, j].color = string.Empty;

                        UpdateRoomData();
                        //move object to postion //preinit position block name
                        //end turn
                        return Tuple.Create(i, j, 3);
                    }

                    if (tempTile[i, j].Tiletype == "grass")
                    {

                        _localPlayer.CustomProperties[tempTile[i, j].Tiletype] = (int)_localPlayer.CustomProperties[tempTile[i, j].Tiletype] + 1;
                        tempTile[i, j].color = string.Empty;
                        tempTile[i, j].Tiletype = string.Empty;
                        tempTile[i, j].isAvaliable = false;
                        ChangeTextureEvent(SpawnTiles.TilesName[i, j].name, 1);

                        return Tuple.Create(i, j, 1);
                    }

                    if (OnClick_SpecialTile)
                    {
                        Debug.Log("Tile status :  " + tempTile[i, j].isAvaliable);
                        Hashtable ht = new Hashtable();
                        ht["grass"] = (int)_localPlayer.CustomProperties["grass"] - 1;
                        _localPlayer.SetCustomProperties(ht);
                        tempTile[i, j].isAvaliable = true;
                        ChangeTextureEvent(SpawnTiles.TilesName[i, j].name, 0);
                        OnClick_SpecialTile = false;
                      
                        return Tuple.Create(i, j, 1);
                    }

                    if (tempTile[i, j].color != string.Empty && (string)_localPlayer.CustomProperties["color"] == tempTile[i, j].color)
                    {
                       
                        _localPlayer.CustomProperties[tempTile[i, j].Tiletype] = (int)_localPlayer.CustomProperties[tempTile[i, j].Tiletype] + 1;
                        tempTile[i, j].color = string.Empty;
                        tempTile[i, j].Tiletype = string.Empty;
                        tempTile[i, j].isAvaliable = false;
                        ChangeTextureEvent(SpawnTiles.TilesName[i, j].name, 1);                  

                        return Tuple.Create(i, j, 1);
                    }
                    return Tuple.Create(i, j,0);
                }
            }
        }
        return null;
    }

    public void CheckUnAvaliable()
    {
        int x = (int)_localPlayer.CustomProperties["x"];
        int y = (int)_localPlayer.CustomProperties["y"];
        
        //maybe cache this
        List<int> dir = FindDir(x, y);
        foreach (int i in dir)
        {
            switch (i)
            {
                case 0:
                    if (tempTile[x, y + 1].isAvaliable | FindIsPlayerInPosition(x, y + 1))
                    {
                        continue;
                    }
                    PossibleMove.Add(SpawnTiles.TilesName[x, y + 1]);
                    break;
                case 1:
                    if (tempTile[x, y - 1].isAvaliable | FindIsPlayerInPosition(x, y - 1))
                    {
                        continue;
                    }
                    else
                    {
                        PossibleMove.Add(SpawnTiles.TilesName[x, y - 1]);
                    }
                    break;
                case 2:
                    if (tempTile[x + 1, y].isAvaliable | FindIsPlayerInPosition(x + 1, y))
                    {
                        continue;
                    }
                    else
                    {
                        PossibleMove.Add(SpawnTiles.TilesName[x + 1, y]);
                    }
                    break;
                case 3:
                    if (tempTile[x - 1, y].isAvaliable | FindIsPlayerInPosition(x - 1, y))
                    {
                        continue;
                    }
                    else
                    {
                        PossibleMove.Add(SpawnTiles.TilesName[x - 1, y]);
                    }
                    break;
                case 4:
                    if (tempTile[x + 1, y + 1].isAvaliable | FindIsPlayerInPosition(x + 1, y + 1))
                    {
                        continue;
                    }
                    else
                    {
                        PossibleMove.Add(SpawnTiles.TilesName[x + 1, y + 1]);
                    }
                    break;
                case 5:
                    if (tempTile[x + 1, y - 1].isAvaliable | FindIsPlayerInPosition(x + 1, y - 1))
                    {
                        continue;
                    }
                    else
                    {
                        PossibleMove.Add(SpawnTiles.TilesName[x + 1, y - 1]);
                    }
                    break;
                case 6:
                    if (tempTile[x - 1, y + 1].isAvaliable | FindIsPlayerInPosition(x - 1, y + 1))
                    {
                        continue;
                    }
                    else
                    {
                        PossibleMove.Add(SpawnTiles.TilesName[x - 1, y + 1]);
                    }
                    break;
                case 7:
                    if (tempTile[x - 1, y - 1].isAvaliable | FindIsPlayerInPosition(x - 1, y - 1))
                    {
                        continue;
                    }
                    else
                    {
                        PossibleMove.Add(SpawnTiles.TilesName[x - 1, y - 1]);
                    }
                    break;
            }
        }

    }

    public void SetAvaliableAndTexture(string n)
    {
        for (int i = 0; i < MaxSize; i++)
        {
            for (int j = 0; j < MaxSize; j++)
            {
                if (SpawnTiles.TilesName[i, j].name == n)
                {
                    PossibleMove.Remove(SpawnTiles.TilesName[i, j]);
                    Debug.Log("Set avaliable" + n);
                    tempTile[i, j].isAvaliable = true;
                    ChangeTextureEvent(SpawnTiles.TilesName[i, j].name, 0);
                    return;
                }
            }
        }
        return;
    }

    public IEnumerator UpdatePosition(string n)
    {
        // 0 = do nothing , 1 = update room Prop , 2 = Destory Highlights , 3 = don't Endturn yet
        var result = SetFound(n);
        yield return new WaitForSeconds(0.5f);
        if (result!=null)
        {
            Hashtable ht = new Hashtable();
            ht["x"] = result.Item1;
            ht["y"] = result.Item2;
            _localPlayer.SetCustomProperties(ht);
            Debug.Log("update" + PhotonNetwork.LocalPlayer.NickName + " To " + result.Item1 + "," + result.Item2);
        }

        if(result.Item3==1)
            UpdateRoomData();

        yield return new WaitForSeconds(0.5f);


        if (result.Item3 == 2)
        {
            yield return new WaitForSeconds(1f);
            CreateHighlight.Instance.DestroyHighlight();
        }
        
        if (result.Item3 != 3)
             _stateManager.EndTurn();
             
    }

    public void UpdateRoomData()
    {
        Hashtable prop = new Hashtable();
        prop.Add("TilesData", ProtobufManager.Serialize(tempTile));
        PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
    }

    protected void ChangeTextureEvent(string name,int i)
    {
        object[] content = new object[] { name, i };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent((byte)0, content, raiseEventOptions, sendOptions);
    }

    public void DoSwap(GameObject Obj)
    {
        if (swapmode == 0)
            SwapPosition(Obj);
        else
            SwapPlayer(Obj);
    }

    private void SwapPosition(GameObject Obj)
    {
        PhotonView ObjView = Obj.GetComponent<PhotonView>();
        Player ObjOwner = ObjView.Owner;
        if (ObjOwner == _localPlayer)
        {
            Debug.Log("Obj is same owner as local");
            return;
        }

        int x = (int)ObjOwner.CustomProperties["x"];
        int y = (int)ObjOwner.CustomProperties["y"];
        int xx= (int)_localPlayer.CustomProperties["x"];
        int yy = (int)_localPlayer.CustomProperties["y"];

        Vector3 target = SpawnTiles.TilesName[x, y].transform.position;
        Vector3 original = SpawnTiles.TilesName[xx, yy].transform.position;

        StartCoroutine(MoveToPosition(_localPlayer.TagObject as GameObject, target, 5f, null));
        
        Hashtable ht = new Hashtable();
        ht["x"] = x;
        ht["y"] = y;
        _localPlayer.SetCustomProperties(ht);

        //

        ht.Clear();

        ht["x"] = xx;
        ht["y"] = yy;

        ObjOwner.SetCustomProperties(ht);

        StartCoroutine(MoveToPosition(Obj, original, 5f, ObjView));

    }
 
    public void SwapPlayer(GameObject Obj)
    {
        Player ObjOwner = Obj.GetComponent<PhotonView>().Owner;
        if (ObjOwner == _localPlayer)
        {
            Debug.Log("Obj is same owner as local");
            return;
        }

         if ((bool)ObjOwner.CustomProperties["protected"])
         {
             Debug.Log("Player is procted");
             return;
         }

        _PlayersGO = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("List of" + _PlayersGO[0].name + "," + _PlayersGO[1].name);

        Hashtable ht = new Hashtable();
        Hashtable ht2 = new Hashtable();

        ht["color"] = _localPlayer.CustomProperties["color"];
        ht["x"] = _localPlayer.CustomProperties["x"];
        ht["y"] = _localPlayer.CustomProperties["y"];
        ht["home"] = _localPlayer.CustomProperties["home"];
        ht["rice"] = _localPlayer.CustomProperties["rice"];
        ht["tree"] = _localPlayer.CustomProperties["tree"];
        ht["water"] = _localPlayer.CustomProperties["water"];

        ht2["color"] = ObjOwner.CustomProperties["color"];
        ht2["x"] = ObjOwner.CustomProperties["x"];
        ht2["y"] = ObjOwner.CustomProperties["y"];
        ht2["home"] = ObjOwner.CustomProperties["home"];
        ht2["rice"] = ObjOwner.CustomProperties["rice"];
        ht2["tree"] = ObjOwner.CustomProperties["tree"];
        ht2["water"] = ObjOwner.CustomProperties["water"];

        ObjOwner.SetCustomProperties(ht);
        _localPlayer.SetCustomProperties(ht2);

        ht.Clear();
        ht2.Clear();


        GameObject go2 = _PlayersGO.Where(x => x.GetComponent<PhotonView>().Owner == ObjOwner).FirstOrDefault();
        Debug.Log("before " + go2.GetComponent<PhotonView>().OwnerActorNr);
        go2.GetComponent<PhotonView>().TransferOwnership(_localPlayer);
        Debug.Log("After " + go2.GetComponent<PhotonView>().OwnerActorNr);
        GameObject go3 = _localPlayer.TagObject as GameObject;
        Debug.Log("Host before " + go3.GetComponent<PhotonView>().OwnerActorNr);
        go3.GetComponent<PhotonView>().TransferOwnership(ObjOwner);
        Debug.Log("Host After " + go3.GetComponent<PhotonView>().OwnerActorNr);

        _localPlayer.TagObject = go2;

        Debug.Log("Request Player is" + _localPlayer.NickName + "Owner player is" + ObjOwner.NickName);

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("SetTagObject", RpcTarget.All);

        GameObject.FindGameObjectsWithTag("Player").ToList().ForEach(x => Debug.Log("Player " + x.name + "object owner is" + x.GetComponent<PhotonView>().OwnerActorNr));

        CreateHighlight.Instance.DestroyHighlight();

        Onclick_swapPlayer = false;
        _gui.HideSwapText();
        // refresh UI
        _stateManager.EndTurn();
    }

    private void GoBackAtStart(string color)
    {
        Player _owner;
        GameObject go;

        int id;

        if (color == (string)PhotonNetwork.LocalPlayer.CustomProperties["color"])
        {           
            _owner = PhotonNetwork.LocalPlayer;
             go = _owner.TagObject as GameObject;
            id = (int)_owner.CustomProperties["customID"];
            StartCoroutine(_Controller.MoveMe(_preposition[id], 5f, _preName[id],1f));
            return;
        }
        else
        {
            _PlayersGO = GameObject.FindGameObjectsWithTag("Player");
             go = null;

            for (int i=0; i<_PlayersGO.Length;i++)
            {
                if((string)_PlayersGO[i].GetComponent<PhotonView>().Owner.CustomProperties["color"] == color)
                {
                    Debug.Log("Found it");
                     go = _PlayersGO[i];
                    break;
                }
            }

            if (go == null)
            {
                Debug.Log("Can't find Player object");
                _stateManager.EndTurn();
                //maybe player leave or  lose
                return;
            }

            _owner = go.GetComponent<PhotonView>().Owner;
            id = (int)_owner.CustomProperties["customID"];
            if ((int)_owner.CustomProperties["x"] == _preMaxtrixPosition[id,0] && (int)_owner.CustomProperties["y"] == _preMaxtrixPosition[id, 1])
            {
                Debug.Log("Target Player is Already in starting position");
                _stateManager.EndTurn();
                return;
            }

        }

        Debug.Log("Found" + go.name);
        //move object to postion //preinit position block name
        StartCoroutine(MoveMe(go, _preposition[id], 5f, _preName[id]));
    }

    private IEnumerator UpdatePosition(string n, Player owner)
    {
        // 0 = do nothing , 1 = update room Prop , 2 = Destory Highlights , 3 = don't Endturn yet
        var result = SetFound(n);
        yield return new WaitForSeconds(0.5f);
        if (result != null)
        {
            Hashtable ht = new Hashtable();
            ht["x"] = result.Item1;
            ht["y"] = result.Item2;
            owner.SetCustomProperties(ht);
            Debug.Log("update" + owner.NickName + " To " + result.Item1 + "," + result.Item2);
        }

        if (result.Item3 == 1)
            UpdateRoomData();

        yield return new WaitForSeconds(0.5f);


        if (result.Item3 == 2)
        {
            yield return new WaitForSeconds(1f);
            CreateHighlight.Instance.DestroyHighlight();
        }

        if (result.Item3 != 3)
            _stateManager.EndTurn();

    }

    private IEnumerator MoveMe(GameObject go, Vector3 b, float speed, string name)
    {
        
        Player tempOwner = go.GetComponent<PhotonView>().Owner;
        go.GetComponent<PhotonView>().TransferOwnership(_localPlayer);

        yield return new WaitForSeconds(1f);
        go.transform.LookAt(b);
        go.GetComponent<Animator>().SetFloat("MoveSpeed", 999);
        Vector3 a = go.transform.position;
        float step = (speed / (a - b).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step; // Goes from 0 to 1, incrementing by step each time
            go.transform.position = Vector3.Lerp(a, b, t); // Move objectToMove closer to b
            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }
        go.transform.position = b;
        go.GetComponent<Animator>().SetFloat("MoveSpeed", 0);

        //transfer back
        go.GetComponent<PhotonView>().TransferOwnership(tempOwner);

        StartCoroutine(UpdatePosition(name, go.GetComponent<PhotonView>().Owner));

    }

    private IEnumerator MoveToPosition(GameObject go, Vector3 b, float speed, PhotonView goView)
    {

        Player tempOwner = null;
        if (goView != null)
        {
             goView = go.GetComponent<PhotonView>();
             tempOwner = goView.Owner;
             goView.TransferOwnership(_localPlayer); 
        }

        yield return new WaitForSeconds(1f);
        go.transform.LookAt(b);
        go.GetComponent<Animator>().SetFloat("MoveSpeed", 999);
        Vector3 a = go.transform.position;
        float step = (speed / (a - b).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step; // Goes from 0 to 1, incrementing by step each time
            go.transform.position = Vector3.Lerp(a, b, t); // Move objectToMove closer to b
            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }
        go.transform.position = b;
        go.GetComponent<Animator>().SetFloat("MoveSpeed", 0);

        //transfer back
        if (goView != null)
        {
            goView.TransferOwnership(tempOwner);
            yield return new WaitForSeconds(0.5f);
            CreateHighlight.Instance.DestroyHighlight();
            Onclick_swapPlayer = false;
            _gui.HideSwapText();
            _stateManager.EndTurn();

        }
       
    }

    public void FindAllUnavaliable()
    {
        PossibleMove.Clear();
        PopulateTempTile();
        for (int i = 0; i < MaxSize; i++)
        {
            for (int j = 0; j < MaxSize; j++)
            {
                if (!tempTile[i, j].isAvaliable)
                {
                    PossibleMove.Add(SpawnTiles.TilesName[i, j]);
                }
            }
        }

    }

    [PunRPC]
    void SetAll(byte[] data, PhotonMessageInfo info)
    {      
        _portalList = (List<int[]>)ObjectSerializer.Deserialize(data);
    }

    [PunRPC]
    void SetTagObject(PhotonMessageInfo info)
    {
        _localPlayer = PhotonNetwork.LocalPlayer;
        Debug.Log("caller" + _localPlayer.NickName);
        GameObject playerObject = _localPlayer.TagObject as GameObject;
        if (!playerObject.GetComponent<PhotonView>().IsMine)
        {
            GameObject go = GameObject.FindGameObjectsWithTag("Player").ToList().Where(x => x.GetComponent<PhotonView>().Owner == _localPlayer).FirstOrDefault();
            if(go!=null)
                _localPlayer.TagObject = go;
        }


    }





}
