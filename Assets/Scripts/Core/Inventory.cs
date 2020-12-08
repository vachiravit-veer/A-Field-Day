using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
public class Inventory : MonoBehaviour
{
    
    [SerializeField]
    private Sprite[] _cardsImage;
    private Button[] buttons;
    private int[] tempCards;

    void Start()
    {

        buttons = this.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(selectedCard1);
        buttons[1].onClick.AddListener(selectedCard2);
        buttons[2].onClick.AddListener(selectedCard3);
        buttons[3].onClick.AddListener(CloseInventory);
        _gui = FindObjectOfType<GUIController>();
        _cardLogic = FindObjectOfType<CardLogic>();
        tempCards = new int[3];
        tempCards = (int[])PhotonNetwork.LocalPlayer.CustomProperties["cards"];

        buttons[0].GetComponent<Image>().sprite = _cardsImage[tempCards[0]-1];
        buttons[1].GetComponent<Image>().sprite = _cardsImage[tempCards[1]-1];
        buttons[2].GetComponent<Image>().sprite = _cardsImage[tempCards[2]-1];
    }

    GUIController _gui;
    CardLogic _cardLogic;
    
    

    private void selectedCard1()
    {
        _cardLogic.OnClick_useCard(tempCards[0], buttons[0].gameObject);

    }
    private void selectedCard2()
    {
        _cardLogic.OnClick_useCard(tempCards[1], buttons[1].gameObject);

    }
    private void selectedCard3()
    {
        _cardLogic.OnClick_useCard(tempCards[2], buttons[2].gameObject);
    }
    private void CloseInventory()
    {
        _gui.hideCardInventory();
    }  
 
}
