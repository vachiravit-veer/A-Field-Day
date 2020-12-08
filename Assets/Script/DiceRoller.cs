using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour
{


    // Use this for initialization
    void Start()
    {

        rend = GetComponent<SpriteRenderer>();
        // Load dice sides sprites to array from DiceSides subfolder of Resources folder
        diceSides = Resources.LoadAll<Sprite>("Dice1/");
        theStateManager = FindObjectOfType<StateManager>();
        updateGui = FindObjectOfType<UpdateGui>();
        cards_panel = new GameObject();
        cards_panel = GameObject.FindGameObjectWithTag("Cards");
    }

    StateManager theStateManager;
    UpdateGui updateGui;
    private Sprite[] diceSides;
    private SpriteRenderer rend;
    private GameObject cards_panel;
    public void MoveOneBox()
    {
        if (theStateManager.IsDoneRolling == true)
        {
            // We've already rolled this turn.
            return;
        }
        theStateManager.FindNextOneMove();

        theStateManager.IsDoneRolling = true;
        BoardHighlighter.Instance.HighlightAllowedMoves(theStateManager.PossibleMove);
        theStateManager.IsDoneClicking = true;


    }
    public void RollTheDice()
    {

        if (theStateManager.IsDoneRolling == true)
        {
            // We've already rolled this turn.
            return;
        }
        theStateManager.FindNextMove();

        int rnd = theStateManager.myPlayer[0].rnd;

        int randomDiceSide = 0;

        // Final side or value that dice reads in the end of coroutine

        // Loop to switch dice sides ramdomly
        // before final side appears. 20 itterations here.
        for (int i = 0; i <= 20; i++)
        {
            // Pick up random value from 0 to 5 (All inclusive)
            randomDiceSide = Random.Range(0, 5);
            // Set sprite to upper face of dice from array according to random value
            rend.gameObject.GetComponent<Image>().sprite = diceSides[randomDiceSide];

            // Pause before next itteration
            //yield return new WaitForSeconds(0.05f);
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



        theStateManager.IsDoneRolling = true;
        BoardHighlighter.Instance.HighlightAllowedMoves(theStateManager.PossibleMove);
        theStateManager.IsDoneClicking = true;
    }

    public void Showcard()
    {
        if (theStateManager.IsDoneRolling == true)
        {
            // We've already rolled this turn.
            return;
        }
        updateGui.showCardInventory();
    }
}
