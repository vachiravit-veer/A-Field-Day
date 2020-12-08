using UnityEngine;
using UnityEngine.UI;

public class CardInventory : MonoBehaviour
{
    public GameObject cardPanel;
    private Button[] buttons;
    void Start()
    {

        buttons = this.GetComponentsInChildren<Button>();
        updateGui = FindObjectOfType<UpdateGui>();
        stateManager = FindObjectOfType<StateManager>();
        gameLogic = FindObjectOfType<GameLogic>();
        buttons[0].onClick.AddListener(selectedCard1);
        buttons[1].onClick.AddListener(selectedCard2);
        buttons[2].onClick.AddListener(selectedCard3);
        buttons[3].onClick.AddListener(CloseInventory);
             updateGui.SetCardPicture();
    }

    UpdateGui updateGui;
    GameLogic gameLogic;
    StateManager stateManager;

    private void selectedCard1()
    {
        gameLogic.SelectedCard(stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.cards[0],buttons[0].gameObject);
        //buttons[0].gameObject.SetActive(false);

    }
    private void selectedCard2()
    {
        gameLogic.SelectedCard(stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.cards[1], buttons[1].gameObject);
        //[1].gameObject.SetActive(false);

    }
    private void selectedCard3()
    {
        gameLogic.SelectedCard(stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.cards[2], buttons[2].gameObject);

       // buttons[2].gameObject.SetActive(false);
    }
    private void CloseInventory()
    {
        updateGui.hideCardInventory();
    }
}
