using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UpdateGui : MonoBehaviour
{
    public GameObject Canvas, scCanvas, controller, yourTurn, wincanvas, inventory_panel, card_panel, inventory, signwin, myPanel, playerPanel, botPanel;


    public Sprite[] Red, Yellow, Scoreimg, OldScoreImg, Winnerimg, player, Cards, Portal,playerIMG,specialTiles;

 
    // Use this for initialization
    void Start()
    {
        stateManager = FindObjectOfType<StateManager>();
       
 
    }

    StateManager stateManager;

    public void NoMovePopUp()
    {
        // show nomove canvas
        // wait for user input
        // use something or forfeit

    }
    public void ChangeScore(string color, string Tiletype)
    {
        switch (color)
        {
            case "red":
                switch (Tiletype)
                {
                    case "home":
                        playerPanel.transform.Find("home_player1").GetComponent<Image>().sprite = Scoreimg[0];
                        playerPanel.transform.Find("Scorehomeplayer1").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.home.ToString();
                        break;
                    case "rice":
                        playerPanel.transform.Find("Scorericeplayer1").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.rices.ToString();
                        playerPanel.transform.Find("rice_player1").GetComponent<Image>().sprite = Scoreimg[1];
                        break;
                    case "tree":
                        playerPanel.transform.Find("Scoretreeplayer1").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.trees.ToString();
                        playerPanel.transform.Find("tree_player1").GetComponent<Image>().sprite = Scoreimg[2];
                        break;
                    case "water":
                        playerPanel.transform.Find("Scorewaterplayer1").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.waters.ToString();
                        playerPanel.transform.Find("water_player1").GetComponent<Image>().sprite = Scoreimg[3];
                        break;

                }
                break;
            case "yellow":
                switch (Tiletype)
                {
                    case "home":
                        botPanel.transform.Find("ScoreHomeplayer2").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.home.ToString();
                        botPanel.transform.Find("home_player2").GetComponent<Image>().sprite = Scoreimg[0];
                        break;
                    case "rice":
                        botPanel.transform.Find("Scorericeplayer2").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.rices.ToString();
                        botPanel.transform.Find("rice_player2").GetComponent<Image>().sprite = Scoreimg[1];
                        break;
                    case "tree":
                        botPanel.transform.Find("Scoretreeplayer2").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.trees.ToString();
                        botPanel.transform.Find("tree_player2").GetComponent<Image>().sprite = Scoreimg[2];
                        break;
                    case "water":
                        botPanel.transform.Find("Scorewaterplayer2").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.waters.ToString();
                        botPanel.transform.Find("water_player2").GetComponent<Image>().sprite = Scoreimg[3];
                        break;

                }
                break;


        }



    }

    public IEnumerator ShowPopUp(string color, string Tiletype)
    {



        controller.SetActive(false);

        switch (color)
        {
            case "null":
                switch(Tiletype)
                {
                    case "swap":
                        myPanel.transform.Find("Image").GetComponent<Image>().sprite = specialTiles[0];
                        break;

                    case "wind":
                        myPanel.transform.Find("Image").GetComponent<Image>().sprite = specialTiles[1];
                        break;
                    case "portal":
                        myPanel.transform.Find("Image").GetComponent<Image>().sprite = specialTiles[2];
                        break;

                }
                break;
            case "red":
                switch (Tiletype)
                {
                    case "home":
                        myPanel.transform.Find("Image").GetComponent<Image>().sprite = Red[0];
                        break;
                    case "rice":
                        myPanel.transform.Find("Image").GetComponent<Image>().sprite = Red[1];
                        break;
                    case "tree":
                        myPanel.transform.Find("Image").GetComponent<Image>().sprite = Red[2];
                        break;
                    case "water":
                        myPanel.transform.Find("Image").GetComponent<Image>().sprite = Red[3];
                        break;

                }
                break;
            case "yellow":
                switch (Tiletype)
                {
                    case "home":
                        myPanel.transform.Find("Image").GetComponent<Image>().sprite = Yellow[0];
                        break;
                    case "rice":
                        myPanel.transform.Find("Image").GetComponent<Image>().sprite = Yellow[1];
                        break;
                    case "tree":
                        myPanel.transform.Find("Image").GetComponent<Image>().sprite = Yellow[2];
                        break;
                    case "water":
                        myPanel.transform.Find("Image").GetComponent<Image>().sprite = Yellow[3];
                        break;

                }
                break;

        }

        scCanvas.SetActive(true);

        //Display Messagage    .SetActive
        //  UPDATE SCORE
        //wait for 1 second
        yield return new WaitForSeconds(1.5f);
        scCanvas.SetActive(false);

        yield return new WaitForSeconds(1.4f);
        if (!stateManager.hasPortalOpen && !stateManager.showAroundTile)
        {
            stateManager.IsDonePopup = true;
        }
        Debug.Log("update gui done");

     
    }


    public IEnumerator showyourTurn()
    {
        yourTurn.SetActive(true);
        yield return new WaitForSeconds(1f);
        yourTurn.SetActive(false);
        controller.SetActive(true);
    }


    public void showWinner(int id)
    {
        Canvas.SetActive(false);
        signwin.transform.Find("player").GetComponent<Image>().sprite = player[id];
        wincanvas.SetActive(true);


    }

    public void Showinventory()
    {
        Canvas.SetActive(false);
        stateManager.FindUnAvaliable();
        UpdateGuiScore("null", "inventory");
        inventory_panel.SetActive(true);


    }

    public void ShowAllMenu()
    {
        inventory_panel.SetActive(false);
        UpdateGuiScore("red", "null");
        Canvas.SetActive(true);
    }


    public void RefreashGuiScore()
    {
        Sprite tempsprite = playerPanel.GetComponent<Image>().sprite;
        playerPanel.GetComponent<Image>().sprite = botPanel.GetComponent<Image>().sprite;

        playerPanel.transform.Find("Scorehomeplayer1").GetComponent<Text>().text = stateManager.myPlayer[0].Inventory.home.ToString();
        if (stateManager.myPlayer[0].Inventory.home == 0)
        {
            playerPanel.transform.Find("home_player1").GetComponent<Image>().sprite = OldScoreImg[0];
        }
        else
        {
            playerPanel.transform.Find("home_player1").GetComponent<Image>().sprite = Scoreimg[0];
        }
        playerPanel.transform.Find("Scorericeplayer1").GetComponent<Text>().text = stateManager.myPlayer[0].Inventory.rices.ToString();
        if (stateManager.myPlayer[0].Inventory.rices == 0)
        {
            playerPanel.transform.Find("rice_player1").GetComponent<Image>().sprite = OldScoreImg[1];
        }
        else
        {
            playerPanel.transform.Find("rice_player1").GetComponent<Image>().sprite = Scoreimg[1];
        }
        playerPanel.transform.Find("Scoretreeplayer1").GetComponent<Text>().text = stateManager.myPlayer[0].Inventory.trees.ToString();
        if (stateManager.myPlayer[0].Inventory.trees == 0)
        {
            playerPanel.transform.Find("tree_player1").GetComponent<Image>().sprite = OldScoreImg[2];
        }
        else
        {
            playerPanel.transform.Find("tree_player1").GetComponent<Image>().sprite = Scoreimg[2];
        }
        playerPanel.transform.Find("Scorewaterplayer1").GetComponent<Text>().text = stateManager.myPlayer[0].Inventory.waters.ToString();
        if (stateManager.myPlayer[0].Inventory.waters == 0)
        {
            playerPanel.transform.Find("water_player1").GetComponent<Image>().sprite = OldScoreImg[3];
        }
        else
        {
            playerPanel.transform.Find("water_player1").GetComponent<Image>().sprite = Scoreimg[3];
        }
        botPanel.GetComponent<Image>().sprite = tempsprite;
        botPanel.transform.Find("ScoreHomeplayer2").GetComponent<Text>().text = stateManager.myPlayer[1].Inventory.home.ToString();
        botPanel.transform.Find("Scorericeplayer2").GetComponent<Text>().text = stateManager.myPlayer[1].Inventory.rices.ToString();
        botPanel.transform.Find("Scoretreeplayer2").GetComponent<Text>().text = stateManager.myPlayer[1].Inventory.trees.ToString();
        botPanel.transform.Find("Scorewaterplayer2").GetComponent<Text>().text = stateManager.myPlayer[1].Inventory.waters.ToString();

        if (stateManager.myPlayer[1].Inventory.home == 0)
        {
            botPanel.transform.Find("home_player2").GetComponent<Image>().sprite = OldScoreImg[0];
        }
        else
        {
            botPanel.transform.Find("home_player2").GetComponent<Image>().sprite = Scoreimg[0];
        }

        if (stateManager.myPlayer[1].Inventory.rices == 0)
        {
            botPanel.transform.Find("rice_player2").GetComponent<Image>().sprite = OldScoreImg[1];
        }
        else
        {
            botPanel.transform.Find("rice_player2").GetComponent<Image>().sprite = Scoreimg[1];
        }

        if (stateManager.myPlayer[1].Inventory.trees == 0)
        {
            botPanel.transform.Find("tree_player2").GetComponent<Image>().sprite = OldScoreImg[2];
        }
        else
        {
            botPanel.transform.Find("tree_player2").GetComponent<Image>().sprite = Scoreimg[2];
        }
        if (stateManager.myPlayer[1].Inventory.waters == 0)
        {
            botPanel.transform.Find("water_player2").GetComponent<Image>().sprite = OldScoreImg[3];
        }
        else
        {
            botPanel.transform.Find("water_player2").GetComponent<Image>().sprite = Scoreimg[3];
        }

    }





    public void UpdateGuiScore(string color, string PanelType)
    {
        if (color != "null")
        {

            switch (color)
            {
                case "red":
                    playerPanel.transform.Find("Scorehomeplayer1").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.home.ToString();
                    if (stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.home == 0)
                    {
                        playerPanel.transform.Find("home_player1").GetComponent<Image>().sprite = OldScoreImg[0];
                    }
                    else
                    {
                        playerPanel.transform.Find("home_player1").GetComponent<Image>().sprite = Scoreimg[0];
                    }
                    playerPanel.transform.Find("Scorericeplayer1").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.rices.ToString();
                    if (stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.rices == 0)
                    {
                        playerPanel.transform.Find("rice_player1").GetComponent<Image>().sprite = OldScoreImg[1];
                    }
                    else
                    {
                        playerPanel.transform.Find("rice_player1").GetComponent<Image>().sprite = Scoreimg[1];
                    }
                    playerPanel.transform.Find("Scoretreeplayer1").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.trees.ToString();
                    if (stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.trees == 0)
                    {
                        playerPanel.transform.Find("tree_player1").GetComponent<Image>().sprite = OldScoreImg[2];
                    }
                    else
                    {
                        playerPanel.transform.Find("tree_player1").GetComponent<Image>().sprite = Scoreimg[2];
                    }
                    playerPanel.transform.Find("Scorewaterplayer1").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.waters.ToString();
                    if (stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.waters == 0)
                    {
                        playerPanel.transform.Find("water_player1").GetComponent<Image>().sprite = OldScoreImg[3];
                    }
                    else
                    {
                        playerPanel.transform.Find("water_player1").GetComponent<Image>().sprite = Scoreimg[3];
                    }
                    break;
                case "yellow":
                    botPanel.transform.Find("ScoreHomeplayer2").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.home.ToString();
                    botPanel.transform.Find("Scorericeplayer2").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.rices.ToString();
                    botPanel.transform.Find("Scoretreeplayer2").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.trees.ToString();
                    botPanel.transform.Find("Scorewaterplayer2").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.waters.ToString();

                    if (stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.home == 0)
                    {
                        botPanel.transform.Find("home_player2").GetComponent<Image>().sprite = OldScoreImg[0];
                    }
                    else
                    {
                        botPanel.transform.Find("home_player2").GetComponent<Image>().sprite = Scoreimg[0];
                    }

                    if (stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.rices == 0)
                    {
                        botPanel.transform.Find("rice_player2").GetComponent<Image>().sprite = OldScoreImg[1];
                    }
                    else
                    {
                        botPanel.transform.Find("rice_player2").GetComponent<Image>().sprite = Scoreimg[1];
                    }

                    if (stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.trees == 0)
                    {
                        botPanel.transform.Find("tree_player2").GetComponent<Image>().sprite = OldScoreImg[2];
                    }
                    else
                    {
                        botPanel.transform.Find("tree_player2").GetComponent<Image>().sprite = Scoreimg[2];
                    }
                    if (stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.waters == 0)
                    {
                        botPanel.transform.Find("water_player2").GetComponent<Image>().sprite = OldScoreImg[3];
                    }
                    else
                    {
                        botPanel.transform.Find("water_player2").GetComponent<Image>().sprite = Scoreimg[3];
                    }
                  
                     break;

            }
        }
        if (PanelType == "inventory")
        {
            inventory.transform.Find("Scorericeplayer1").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.rices.ToString();
            inventory.transform.Find("Scoretreeplayer1").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.trees.ToString();
            inventory.transform.Find("Scorewaterplayer1").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.waters.ToString();
            inventory.transform.Find("Scorehomeplayer1").GetComponent<Text>().text = stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.home.ToString();

        }

    }






    public void SetCardPicture()
    {
        Button[] buttons = card_panel.GetComponentsInChildren<Button>();
        buttons[0].GetComponent<Image>().sprite = Cards[stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.cards[0] - 1];
        buttons[1].GetComponent<Image>().sprite = Cards[stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.cards[1] - 1];
        buttons[2].GetComponent<Image>().sprite = Cards[stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.cards[2] - 1];
    }
    public void showCardInventory()
    {

        if (card_panel.GetComponentsInChildren<Transform>().GetLength(0) == 1)
        {
            //show no more card popup
            //donerolling flasefalse;
            return;

        }
        else
        {
            controller.SetActive(false);
            // stateManager.IsDoneRolling = true;
            card_panel.SetActive(true);
        }
    }

    public void hideCardInventory()
    {
        card_panel.SetActive(false);
        if (!stateManager.RevealCard)
        {
            controller.SetActive(true);
        }
    }
}
