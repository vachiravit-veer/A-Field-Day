using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private void Start()
    {
        stateManager = FindObjectOfType<StateManager>();
        updateGui = FindObjectOfType<UpdateGui>();

    }

    StateManager stateManager;
    UpdateGui updateGui;
    private int x, y, nextx, nexty;

    public void SelectedCard(int c, GameObject button)
    {
        bool hasToCheckMove = false;
        x = stateManager.myPlayer[stateManager.CurrentPlayerId].x;
        y = stateManager.myPlayer[stateManager.CurrentPlayerId].y;
        nextx = stateManager.myPlayer[stateManager.nextPlayerId].x;
        nexty = stateManager.myPlayer[stateManager.nextPlayerId].y;

        switch (c)
        {
            case 1:
                card1();
                hasToCheckMove = true;
                break;
            case 2:
                card2();
                break;
            case 3:
                card3();
                break;
            case 4:
                card4();
                hasToCheckMove = true;
                break;
            case 5:
                card5();
                hasToCheckMove = true;
                break;
            case 6:
                card6();
                hasToCheckMove = true;
                break;
            case 7:
                card7();
                break;
            case 8:
                card8();
                hasToCheckMove = true;
                break;
            case 10:
                card10();
                hasToCheckMove = true;
                break;
            case 11:
                card11();
                hasToCheckMove = true;
                break;
            case 12:
                card7();

                break;
        }

        //check if still no move
        if (hasToCheckMove && stateManager.PossibleMove.Count == 0)
        {
            //no move
            //stateManager.IsDoneRolling= true;
            return;
        }
        else
        {
            stateManager.IsDoneRolling = true;
            updateGui.hideCardInventory();
            button.SetActive(false);
            stateManager.IsDoneClicking = true;
            if (hasToCheckMove)
            {
                BoardHighlighter.Instance.HighlightAllowedMoves(stateManager.PossibleMove);
            }
        }

    }

    public void OpenPortal(GameObject n)
    {
        Debug.Log("open portal" + stateManager.hasPortalOpen);

        if (!stateManager.PortalList.Contains(n))
        {
            stateManager.PortalList.Add(n);
            BoardHighlighter.Instance.HighlightAllowedSingleObject(n, "portal");
        }

        if (stateManager.PortalList.Count > 1)
        {
            stateManager.IsDoneClicking = true;
            stateManager.hasPortalOpen = true;
            stateManager.TempList.AddRange(stateManager.PortalList);
            stateManager.PortalList.Remove(n);

        }


        //create tiles on portrallist
        //set boolean in update
        // if bool = true
        // hit in PortalList
        //move to Select item in Portral

    }

    public void SwapChar()
    {
        if (stateManager.myPlayer[stateManager.nextPlayerId].isProtected)
        {
            Debug.Log("Player " + stateManager.myPlayer[stateManager.nextPlayerId] + "is procted");
            return;
        }
        int tempx, tempy;
        int temprice, temphome, tempwater, temptree;
        string tempcolor;
        GameObject tempobj;

        //tempIsAI = stateManager.myPlayer[stateManager.CurrentPlayerId].isAI;
       // stateManager.myPlayer[stateManager.CurrentPlayerId].isAI = stateManager.myPlayer[stateManager.nextPlayerId].isAI;
        //stateManager.myPlayer[stateManager.nextPlayerId].isAI = tempIsAI;


        tempcolor = stateManager.myPlayer[stateManager.CurrentPlayerId].color;
        stateManager.myPlayer[stateManager.CurrentPlayerId].color = stateManager.myPlayer[stateManager.nextPlayerId].color;
        stateManager.myPlayer[stateManager.nextPlayerId].color = tempcolor;
        Debug.Log("now " + stateManager.myPlayer[stateManager.CurrentPlayerId].color + "before" + stateManager.myPlayer[stateManager.nextPlayerId].color);

        tempobj = stateManager.playerObjects[stateManager.CurrentPlayerId];
        stateManager.playerObjects[stateManager.CurrentPlayerId] = stateManager.playerObjects[stateManager.nextPlayerId];
        stateManager.playerObjects[stateManager.nextPlayerId] = tempobj;


        tempx = stateManager.myPlayer[stateManager.CurrentPlayerId].x;
        tempy = stateManager.myPlayer[stateManager.CurrentPlayerId].y;
        temprice = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.rices;
        temphome = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.home;
        temptree = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.trees;
        tempwater = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.waters;

        stateManager.myPlayer[stateManager.CurrentPlayerId].x = stateManager.myPlayer[stateManager.nextPlayerId].x;
        stateManager.myPlayer[stateManager.CurrentPlayerId].y = stateManager.myPlayer[stateManager.nextPlayerId].y;

        stateManager.myPlayer[stateManager.nextPlayerId].x = tempx;
        stateManager.myPlayer[stateManager.nextPlayerId].y = tempy;

        stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.rices = stateManager.myPlayer[stateManager.nextPlayerId].Inventory.rices;
        stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.waters = stateManager.myPlayer[stateManager.nextPlayerId].Inventory.waters;
        stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.trees = stateManager.myPlayer[stateManager.nextPlayerId].Inventory.trees;
        stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.home = stateManager.myPlayer[stateManager.nextPlayerId].Inventory.home;
       

        stateManager.myPlayer[stateManager.nextPlayerId].Inventory.rices = temprice;
        stateManager.myPlayer[stateManager.nextPlayerId].Inventory.waters = tempwater;
        stateManager.myPlayer[stateManager.nextPlayerId].Inventory.trees = temptree;
        stateManager.myPlayer[stateManager.nextPlayerId].Inventory.home = temphome;
        updateGui.RefreashGuiScore();
        Debug.Log("swap player ");


        //swap score
        //swap player
        //swap x,y
    }


    public void card1()
    {
        stateManager.PossibleMove.Clear();
        stateManager.FindNextDirection();


        foreach (int step in stateManager.RandomList)
        {

            if (step == 0)
            {
                //j=y+1 1 2 3 j<y+4
                //y=0 
                // y-1 4 3 2 y-4
                //j>y-4
                for (int j = y + 1; j < y + 4 && j < 6; j++)
                {

                    if (!stateManager.Tiles[x, j].isAvaliable | (x == nextx && j == nexty))
                    {
                        break;
                    }

                    stateManager.PossibleMove.Add(stateManager.Tiles[x, j].obj);

                }
                continue;

            }
            // go left
            if (step == 1)
            {
                for (int j = y - 1; j > y - 4 && j > -1; j--)
                {
                    if (!stateManager.Tiles[x, j].isAvaliable | (x == nextx && j == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[x, j].obj);
                }
                continue;
            }
            //up
            if (step == 2)
            {
                for (int i = x + 1; i < x + 4 && i < 6; i++)
                {

                    if (!stateManager.Tiles[i, y].isAvaliable | (i == nextx && y == nexty))
                    {
                        break;
                    }

                    stateManager.PossibleMove.Add(stateManager.Tiles[i, y].obj);
                }
                continue;
            }
            //down
            if (step == 3)
            {
                for (int i = x - 1; i > x - 4 && i > -1; i--)
                {
                    if (!stateManager.Tiles[i, y].isAvaliable | (i == nextx && y == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[i, y].obj);
                }
                continue;
            }
            // up right
            if (step == 4)
            {
                for (int i = x + 1, j = y + 1; i < x + 4 && j < y + 4 && i < 6 && j < 6; i++, j++)
                {
                    if (!stateManager.Tiles[i, j].isAvaliable | (i == nextx && j == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[i, j].obj);

                }

                continue;
            }
            //up left
            if (step == 5)
            {
                for (int i = x + 1, j = y - 1; i < x + 4 && j > y - 4 && i < 6 && j > -1; i++, j--)
                {
                    if (!stateManager.Tiles[i, j].isAvaliable | (i == nextx && j == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[i, j].obj);

                }
                continue;
            }
            //down right
            if (step == 6)
            {
                for (int i = x - 1, j = y + 1; i > x - 4 && j < x + 4 && i > -1 && j < 6; i--, j++)
                {
                    if (!stateManager.Tiles[i, j].isAvaliable | (i == nextx && j == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[i, j].obj);

                }
                continue;
            }

            //down left
            if (step == 7)
            {
                for (int i = x - 1, j = y - 1; i > x - 4 && j > y - 4 && i > -1 && j > -1; i--, j--)
                {
                    if (!stateManager.Tiles[i, j].isAvaliable | (i == nextx && j == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[i, j].obj);
                }
                continue;
            }

        }


    }

    public void card2()
    {
        stateManager.PickUp();

    }

    public void card3()
    {
        stateManager.myPlayer[stateManager.CurrentPlayerId].isProtected = true;
    }

    public void card4()
    {
        stateManager.PossibleMove.Clear();
        stateManager.FindNextDirection();

        List<GameObject> temp = new List<GameObject>();
        bool isChecked = false;


        foreach (int ran in stateManager.RandomList)
        {
            Debug.Log("card4" + stateManager.PossibleMove.Count);
            isChecked = false;
            temp.Clear();

            if (ran == 0)
            {

                for (int j = y + 1; j < 6; j++)
                {
                    if ((x == nextx && j == nexty))
                    {
                        break;
                    }
                    if (!stateManager.Tiles[x, j].isAvaliable)
                    {
                        isChecked = true;
                        continue;
                    }

                    temp.Add(stateManager.Tiles[x, j].obj);

                }
                if (isChecked)
                {
                    stateManager.PossibleMove.AddRange(temp);
                }
                continue;
            }
            // go left
            if (ran == 1)
            {
                for (int j = y - 1; j > -1; j--)
                {
                    if (x == nextx && j == nexty)
                    {
                        break;
                    }
                    if (!stateManager.Tiles[x, j].isAvaliable)
                    {
                        isChecked = true;
                        continue;
                    }

                    temp.Add(stateManager.Tiles[x, j].obj);
                }
                if (isChecked)
                {
                    stateManager.PossibleMove.AddRange(temp);
                }
                continue;
            }
            //up
            if (ran == 2)
            {
                for (int i = x + 1; i < 6; i++)
                {
                    if (i == nextx && y == nexty)
                    {
                        break;
                    }
                    if (!stateManager.Tiles[i, y].isAvaliable)
                    {
                        isChecked = true;
                        continue;
                    }
                    temp.Add(stateManager.Tiles[i, y].obj);
                }
                if (isChecked)
                {
                    stateManager.PossibleMove.AddRange(temp);
                }
                continue;
            }
            //down
            if (ran == 3)
            {
                for (int i = x - 1; i > -1; i--)
                {
                    if (i == nextx && y == nexty)
                    {
                        break;
                    }
                    if (!stateManager.Tiles[i, y].isAvaliable)
                    {
                        isChecked = true;
                        continue;
                    }


                    temp.Add(stateManager.Tiles[i, y].obj);
                }
                if (isChecked)
                {
                    stateManager.PossibleMove.AddRange(temp);
                }
                continue;
            }
            // up right
            if (ran == 4)
            {
                for (int i = x + 1, j = y + 1; i < 6 && j < 6; i++, j++)
                {
                    if (i == nextx && j == nexty)
                    {
                        break;
                    }
                    if (!stateManager.Tiles[i, j].isAvaliable)
                    {
                        isChecked = true;
                        continue;
                    }


                    temp.Add(stateManager.Tiles[i, j].obj);

                }
                if (isChecked)
                {
                    stateManager.PossibleMove.AddRange(temp);
                }
                continue;

            }
            //up left
            if (ran == 5)
            {
                for (int i = x + 1, j = y - 1; i < 6 && j > -1; i++, j--)
                {
                    if (i == nextx && j == nexty)
                    {
                        break;
                    }
                    if (!stateManager.Tiles[i, j].isAvaliable)
                    {
                        isChecked = true;
                        continue;
                    }


                    temp.Add(stateManager.Tiles[i, j].obj);

                }
                if (isChecked)
                {
                    stateManager.PossibleMove.AddRange(temp);
                }
                continue;
            }
            //down right
            if (ran == 6)
            {
                for (int i = x - 1, j = y + 1; i > -1 && j < 6; i--, j++)
                {
                    if (i == nextx && j == nexty)
                    {
                        break;
                    }
                    if (!stateManager.Tiles[i, j].isAvaliable)
                    {
                        isChecked = true;
                        continue;
                    }


                    temp.Add(stateManager.Tiles[i, j].obj);

                }
                if (isChecked)
                {
                    stateManager.PossibleMove.AddRange(temp);
                }
                continue;
            }

            //down left
            if (ran == 7)
            {
                for (int i = x - 1, j = y - 1; i > -1 && j > -1; i--, j--)
                {
                    if (i == nextx && j == nexty)
                    {
                        break;
                    }
                    if (!stateManager.Tiles[i, j].isAvaliable)
                    {
                        isChecked = true;
                        continue;
                    }


                    temp.Add(stateManager.Tiles[i, j].obj);
                }
                if (isChecked)
                {
                    stateManager.PossibleMove.AddRange(temp);
                }
                continue;
            }



        }

    }

    public void card5()
    {
        stateManager.PossibleMove.Clear();

        stateManager.FindNextDirection();

        //go right
        foreach (int ran in stateManager.RandomList)
        {
            if (ran == 0)
            {
                for (int j = y + 1; j < 6; j++)
                {
                    if (!stateManager.Tiles[x, j].isAvaliable | (x == nextx && j == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[x, j].obj);

                }
                continue;

            }
            // go left
            if (ran == 1)
            {
                for (int j = y - 1; j > -1; j--)
                {
                    if (!stateManager.Tiles[x, j].isAvaliable | (x == nextx && j == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[x, j].obj);
                }
                continue;
            }
            //up
            if (ran == 2)
            {
                for (int i = x + 1; i < 6; i++)
                {

                    if (!stateManager.Tiles[i, y].isAvaliable | (i == nextx && y == nexty))
                    {
                        break;
                    }

                    stateManager.PossibleMove.Add(stateManager.Tiles[i, y].obj);
                }
                continue;
            }
            //down
            if (ran == 3)
            {
                for (int i = x - 1; i > -1; i--)
                {
                    if (!stateManager.Tiles[i, y].isAvaliable | (i == nextx && y == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[i, y].obj);
                }
                continue;
            }
            // up right
            if (ran == 4)
            {
                for (int i = x + 1, j = y + 1; i < 6 && j < 6; i++, j++)
                {
                    if (!stateManager.Tiles[i, j].isAvaliable | (i == nextx && j == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[i, j].obj);

                }
                continue;
            }
            //up left
            if (ran == 5)
            {
                for (int i = x + 1, j = y - 1; i < 6 && j > -1; i++, j--)
                {
                    if (!stateManager.Tiles[i, j].isAvaliable | (i == nextx && j == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[i, j].obj);

                }
                continue;
            }
            //down right
            if (ran == 6)
            {
                for (int i = x - 1, j = y + 1; i > -1 && j < 6; i--, j++)
                {
                    if (!stateManager.Tiles[i, j].isAvaliable | (i == nextx && j == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[i, j].obj);

                }
                continue;
            }

            //down left
            if (ran == 7)
            {
                for (int i = x - 1, j = y - 1; i > -1 && j > -1; i--, j--)
                {
                    if (!stateManager.Tiles[i, j].isAvaliable | (i == nextx && j == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[i, j].obj);
                }
                continue;
            }
        }



    }

    public void card6()
    {
        stateManager.PossibleMove.Clear();

        stateManager.FindNextDirection();


        foreach (int ran in stateManager.RandomList)
        {
            // up right
            if (ran == 4)
            {
                for (int i = x + 1, j = y + 1; i < 6 && j < 6; i++, j++)
                {
                    if (!stateManager.Tiles[i, j].isAvaliable | (i == nextx && j == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[i, j].obj);

                }
                continue;
            }
            //up left
            if (ran == 5)
            {
                for (int i = x + 1, j = y - 1; i < 6 && j > -1; i++, j--)
                {
                    if (!stateManager.Tiles[i, j].isAvaliable | (i == nextx && j == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[i, j].obj);

                }
                continue;
            }
            //down right
            if (ran == 6)
            {
                for (int i = x - 1, j = y + 1; i > -1 && j < 6; i--, j++)
                {
                    if (!stateManager.Tiles[i, j].isAvaliable | (i == nextx && j == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[i, j].obj);

                }
                continue;
            }

            //down left
            if (ran == 7)
            {
                for (int i = x - 1, j = y - 1; i > -1 && j > -1; i--, j--)
                {
                    if (!stateManager.Tiles[i, j].isAvaliable | (i == nextx && j == nexty))
                    {
                        break;
                    }
                    stateManager.PossibleMove.Add(stateManager.Tiles[i, j].obj);
                }
                continue;
            }
        }
    }

    public void card7()
    {
        stateManager.RevealCard = true;
    }


    public void card8()
    {
        stateManager.PossibleMove.Clear();

        stateManager.FindNextDirection();

        foreach (int i in stateManager.RandomList)
        {
            switch (i)
            {
                case 4:
                    if (stateManager.Tiles[x + 1, y + 1].isAvaliable)
                    {
                        break;
                    }
                    if (x + 2 < 6 && y + 2 < 6)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x + 2, y + 2].obj);
                    }
                    break;
                case 5:
                    if (stateManager.Tiles[x + 1, y - 1].isAvaliable)
                    {
                        break;
                    }
                    if (x + 2 < 6 && y - 2 > -1)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x + 2, y - 2].obj);
                    }
                    break;
                case 6:
                    if (stateManager.Tiles[x - 1, y + 1].isAvaliable)
                    {
                        break;
                    }
                    if (x - 2 > -1 && y + 2 < 6)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x - 2, y + 2].obj);
                    }
                    break;
                case 7:
                    if (stateManager.Tiles[x - 1, y - 1].isAvaliable)
                    {
                        break;
                    }
                    if (x - 2 > -1 && y - 2 > -1)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x - 2, y - 2].obj);
                    }
                    break;
            }

        }



    }

    public void card10()
    {
        stateManager.PossibleMove.Clear();


        stateManager.FindNextDirection();
        Debug.Log(stateManager.RandomList.Count);
        foreach (int i in stateManager.RandomList)
        {
            Debug.Log("rnd" + i);
            switch (i)
            {
                case 0:
                    if (stateManager.Tiles[x, y + 1].isAvaliable)
                    {
                        break;
                    }
                    if (y + 2 < 6)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x, y + 2].obj);
                    }
                    break;
                case 1:
                    if (stateManager.Tiles[x, y - 1].isAvaliable)
                    {
                        break;
                    }
                    if (y - 2 > -1)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x, y - 2].obj);
                    }
                    break;
                case 2:
                    if (stateManager.Tiles[x + 1, y].isAvaliable)
                    {
                        break;
                    }
                    if (x + 2 < 6)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x + 2, y].obj);
                    }
                    break;
                case 3:
                    if (stateManager.Tiles[x - 1, y].isAvaliable)
                    {
                        break;
                    }
                    if (x - 2 > -1)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x - 2, y].obj);
                    }
                    break;
                case 4:
                    if (stateManager.Tiles[x + 1, y + 1].isAvaliable)
                    {
                        break;
                    }
                    if (x + 2 < 6 && y + 2 < 6)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x + 2, y + 2].obj);
                    }
                    break;
                case 5:
                    if (stateManager.Tiles[x + 1, y - 1].isAvaliable)
                    {
                        break;
                    }
                    if (x + 2 < 6 && y - 2 > -1)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x + 2, y - 2].obj);
                    }
                    break;
                case 6:
                    if (stateManager.Tiles[x - 1, y + 1].isAvaliable)
                    {
                        break;
                    }
                    if (x - 2 > -1 && y + 2 < 6)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x - 2, y + 2].obj);
                    }
                    break;
                case 7:
                    if (stateManager.Tiles[x - 1, y - 1].isAvaliable)
                    {
                        break;
                    }
                    if (x - 2 > -1 && y - 2 > -1)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x - 2, y - 2].obj);
                    }
                    break;
            }

        }



    }

    public void card11()
    {
        stateManager.PossibleMove.Clear();
        stateManager.FindNextDirection();

        foreach (int i in stateManager.RandomList)
        {
            switch (i)
            {
                case 0:
                    if (!(x == nextx && y + 1 == nexty))
                    {
                        break;
                    }
                    if (y + 2 < 6)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x, y + 2].obj);
                    }
                    break;
                case 1:
                    if (!(x == nextx && y - 1 == nexty))
                    {
                        break;
                    }
                    if (y - 2 > -1)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x, y - 2].obj);
                    }
                    break;
                case 2:
                    if (!(x + 1 == nextx && y == nexty))
                    {
                        break;
                    }
                    if (x + 2 < 6)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x + 2, y].obj);
                    }
                    break;
                case 3:
                    if (!(x - 1 == nextx && y == nexty))
                    {
                        break;
                    }
                    if (x - 2 > -1)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x - 2, y].obj);
                    }
                    break;
                case 4:
                    if (!(x + 1 == nextx && y + 1 == nexty))
                    {
                        break;
                    }
                    if (x + 2 < 6 && y + 2 < 6)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x + 2, y + 2].obj);
                    }
                    break;
                case 5:
                    if (!(x + 1 == nextx && y - 1 == nexty))
                    {
                        break;
                    }
                    if (x + 2 < 6 && y - 2 > -1)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x + 2, y - 2].obj);
                    }
                    break;
                case 6:
                    if (!(x - 1 == nextx && y + 1 == nexty))
                    {
                        break;
                    }
                    if (x - 2 > -1 && y + 2 < 6)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x - 2, y + 2].obj);
                    }
                    break;
                case 7:
                    if (!(x - 1 == nextx && y - 1 == nexty))
                    {
                        break;
                    }
                    if (x - 2 > -1 && y - 2 > -1)
                    {
                        stateManager.PossibleMove.Add(stateManager.Tiles[x - 2, y - 2].obj);
                    }
                    break;
            }

        }
    }
}