using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{

    void Start()
    {
        Button[] buttons = this.GetComponentsInChildren<Button>();
        stateManager = FindObjectOfType<StateManager>();
        updateGui = FindObjectOfType<UpdateGui>();
        buttons[0].onClick.AddListener(selectedRice);
        buttons[1].onClick.AddListener(selectedTree);
        buttons[2].onClick.AddListener(selectedWater);
        buttons[3].onClick.AddListener(selectedHome);
        buttons[4].onClick.AddListener(Goback);
        updateGui.UpdateGuiScore("null", "inventory");
    }


    StateManager stateManager;
    UpdateGui updateGui;

    // Update is called once per frame
    private bool Selected;
    private int SelectedType;
    void Update()
    {
        if (!Selected)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("Boards"))
            {
                if (stateManager.PossibleMove.Contains(hit.transform.gameObject))
                {
                    stateManager.SetAvaliable(hit.transform.gameObject, SelectedType);
                    UpdateScore(SelectedType);
                    Selected = false;
                }

            }

        }


    }
    private void UpdateScore(int t)
    {
        switch (t)
        {
            case 0:
                stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.rices -= 1;
                break;
            case 1:
                stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.trees -= 1;
                break;
            case 2:
                stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.waters -= 1;
                break;
            case 3:
                stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.home -= 1;
                break;
        }
        updateGui.UpdateGuiScore("null", "inventory");


    }
    private void selectedRice()
    {
        if (stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.rices > 0)
        {
            Selected = true;
            SelectedType = 0;
        }


    }
    private void selectedTree()
    {
        if (stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.trees > 0)
        {
            Selected = true;
            SelectedType = 1;
        }
    }
    private void selectedWater()
    {
        if (stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.waters > 0)
        {
            Selected = true;
            SelectedType = 2;
        }
    }
    private void selectedHome()
    {
        if (stateManager.myPlayer[stateManager.CurrentPlayerId].Inventory.home > 0)
        {
            Selected = true;
            SelectedType = 3;
        }
    }

    private void Goback()
    {
        updateGui.ShowAllMenu();
        stateManager.IsDoneAnimating = true;
        stateManager.IsDonePopup = true;
        stateManager.NewTurn();

    }


}
