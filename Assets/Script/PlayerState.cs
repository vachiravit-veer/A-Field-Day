using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerState : MonoBehaviour
{


    private void Start()
    {
        stateManager = FindObjectOfType<StateManager>();
        moveObject = FindObjectOfType<MoveObject>();
        updateGui = FindObjectOfType<UpdateGui>();

    }

    StateManager stateManager;
    MoveObject moveObject;
    UpdateGui updateGui;

    private void Update()
    {
        if (stateManager.IsDoneClicking == true)
        {

            if (stateManager.PossibleMove.Count == 0 && !stateManager.RevealCard && !stateManager.hasPortalOpen)
            {
                Debug.Log("player has no possible move");
                //Show canvas//
                stateManager.IsDoneClicking = false;
                updateGui.Showinventory();
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {

                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("Boards"))
                {

                    if (stateManager.RevealCard)
                    {
                        stateManager.ShowCard(hit.transform.gameObject);
                        return;

                    }
                    if (stateManager.hasPortalOpen)
                    {
                        if (stateManager.PortalList.Contains(hit.transform.gameObject))
                        {
                            stateManager.IsDoneClicking = false;
                            moveObject.MoveMe(stateManager.playerObjects[stateManager.CurrentPlayerId], hit.transform, 5.0f);
                            stateManager.UpdatePosition(hit.transform.name);
                            return;
                        }
                    }
                    if (stateManager.PossibleMove.Contains(hit.transform.gameObject))
                    {
                        if (stateManager.canPickupCard)
                        {
                            stateManager.UpdatePosition(hit.transform.name);
                            return;
                        }

                        stateManager.IsDoneClicking = false;
                        moveObject.MoveMe(stateManager.playerObjects[stateManager.CurrentPlayerId], hit.transform, 5.0f);
                        //StartCoroutine(stateManager.MoveOverSpeed(Player1,hit.transform.position, 5.0f));
                        //stateManager.MoveMe(hit.transform.position, 5.0f);

                        stateManager.UpdatePosition(hit.transform.name);



                    }

                }

            }
        }

    }







}

