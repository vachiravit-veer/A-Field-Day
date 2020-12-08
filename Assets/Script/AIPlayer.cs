using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class AIPlayer : MyPlayer
{

    public AIPlayer()
    {
        stateManager = GameObject.FindObjectOfType<StateManager>();
        moveObject = GameObject.FindObjectOfType<MoveObject>();
        updateGui = GameObject.FindObjectOfType<UpdateGui>();
        gameLogic = GameObject.FindObjectOfType<GameLogic>();

    }

    StateManager stateManager;
    MoveObject moveObject;
    UpdateGui updateGui;
    GameLogic gameLogic;

    private string rnd_name;

    override public void DoAI()
    {

        if (stateManager.isDoneWaited == false)
        {
            return;

        }
        if (stateManager.IsDoneRolling == false)
        {

            Debug.Log("Ai START");
            // We need to move
            DoRoll();
            return;
        }
        if (stateManager.IsDoneAnimating == false)
        {
            return;
        }
       /* if (stateManager.IsDonePopup == false)
        {
            DoPopUP();
            return;
        }*/
        if(stateManager.IsDoneClicking==true)
        {
            DoPortal();
        }
        if(stateManager.hasPortalOpen==true)
        {
            return;
        }
        if(stateManager.hasPortalOpen==false)
        {
            DoEnd();
        }
    }


    private void DoRoll()
    {

        stateManager.IsDoneRolling = true;
       
        if (Random.value < 0.50f)
        {

            stateManager.FindNextOneMove();
        }
        else
        {
            stateManager.FindNextMove();
        }

        if (stateManager.PossibleMove.Count == 0)
        {
            bool hasUseableCard = false;
            List<int> listofcard = ReturnInventoryCard();
            //check if there is card 4 11
            //4 11
            if (listofcard.Contains(4) && !hasUseableCard)
            {
                gameLogic.card4();
                if (stateManager.PossibleMove.Count > 0)
                {
                    Debug.Log("Ai use card 4");
                    //card has effect
                    RemoveInventoryCard(4);
                    hasUseableCard = true;
                }

            }
            if (listofcard.Contains(11) && !hasUseableCard)
            {
                gameLogic.card11();
                if (stateManager.PossibleMove.Count > 0)
                {
                    RemoveInventoryCard(11);
                    Debug.Log("Ai usse card 11");
                }
            }
        }


        if (stateManager.PossibleMove.Count == 0)
        {
            Debug.Log("Ai possible move  " + stateManager.PossibleMove.Count + "bot rnd" + stateManager.myPlayer[stateManager.CurrentPlayerId].rnd);
            //can ai use something or need to  forfeit
            //if use something need to random action
            //       
            List<int> RandomList = new List<int>();


            stateManager.FindUnAvaliable();

            foreach (GameObject obj in stateManager.PossibleMove)
            {
                RandomList.Clear();
                if (Inventory.rices > 0)
                {
                    RandomList.Add(0);
                }
                if (Inventory.trees > 0)
                {
                    RandomList.Add(1);
                }
                if (Inventory.waters > 0)
                {
                    RandomList.Add(2);
                }
                if (Inventory.home > 0)
                {
                    RandomList.Add(3);
                }
                if (RandomList.Count == 0)
                { break; }
                var ran = RandomList[Random.Range(0, RandomList.Count)];
                // 0 rices 1 tree 2 water 3 home
                switch (ran)
                {
                    case 0:
                        stateManager.SetAvaliable(obj, 0);
                        Inventory.rices -= 1;
                        break;
                    //create rices on its
                    case 1:
                        stateManager.SetAvaliable(obj, 1);
                        Inventory.trees -= 1;
                        break;
                    case 2:
                        stateManager.SetAvaliable(obj, 2);
                        Inventory.waters -= 1;
                        break;
                    case 3:
                        stateManager.SetAvaliable(obj, 3);
                        Inventory.home -= 1;
                        break;

                }

                if (Random.value < 0.50f)
                {

                    break;
                }

            }

            stateManager.IsDonePopup = true;
            stateManager.IsDoneAnimating = true;
            updateGui.UpdateGuiScore("yellow", "null");
            updateGui.controller.SetActive(true); 
        }
        else
        {
            if (stateManager.PossibleMove.Count == 1)
            {
                moveObject.MoveMe(stateManager.playerObjects[stateManager.CurrentPlayerId], stateManager.PossibleMove[0].transform, 5.0f);
                rnd_name = stateManager.PossibleMove[0].name;
                return;
            }
            // Bot select Move randoml
            GameObject rndNext = stateManager.PossibleMove[Random.Range(0, stateManager.PossibleMove.Count)];
            rnd_name = rndNext.name;
         
            moveObject.MoveMe(stateManager.playerObjects[stateManager.CurrentPlayerId], rndNext.transform, 5.0f);
        }
        DoPopUP();


    }

    private void DoPopUP()
    {

        stateManager.UpdatePosition(rnd_name);

        //bot do something



    }

    private void DoPortal()
    {
        stateManager.IsDoneClicking = false;
        GameObject rndNext = stateManager.PortalList[Random.Range(0, stateManager.PortalList.Count)];
        rnd_name = rndNext.name;
        moveObject.MoveMe(stateManager.playerObjects[stateManager.CurrentPlayerId], rndNext.transform, 5.0f);
        stateManager.UpdatePosition(rnd_name);
    }

    private void DoEnd()
    {
        stateManager.IsDonePopup = true;
        updateGui.controller.SetActive(true);
        Debug.Log("AI done");
    }

}
