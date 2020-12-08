using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public struct Tileinfo
    {
        public GameObject obj;
        public string color;
        public bool isAvaliable;
        public string Tiletype;

    }

    public List<GameObject> PossibleMove, RevealedTiles;
    public List<int> RandomList, ListToCheck;
    public List<GameObject> PortalList, TempList;

    public int NumberOfPlayers = 2;
    public int CurrentPlayerId = 0;
    public int nextPlayerId;
    public MyPlayer[] myPlayer;

    private string[] rngcolor, rngTiletype;
    public Tileinfo[,] Tiles = new Tileinfo[6, 6];
    public Material def_mats, new_mats;
    public GameObject[] playerObjects;
    UpdateGui updateGui;
    GameLogic gameLogic;

    
    void Awake()
    {

        rngcolor = new string[] { "red", "yellow" };
        rngTiletype = new string[] { "home", "water", "rice", "tree" };

        List<string> typerandom = new List<string> { "wind","wind","wind" ,"wind","wind","wind"  ,"portal","portal","portal","portal","swap","swap"};
        int index;

        int a = 0;
        int b = 0;
        int c = 0;
        int t = 0;
        int i = 0;
  
        foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("Boards").OrderBy(obj => obj.name, new AlphanumComparatorFast()).ToArray())
        {

            //Tiles[i,j].color = rngcolor[Random.Range(0,rngcolor.Length)];
            if (b > 5)
            {

                a++;
                b = 0;

            }

            Tiles[a, b].obj = Obj;
           
        if(Random.value <0.5f | typerandom.Count == 0)
        {
                if (t > 2)
                {
                    t = 0;
                    c++;
                }
                if (c > 3)
                {
                    i++;
                    c = 0;
                }
                Tiles[a, b].color = rngcolor[i];
                Tiles[a, b].Tiletype = rngTiletype[t];
                t++;
        }
        else
        {
             
            index = Random.Range(0, typerandom.Count);
            Tiles[a, b].Tiletype = typerandom[index];
           
            typerandom.RemoveAt(index);
        }

            Tiles[a, b].isAvaliable = true;
            b++;

        }


    }

  

    void Start()
    {
        updateGui = FindObjectOfType<UpdateGui>();
        gameLogic = FindObjectOfType<GameLogic>();
        playerObjects = new GameObject[NumberOfPlayers];

        playerObjects[0] = GameObject.Find("Player3");
        playerObjects[1] = GameObject.Find("Player1");

        myPlayer = new MyPlayer[NumberOfPlayers];
        myPlayer[0] = new MyPlayer();
        myPlayer[1] = new AIPlayer();

     
        myPlayer[0].isAI = false;
        myPlayer[1].isAI = true;
        myPlayer[0].color = "red";
        myPlayer[1].color = "yellow";
        myPlayer[0].x = 0;
        myPlayer[0].y = 0;
        myPlayer[1].x = 0;
        myPlayer[1].y = 5;

        myPlayer[0].Inventory.waters = 3;
        myPlayer[1].Inventory.rices = 3;

        RevealedTiles = new List<GameObject>();
        nextPlayerId = (CurrentPlayerId + 1) % NumberOfPlayers;
        ListToCheck = new List<int> { 4, 8, 10 };
        PortalList = new List<GameObject>();
        TempList = new List<GameObject>();
    }



    // NOTE: enum / statemachine is probably a stronger choice, but I'm aiming for simpler to explain.
    public bool IsDoneRolling = false;
    public bool IsDoneClicking = false;
    public bool IsDoneAnimating = false;
    public bool IsDonePopup = false;
    public bool isDoneWaited = false;
    private bool gameOver = false;
    public bool RevealCard = false;
    public bool canPickupCard = false;
    public bool hasSameColor = false;
    public bool hasPortalOpen = false;
    private bool needToShowPopUP = true;
    public bool showAroundTile = false;
    public void NewTurn()
    {
        
        if (myPlayer[CurrentPlayerId].IsWinner())
        {
            updateGui.showWinner(CurrentPlayerId);
            gameOver = true;
            return;
        }


        //Debug.Log("NewTurn");
        // This is the start of a player's turn.
        // We don't have a roll for them yet.
        isDoneWaited = false;
        IsDoneRolling = false;
        IsDoneAnimating = false;
        IsDonePopup = false;
        canPickupCard = false;
        hasSameColor = false;
        RevealCard = false;
        hasPortalOpen = false;
        needToShowPopUP = true;
        showAroundTile = false;
        CurrentPlayerId = (CurrentPlayerId + 1) % NumberOfPlayers;
        nextPlayerId = (CurrentPlayerId + 1) % NumberOfPlayers;
        if (!myPlayer[CurrentPlayerId].isAI)
        {
            StartCoroutine(updateGui.showyourTurn());
        }
        if (myPlayer[CurrentPlayerId].isAI)
        {
            StartCoroutine(BotWaitForStart());
        }

        FindNextOneMove();
        bool hasUseableCard = false;
        if (PossibleMove.Count == 0)
        {
            if (myPlayer[CurrentPlayerId].GetInventoryCount() == 0)
            {
                gameOver = true;
            }
            foreach (int i in ListToCheck)
            {
                if (myPlayer[CurrentPlayerId].FindInventoryCard(i))
                {
                    hasUseableCard = true;
                }
            }
            if (!hasUseableCard)
            {
                gameOver = true;
            }
            //check card 
            //check tiles
            Debug.Log("Player" + CurrentPlayerId + "Lose");
            if (gameOver)
            {
                updateGui.showWinner(CurrentPlayerId);
            }

        }
        PossibleMove.Clear();
       if(myPlayer[CurrentPlayerId].isProtected)
        {
            myPlayer[CurrentPlayerId].isProtected = false;
        }


    }

    private void Update()
    {
        if (gameOver == true)
        {
            return;
        }

        // Is the turn done?
        if (IsDoneRolling && IsDoneAnimating && IsDonePopup)
        {
            Debug.Log("Turn is done!");
            NewTurn();
            return;
        }


        if (myPlayer[CurrentPlayerId].isAI)
        {
            myPlayer[CurrentPlayerId].DoAI();
        }



    }
    public void AineedWait()
    {
        StartCoroutine(BotWaitForStart());
    }
    public IEnumerator BotWaitForStart()
    {
        yield return new WaitForSeconds(1.5f);
        isDoneWaited = true;

    }
    public void UpdatePosition(string n)
    {

        StartCoroutine(DoUpdatePosition(n));


    }
    private IEnumerator DoUpdatePosition(string n)
    {
        while (IsDoneAnimating == false)
        {
            yield return null;
        };

        int found_i = -1;
        int found_j = -1;
        int found_c = -1;
        for (int i = 0; i < 6 && found_i < 0; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (Tiles[i, j].obj.name == n)
                {
                    if(myPlayer[CurrentPlayerId].isAI)
                    {
                        Debug.Log("Ai has possible " + PossibleMove.Count);
                    }
                    if(Tiles[i,j].Tiletype == "swap")
                    {
                        BoardHighlighter.Instance.HighlightAllowedSingleObject(Tiles[i, j].obj, "swap");
                    }
                    if (Tiles[i, j].Tiletype == "wind")
                    {
                        showAroundTile = true;
                        BoardHighlighter.Instance.HighlightAllowedSingleObject(Tiles[i, j].obj, "wind");
                       
                    }
                    if (Tiles[i, j].Tiletype == "portal")
                    {
                        Debug.Log("portal status" + hasPortalOpen);
                        if (hasPortalOpen)
                        {
                            hasPortalOpen = false;
                            PortalList.Clear();
                            PortalList.AddRange(TempList);
                            needToShowPopUP = false;
                            if(!myPlayer[CurrentPlayerId].isAI)
                            { 
                                StartCoroutine(needwait());
                            }

                        }
                        else
                        {
                            gameLogic.OpenPortal(Tiles[i, j].obj);
                        }
                    }
                    if (!myPlayer[CurrentPlayerId].isAI && !canPickupCard)
                    {

                        if (needToShowPopUP)
                        {

                            StartCoroutine(updateGui.ShowPopUp(Tiles[i, j].color, Tiles[i, j].Tiletype));
                        }

                    }

                    if (Tiles[i, j].color == myPlayer[CurrentPlayerId].color)
                    {

                        changeTexture(Tiles[i, j].obj, 1);
                        playerObjects[CurrentPlayerId].GetComponent<Animator>().SetTrigger("Wave");

                        //+ Score
           
                        UpdateScore(Tiles[i, j].color, Tiles[i, j].Tiletype);
                        if (canPickupCard)
                        {
                            IsDoneClicking = false;
                            updateGui.ShowAllMenu();
                            RevealedTiles.Clear();
                            StartCoroutine(needwait());
                            found_c = 0;

                            canPickupCard = false;
                        }

                        Tiles[i, j].isAvaliable = false;
                        Tiles[i, j].color = "";

                    }

                    found_i = i;
                    found_j = j;


                    break;
                }
            }
        }
        if (found_c != 0)
        {
            myPlayer[CurrentPlayerId].x = found_i;
            myPlayer[CurrentPlayerId].y = found_j;

            if (showAroundTile)
            {
                FindNextOneMove();
                Tiles[found_i, found_j].Tiletype = "null";
               
            }
            if(Tiles[found_i,found_j].Tiletype == "swap")
            {
                Tiles[found_i, found_j].Tiletype = "null";
                gameLogic.SwapChar();
            }
        }


        Debug.Log("update postion done");

    }
    private void changeTexture(GameObject obj, int type)
    {
        MeshRenderer m_renderer = obj.GetComponent<MeshRenderer>();
        m_renderer.material = new_mats;
        switch (type)
        {
            case 0:
                m_renderer.material = def_mats;
                break;
            case 1:
                m_renderer.material = new_mats;
                break;
        }
    }
    private void UpdateScore(string c, string t)
    {
        switch (t)
        {
            case "home":
                myPlayer[CurrentPlayerId].Inventory.home += 1;
                break;
            case "rice":
                myPlayer[CurrentPlayerId].Inventory.rices += 1;
                break;
            case "tree":
                myPlayer[CurrentPlayerId].Inventory.trees += 1;
                break;
            case "water":
                myPlayer[CurrentPlayerId].Inventory.waters += 1;
                break;
        }
        updateGui.ChangeScore(c, t);

    }

    public void FindNextDirection()
    {
        RandomList = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };

        if (myPlayer[CurrentPlayerId].x == 0)
        {
            RandomList.Remove(3);
            RandomList.Remove(6);
            RandomList.Remove(7);
            //cant go down
            //cant go down right
        }
        if (myPlayer[CurrentPlayerId].y == 0)
        {
            RandomList.Remove(1);
            RandomList.Remove(7);
            RandomList.Remove(5);
            //cant go left
            //cant go up left
        }
        if (myPlayer[CurrentPlayerId].x == 5)
        {
            RandomList.Remove(2);
            RandomList.Remove(4);
            RandomList.Remove(5);
            //can't go up
            //can't go up left
            //cant' go up right
        }
        if (myPlayer[CurrentPlayerId].y == 5)
        {
            RandomList.Remove(4);
            RandomList.Remove(0);
            RandomList.Remove(6);
            //cant go right
            //cant go upright
            //cant go down right
        }
    }

    public void FindNextOneMove()
    {
        PossibleMove.Clear();
        int x = myPlayer[CurrentPlayerId].x;
        int y = myPlayer[CurrentPlayerId].y;
        FindNextDirection();

        foreach (int i in RandomList)
        {

            switch (i)
            {
                case 0:
                    if (!Tiles[x, y + 1].isAvaliable | (x == myPlayer[nextPlayerId].x && y + 1 == myPlayer[nextPlayerId].y))
                    {
                        continue;
                    }
                    if (canPickupCard)
                    {
                        if (Tiles[x, y + 1].color == myPlayer[CurrentPlayerId].color)
                        {
                            hasSameColor = true;
                        }
                    }
                    if (showAroundTile)
                    {
                        BoardHighlighter.Instance.HighlightAllowedMove(Tiles[x, y + 1].obj, Tiles[x, y + 1].color, Tiles[x, y + 1].Tiletype);
                    }
                    else
                    {
                        PossibleMove.Add(Tiles[x, y + 1].obj);
                    }
                    break;
                case 1:
                    if (!Tiles[x, y - 1].isAvaliable | (x == myPlayer[nextPlayerId].x && y - 1 == myPlayer[nextPlayerId].y))
                    {
                        continue;
                    }
                    if (canPickupCard)
                    {
                        if (Tiles[x, y - 1].color == myPlayer[CurrentPlayerId].color)
                        {
                            hasSameColor = true;
                        }
                    }
                    if (showAroundTile)
                    {
                        BoardHighlighter.Instance.HighlightAllowedMove(Tiles[x, y - 1].obj, Tiles[x, y - 1].color, Tiles[x, y - 1].Tiletype);
                    }
                    else
                    {
                        PossibleMove.Add(Tiles[x, y - 1].obj);
                    }
                    break;
                case 2:

                    if (!Tiles[x + 1, y].isAvaliable | (x + 1 == myPlayer[nextPlayerId].x && y == myPlayer[nextPlayerId].y))
                    {
                        continue;
                    }
                    if (canPickupCard)
                    {
                        if (Tiles[x + 1, y].color == myPlayer[CurrentPlayerId].color)
                        {
                            hasSameColor = true;
                        }
                    }
                    if (showAroundTile)
                    {
                        BoardHighlighter.Instance.HighlightAllowedMove(Tiles[x + 1, y].obj, Tiles[x + 1, y].color, Tiles[x + 1, y].Tiletype);
                    }
                    else
                    {
                        PossibleMove.Add(Tiles[x + 1, y].obj);
                    }
                    break;
                case 3:
                    if (!Tiles[x - 1, y].isAvaliable | (x - 1 == myPlayer[nextPlayerId].x && y == myPlayer[nextPlayerId].y))
                    {
                        continue;
                    }
                    if (canPickupCard)
                    {
                        if (Tiles[x - 1, y].color == myPlayer[CurrentPlayerId].color)
                        {
                            hasSameColor = true;
                        }
                    }
                    if (showAroundTile)
                    {
                        BoardHighlighter.Instance.HighlightAllowedMove(Tiles[x - 1, y].obj, Tiles[x - 1, y].color, Tiles[x - 1, y].Tiletype);
                    }
                    else
                    {
                        PossibleMove.Add(Tiles[x - 1, y].obj);
                    }
                    break;
                case 4:
                    if (!Tiles[x + 1, y + 1].isAvaliable | (x + 1 == myPlayer[nextPlayerId].x && y + 1 == myPlayer[nextPlayerId].y))
                    {
                        continue;
                    }
                    if (canPickupCard)
                    {
                        if (Tiles[x + 1, y + 1].color == myPlayer[CurrentPlayerId].color)
                        {
                            hasSameColor = true;
                        }
                    }
                    if (showAroundTile)
                    {
                        BoardHighlighter.Instance.HighlightAllowedMove(Tiles[x + 1, y + 1].obj, Tiles[x + 1, y + 1].color, Tiles[x + 1, y + 1].Tiletype);
                    }
                    else
                    {
                        PossibleMove.Add(Tiles[x + 1, y + 1].obj);
                    }
                    break;
                case 5:
                    if (!Tiles[x + 1, y - 1].isAvaliable | (x + 1 == myPlayer[nextPlayerId].x && y - 1 == myPlayer[nextPlayerId].y))
                    {
                        continue;
                    }
                    if (canPickupCard)
                    {
                        if (Tiles[x + 1, y - 1].color == myPlayer[CurrentPlayerId].color)
                        {
                            hasSameColor = true;
                        }
                    }
                    if (showAroundTile)
                    {
                        BoardHighlighter.Instance.HighlightAllowedMove(Tiles[x + 1, y - 1].obj, Tiles[x + 1, y - 1].color, Tiles[x + 1, y - 1].Tiletype);
                    }
                    else
                    {
                        PossibleMove.Add(Tiles[x + 1, y - 1].obj);
                    }
                    break;
                case 6:
                    if (!Tiles[x - 1, y + 1].isAvaliable | (x - 1 == myPlayer[nextPlayerId].x && y + 1 == myPlayer[nextPlayerId].y))
                    {
                        continue;
                    }
                    if (canPickupCard)
                    {
                        if (Tiles[x - 1, y + 1].color == myPlayer[CurrentPlayerId].color)
                        {
                            hasSameColor = true;
                        }
                    }
                    if (showAroundTile)
                    {
                        BoardHighlighter.Instance.HighlightAllowedMove(Tiles[x - 1, y + 1].obj, Tiles[x - 1, y + 1].color, Tiles[x - 1, y + 1].Tiletype);
                    }
                    else
                    {
                        PossibleMove.Add(Tiles[x - 1, y + 1].obj);
                    }
                    break;
                case 7:
                    if (!Tiles[x - 1, y - 1].isAvaliable | (x - 1 == myPlayer[nextPlayerId].x && y - 1 == myPlayer[nextPlayerId].y))
                    {
                        continue;
                    }
                    if (canPickupCard)
                    {
                        if (Tiles[x - 1, y - 1].color == myPlayer[CurrentPlayerId].color)
                        {
                            hasSameColor = true;
                        }
                    }
                    if (showAroundTile)
                    {
                        BoardHighlighter.Instance.HighlightAllowedMove(Tiles[x - 1, y - 1].obj, Tiles[x - 1, y - 1].color, Tiles[x - 1, y - 1].Tiletype);
                    }
                    else
                    {
                        PossibleMove.Add(Tiles[x - 1, y - 1].obj);
                    }
                    break;
            }

        }



        if(showAroundTile)
        {
            Debug.Log("Wind list  Random " + RandomList.Count + "possible" + PossibleMove.Count);
            StartCoroutine(needwait());
            showAroundTile = false;
        }


    }

    public void FindUnAvaliable()
    {
        PossibleMove.Clear();
        int x = myPlayer[CurrentPlayerId].x;
        int y = myPlayer[CurrentPlayerId].y;
        FindNextDirection();

        foreach (int i in RandomList)
        {
            switch (i)
            {
                case 0:
                    if (Tiles[x, y + 1].isAvaliable)
                    {
                        return;
                    }
                    PossibleMove.Add(Tiles[x, y + 1].obj);
                    break;
                case 1:
                    if (Tiles[x, y - 1].isAvaliable)
                    {
                        return;
                    }
                    PossibleMove.Add(Tiles[x, y - 1].obj);
                    break;
                case 2:
                    if (Tiles[x + 1, y].isAvaliable)
                    {
                        return;
                    }
                    PossibleMove.Add(Tiles[x + 1, y].obj);
                    break;
                case 3:
                    if (Tiles[x - 1, y].isAvaliable)
                    {
                        return;
                    }
                    PossibleMove.Add(Tiles[x - 1, y].obj);
                    break;
                case 4:
                    if (Tiles[x + 1, y + 1].isAvaliable)
                    {
                        return;
                    }
                    PossibleMove.Add(Tiles[x + 1, y + 1].obj);
                    break;
                case 5:
                    if (Tiles[x + 1, y - 1].isAvaliable)
                    {
                        return;
                    }
                    PossibleMove.Add(Tiles[x + 1, y - 1].obj);
                    break;
                case 6:
                    if (Tiles[x - 1, y + 1].isAvaliable)
                    {
                        return;
                    }
                    PossibleMove.Add(Tiles[x - 1, y + 1].obj);
                    break;
                case 7:
                    if (Tiles[x - 1, y - 1].isAvaliable)
                    {
                        return;
                    }
                    PossibleMove.Add(Tiles[x - 1, y - 1].obj);
                    break;
            }

        }



    }

    private void SetTilesAvaliable(string name)
    {

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (Tiles[i, j].obj.name == name)
                {
                    Tiles[i, j].isAvaliable = true;
                    break;
                }
            }
        }

    }

    public void RevealTiles(string name)
    {

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (Tiles[i, j].obj.name == name)
                {
                    if (i == myPlayer[CurrentPlayerId].x && j == myPlayer[CurrentPlayerId].y)
                    {
                        break;
                    }
                    if (i == myPlayer[nextPlayerId].x && j == myPlayer[nextPlayerId].y)
                    {
                        break;
                    }
                    if (Tiles[i, j].isAvaliable && Tiles[i, j].color != "")
                    {
                        RevealedTiles.Add(Tiles[i, j].obj);
                        BoardHighlighter.Instance.HighlightAllowedMove(Tiles[i, j].obj, Tiles[i, j].color, Tiles[i, j].Tiletype);
                        //call create Prefab on tiles
                        Debug.Log("found" + Tiles[i, j].color);
                    }

                    break;
                }
            }
        }

    }
    public void PickUp()
    {
        canPickupCard = true;
        FindNextOneMove();

        foreach (GameObject obj in PossibleMove)
        {
            RevealTiles(obj.name);
        }
        Debug.Log("possible move " + PossibleMove);
        if (hasSameColor)
        {

            IsDoneAnimating = true;
            IsDoneClicking = true;
        }
        else
        {
            IsDoneAnimating = true;
            StartCoroutine(needwait());
        }

    }
    public void ShowCard(GameObject obj)
    {
        if (RevealedTiles.Count == 2)
        {
            //End
            RevealCard = false;
            IsDoneClicking = false;
            updateGui.ShowAllMenu();
            RevealedTiles.Clear();
            IsDoneAnimating = true;
            StartCoroutine(needwait());
        }
        if (!RevealedTiles.Contains(obj))
        {

            RevealTiles(obj.name);
        }


    }
    public IEnumerator needwait()
    {
        yield return new WaitForSeconds(2f);
        BoardHighlighter.Instance.DestroyHighlights();
        IsDonePopup = true;

    }

    public void SetAvaliable(GameObject obj, int type)
    {
        SetTilesAvaliable(obj.name);
        changeTexture(obj, 0);
        //create texture on tiles
    }

    public void FindNextMove()
    {
        PossibleMove.Clear();
        int x = myPlayer[CurrentPlayerId].x;
        int y = myPlayer[CurrentPlayerId].y;
        FindNextDirection();


        // inside a methode of the same class
        var ran = RandomList[Random.Range(0, RandomList.Count)];
        Debug.Log("RND" + ran);

        myPlayer[CurrentPlayerId].rnd = ran;

        //go right
        if (ran == 0)
        {
            //hightligh possible move tiles
            //Possible move
            for (int j = y + 1; j < 6; j++)
            {
                if (!Tiles[x, j].isAvaliable | (x == myPlayer[nextPlayerId].x && j == myPlayer[nextPlayerId].y))
                {
                    return;
                }
                PossibleMove.Add(Tiles[x, j].obj);

            }
            return;

        }
        // go left
        if (ran == 1)
        {
            for (int j = y - 1; j > -1; j--)
            {
                if (!Tiles[x, j].isAvaliable | (x == myPlayer[nextPlayerId].x && j == myPlayer[nextPlayerId].y))
                {
                    return;
                }
                PossibleMove.Add(Tiles[x, j].obj);
            }
            return;
        }
        //up
        if (ran == 2)
        {
            for (int i = x + 1; i < 6; i++)
            {

                if (!Tiles[i, y].isAvaliable | (i == myPlayer[nextPlayerId].x && y == myPlayer[nextPlayerId].y))
                {
                    return;
                }

                PossibleMove.Add(Tiles[i, y].obj);
            }
            return;
        }
        //down
        if (ran == 3)
        {
            for (int i = x - 1; i > -1; i--)
            {
                if (!Tiles[i, y].isAvaliable | (i == myPlayer[nextPlayerId].x && y == myPlayer[nextPlayerId].y))
                {
                    return;
                }
                PossibleMove.Add(Tiles[i, y].obj);
            }
            return;
        }
        // up right
        if (ran == 4)
        {
            for (int i = x + 1, j = y + 1; i < 6 && j < 6; i++, j++)
            {
                if (!Tiles[i, j].isAvaliable | (i == myPlayer[nextPlayerId].x && j == myPlayer[nextPlayerId].y))
                {
                    return;
                }
                PossibleMove.Add(Tiles[i, j].obj);

            }
            return;
        }
        //up left
        if (ran == 5)
        {
            for (int i = x + 1, j = y - 1; i < 6 && j > -1; i++, j--)
            {
                if (!Tiles[i, j].isAvaliable | (i == myPlayer[nextPlayerId].x && j == myPlayer[nextPlayerId].y))
                {
                    return;
                }
                PossibleMove.Add(Tiles[i, j].obj);

            }
            return;
        }
        //down right
        if (ran == 6)
        {
            for (int i = x - 1, j = y + 1; i > -1 && j < 6; i--, j++)
            {
                if (!Tiles[i, j].isAvaliable | (i == myPlayer[nextPlayerId].x && j == myPlayer[nextPlayerId].y))
                {
                    return;
                }
                PossibleMove.Add(Tiles[i, j].obj);

            }
            return;
        }

        //down left
        if (ran == 7)
        {
            for (int i = x - 1, j = y - 1; i > -1 && j > -1; i--, j--)
            {
                if (!Tiles[i, j].isAvaliable | (i == myPlayer[nextPlayerId].x && j == myPlayer[nextPlayerId].y))
                {
                    return;
                }
                PossibleMove.Add(Tiles[i, j].obj);
            }
            return;
        }

    }


}


