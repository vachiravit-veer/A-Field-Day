using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public bool _isAllowedClick=false;
    OnlineGameLogic _gameLogic;
    private Animator m_animator;
    CardLogic _cardLogic;
    GUIController _gui;
    Ray ray;
    void Start()
    {
        _gameLogic = FindObjectOfType<OnlineGameLogic>();
        _cardLogic = FindObjectOfType<CardLogic>();
        _gui = FindObjectOfType<GUIController>();

    }

    // Update is called once per frame
    void Update()
    {  

        if (_isAllowedClick)
        {

            // if (Input.GetMouseButtonDown(0))
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                }
                else
                {
                    return;
                }
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                }
                else
                {
                    return;
                }
            }

            RaycastHit hit;

            if (OnlineGameLogic.Onclick_swapPlayer)
            {
                Debug.Log("swap able");
                if (Physics.Raycast(ray, out hit,100) && hit.transform.CompareTag("Player"))
                {
                    Debug.Log("click on player" + hit.transform.name);

                    _gameLogic.DoSwap(hit.transform.gameObject);
                    return;
                }
                return;
            }

            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("Boards"))
            {
                Debug.Log("Click on " + hit.transform.name);

                if (CardLogic.RevealCard)
                {
                    _cardLogic.OnClick_RevealTiles(hit.transform.gameObject);
                    return;
                }

                if (CardLogic.PickUpCard && _gameLogic.PossibleMove.Contains(hit.transform.gameObject))
                {
                    _cardLogic.OnClick_PickupTile(hit.transform.gameObject);
                    return;
                }

                if (_gameLogic.PossibleMove.Contains(hit.transform.gameObject))
                {
                    if (OnlineGameLogic.OnClick_SpecialTile)
                        _gui.HideSpecialInventoryWaitForEndTurn();
                    if (!OnlineGameLogic.needToTeleport)
                        CreateHighlight.Instance.HideHighlights();                      
                    _isAllowedClick = false;
                    StartCoroutine(MoveMe(hit.transform.position,5f,hit.transform.name,0.1f));
                }

            }
        }

    }


    public IEnumerator MoveMe(Vector3 b, float speed,string name , float seconds)
    {
        yield return new WaitForSeconds(seconds);

        GameObject go = PhotonNetwork.LocalPlayer.TagObject as GameObject;
       
        go.transform.LookAt(b);
        m_animator = go.GetComponent<Animator>();
        m_animator.SetFloat("MoveSpeed", 999);
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
        m_animator.SetFloat("MoveSpeed", 0);
    
        StartCoroutine(_gameLogic.UpdatePosition(name));

    }


}
