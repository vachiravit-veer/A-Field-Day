using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreateHighlight : MonoBehaviour
{
    [SerializeField]
    private GameObject _windPrefab, _highlightPrefab,_rainPrefab;
    [SerializeField]
    private GameObject[] portalPrefab,red,yellow,_black,_purple, _swapPrefab;
    [SerializeField]
    private List<GameObject> oldhighlights,_highlights;


    private int count=0;
    OnlineGameLogic _gameLogic;

    private List<Vector3> _exists;

    public static CreateHighlight Instance { get; set; }
    void Start()
    {
        Instance = this;
        _gameLogic = FindObjectOfType<OnlineGameLogic>();
        if(PhotonNetwork.IsMasterClient)
            _exists = new List<Vector3>();
    }

    private GameObject GetHighlightObject()
    {
        GameObject go = oldhighlights.Find(g => !g.activeSelf);
        if (!go)
        {
            go = Instantiate(_highlightPrefab);
            oldhighlights.Add(go);
        }
        return go;
    }

    private bool ExistinPosition(Vector3 targetPos)
    {
        
        for (var i = 0; i < _exists.Count; i++)
        {
            if (Vector3.SqrMagnitude(_exists[i] - targetPos) < 0.0001)
                return true;
        }


        return false;
     
    }

    public void HighlightAllowedMoves(List<GameObject> moves)
    {

        foreach (GameObject obj in moves)
        {     
            GameObject go = GetHighlightObject();
            go.SetActive(true);
            go.transform.position = new Vector3(obj.transform.position.x, 2.5f, obj.transform.position.z);

        }
    }

    [PunRPC]
    void HighlightSceneObject(Vector3 pos , string c, string t , PhotonMessageInfo info)
    {
        if (ExistinPosition(pos))
            return;

        if (t == "rain")
        {
            MasterManager.SceneNetworkInstanitate(_rainPrefab, pos, Quaternion.identity);
        }
        else if (t == "swapPosition")
        {
           MasterManager.SceneNetworkInstanitate(_swapPrefab[0], pos, Quaternion.identity);
        }
        else if (t == "swapCharacter")
        {
           MasterManager.SceneNetworkInstanitate(_swapPrefab[1], pos, Quaternion.identity);
        }
        else if (t == "wind")
        {
           MasterManager.SceneNetworkInstanitate(_windPrefab, pos, Quaternion.identity);
        }
        else if (t == "portal")
        {
            switch(count)
            {
                case 0:
                    MasterManager.SceneNetworkInstanitate(portalPrefab[0], pos, Quaternion.identity);
                    break;
                case 1:
                    MasterManager.SceneNetworkInstanitate(portalPrefab[1], pos, Quaternion.identity);
                    break;
                case 2:
                    MasterManager.SceneNetworkInstanitate(portalPrefab[2], pos, Quaternion.identity);
                    break;
                case 3:
                    MasterManager.SceneNetworkInstanitate(portalPrefab[3], pos, Quaternion.identity);
                    break;
                default:
                    MasterManager.SceneNetworkInstanitate(portalPrefab[0], pos, Quaternion.identity);
                    break;
            }
            count++;
           //MasterManager.SceneNetworkInstanitate(portalPrefab[0], pos, Quaternion.identity);
        }
        else
        {
            switch (c)
            {
                case "red":
                    switch (t)
                    {
                        case "home":
                            MasterManager.SceneNetworkInstanitate(red[0], pos, Quaternion.identity);
                            break;
                        case "rice":
                            MasterManager.SceneNetworkInstanitate(red[1], pos, Quaternion.identity);
                            break;
                        case "tree":
                            MasterManager.SceneNetworkInstanitate(red[2], pos, Quaternion.identity);
                            break;
                        case "water":
                            MasterManager.SceneNetworkInstanitate(red[3], pos, Quaternion.identity);
                            break;
                        case "start":
                            MasterManager.SceneNetworkInstanitate(red[4], pos, Quaternion.identity);
                            break;
                    }

                    break;
                case "yellow":
                    switch (t)
                    {
                        case "home":
                            MasterManager.SceneNetworkInstanitate(yellow[0], pos, Quaternion.identity);
                            break;
                        case "rice":
                            MasterManager.SceneNetworkInstanitate(yellow[1], pos, Quaternion.identity);
                            break;
                        case "tree":
                            MasterManager.SceneNetworkInstanitate(yellow[2], pos, Quaternion.identity);
                            break;
                        case "water":
                            MasterManager.SceneNetworkInstanitate(yellow[3], pos, Quaternion.identity);
                            break;
                        case "start":
                            MasterManager.SceneNetworkInstanitate(yellow[4], pos, Quaternion.identity);
                            break;
                    }
                    break;
                case "black":
                    switch (t)
                    {
                        case "home":
                            MasterManager.SceneNetworkInstanitate(_black[0], pos, Quaternion.identity);
                            break;
                        case "rice":
                            MasterManager.SceneNetworkInstanitate(_black[1], pos, Quaternion.identity);
                            break;
                        case "tree":
                            MasterManager.SceneNetworkInstanitate(_black[2], pos, Quaternion.identity);
                            break;
                        case "water":
                            MasterManager.SceneNetworkInstanitate(_black[3], pos, Quaternion.identity);
                            break;
                        case "start":
                            MasterManager.SceneNetworkInstanitate(_black[4], pos, Quaternion.identity);
                            break;
                    }
                    break;
                case "purple":
                    switch (t)
                    {
                        case "home":
                            MasterManager.SceneNetworkInstanitate(_purple[0], pos, Quaternion.identity);
                            break;
                        case "rice":
                            MasterManager.SceneNetworkInstanitate(_purple[1], pos, Quaternion.identity);
                            break;
                        case "tree":
                            MasterManager.SceneNetworkInstanitate(_purple[2], pos, Quaternion.identity);
                            break;
                        case "water":
                            MasterManager.SceneNetworkInstanitate(_purple[3], pos, Quaternion.identity);
                            break;
                        case "start":
                            MasterManager.SceneNetworkInstanitate(_purple[4], pos, Quaternion.identity);
                            break;
                    }
                    break;
                case "":
                    break;


            }
        }

        _exists.Add(pos);

    }

    public void HighlightAllowedMove(GameObject obj, string c, string t)
    {
        GameObject go = null;
        //Debug.Log("create Prefab    " + c + t);
        Vector3 targetPos = new Vector3(obj.transform.position.x, 2.5f, obj.transform.position.z);
        if (t == "rain")
        {
            go = MasterManager.NetworkInstanitate(_rainPrefab, targetPos, Quaternion.identity);
        }
        else if (t == "swapPosition")
        {
            go = MasterManager.NetworkInstanitate(_swapPrefab[0] , targetPos, Quaternion.identity);
        }
        else if (t == "swapCharacter")
        {
            go = MasterManager.NetworkInstanitate(_swapPrefab[1], targetPos, Quaternion.identity);
        }
        else if (t == "wind")
        {
            go = MasterManager.NetworkInstanitate(_windPrefab, targetPos, Quaternion.identity);
        }
        else if (t == "portal")
        {
            go = MasterManager.NetworkInstanitate(portalPrefab[0], targetPos, Quaternion.identity);
        }
        else
        {
            switch (c)
            {
                case "red":
                    switch (t)
                    {
                        case "home":
                            go = MasterManager.NetworkInstanitate(red[0], targetPos, Quaternion.identity);
                            break;
                        case "rice":
                            go = MasterManager.NetworkInstanitate(red[1], targetPos, Quaternion.identity);
                            break;
                        case "tree":
                            go = MasterManager.NetworkInstanitate(red[2], targetPos, Quaternion.identity);
                            break;
                        case "water":
                            go = MasterManager.NetworkInstanitate(red[3], targetPos, Quaternion.identity);
                            break;
                        case "start":
                            go = MasterManager.NetworkInstanitate(red[4], targetPos, Quaternion.identity);
                            break;
                    }

                    break;
                case "yellow":
                    switch (t)
                    {
                        case "home":
                            go = MasterManager.NetworkInstanitate(yellow[0], targetPos, Quaternion.identity);
                            break;
                        case "rice":
                            go = MasterManager.NetworkInstanitate(yellow[1], targetPos, Quaternion.identity);
                            break;
                        case "tree":
                            go = MasterManager.NetworkInstanitate(yellow[2], targetPos, Quaternion.identity);
                            break;
                        case "water":
                            go = MasterManager.NetworkInstanitate(yellow[3], targetPos, Quaternion.identity);
                            break;
                        case "start":
                            go = MasterManager.NetworkInstanitate(yellow[4], targetPos, Quaternion.identity);
                            break;
                    }
                    break;
                case "black":
                    switch (t)
                    {
                        case "home":
                            go = MasterManager.NetworkInstanitate(_black[0], targetPos, Quaternion.identity);
                            break;
                        case "rice":
                            go = MasterManager.NetworkInstanitate(_black[1], targetPos, Quaternion.identity);
                            break;
                        case "tree":
                            go = MasterManager.NetworkInstanitate(_black[2], targetPos, Quaternion.identity);
                            break;
                        case "water":
                            go = MasterManager.NetworkInstanitate(_black[3], targetPos, Quaternion.identity);
                            break;
                        case "start":
                            go = MasterManager.NetworkInstanitate(_black[4], targetPos, Quaternion.identity);
                            break;
                    }
                    break;
                case "purple":
                    switch (t)
                    {
                        case "home":
                            go = MasterManager.NetworkInstanitate(_purple[0], targetPos, Quaternion.identity);
                            break;
                        case "rice":
                            go = MasterManager.NetworkInstanitate(_purple[1], targetPos, Quaternion.identity);
                            break;
                        case "tree":
                            go = MasterManager.NetworkInstanitate(_purple[2], targetPos, Quaternion.identity);
                            break;
                        case "water":
                            go = MasterManager.NetworkInstanitate(_purple[3], targetPos, Quaternion.identity);
                            break;
                        case "start":
                            go = MasterManager.NetworkInstanitate(_purple[4], targetPos, Quaternion.identity);
                            break;
                    }
                    break;
                case "":
                    break;


            }
        }

        _highlights.Add(go);


    }

    public void HighlightAllowedMove(GameObject obj, Tuple<string,string> data)
    {

        GameObject go = null;

         Vector3 targetPos = new Vector3(obj.transform.position.x, 2.5f, obj.transform.position.z);
        if (data.Item2 == "rain")
        {
             go = MasterManager.NetworkInstanitate(_rainPrefab, targetPos, Quaternion.identity);
        }
        else if (data.Item2 == "swapPosition")
        {
            go = MasterManager.NetworkInstanitate(_swapPrefab[0], targetPos, Quaternion.identity);
        }
        else if (data.Item2 == "swapCharacter")
        {
             go = MasterManager.NetworkInstanitate(_swapPrefab[1], targetPos, Quaternion.identity);
        }
        else if (data.Item2 == "wind")
        {
             go = MasterManager.NetworkInstanitate(_windPrefab, targetPos, Quaternion.identity);
        }
        else if (data.Item2 == "portal")
        {
             go = MasterManager.NetworkInstanitate(portalPrefab[0], targetPos, Quaternion.identity);
        }
        else
        {
            switch (data.Item1)
            {
                case "red":
                    switch (data.Item2)
                    {
                        case "home":
                             go = MasterManager.NetworkInstanitate(red[0], targetPos, Quaternion.identity);
                            break;
                        case "rice":
                             go = MasterManager.NetworkInstanitate(red[1], targetPos, Quaternion.identity);
                            break;
                        case "tree":
                             go = MasterManager.NetworkInstanitate(red[2], targetPos, Quaternion.identity);
                            break;
                        case "water":
                             go = MasterManager.NetworkInstanitate(red[3], targetPos, Quaternion.identity);
                            break;
                        case "start":
                            go = MasterManager.NetworkInstanitate(red[4], targetPos, Quaternion.identity);
                            break;
                    }

                    break;
                case "yellow":
                    switch (data.Item2)
                    {
                        case "home":
                             go = MasterManager.NetworkInstanitate(yellow[0], targetPos, Quaternion.identity);
                            break;
                        case "rice":
                             go = MasterManager.NetworkInstanitate(yellow[1], targetPos, Quaternion.identity);
                            break;
                        case "tree":
                             go = MasterManager.NetworkInstanitate(yellow[2], targetPos, Quaternion.identity);
                            break;
                        case "water":
                             go = MasterManager.NetworkInstanitate(yellow[3], targetPos, Quaternion.identity);
                            break;
                        case "start":
                            go = MasterManager.NetworkInstanitate(yellow[4], targetPos, Quaternion.identity);
                            break;
                    }
                    break;
                case "black":
                    switch (data.Item2)
                    {
                        case "home":
                            go = MasterManager.NetworkInstanitate(_black[0], targetPos, Quaternion.identity);
                            break;
                        case "rice":
                            go = MasterManager.NetworkInstanitate(_black[1], targetPos, Quaternion.identity);
                            break;
                        case "tree":
                            go = MasterManager.NetworkInstanitate(_black[2], targetPos, Quaternion.identity);
                            break;
                        case "water":
                            go = MasterManager.NetworkInstanitate(_black[3], targetPos, Quaternion.identity);
                            break;
                        case "start":
                            go = MasterManager.NetworkInstanitate(_black[4], targetPos, Quaternion.identity);
                            break;
                    }
                    break;
                case "purple":
                    switch (data.Item2)
                    {
                        case "home":
                            go = MasterManager.NetworkInstanitate(_purple[0], targetPos, Quaternion.identity);
                            break;
                        case "rice":
                            go = MasterManager.NetworkInstanitate(_purple[1], targetPos, Quaternion.identity);
                            break;
                        case "tree":
                            go = MasterManager.NetworkInstanitate(_purple[2], targetPos, Quaternion.identity);
                            break;
                        case "water":
                            go = MasterManager.NetworkInstanitate(_purple[3], targetPos, Quaternion.identity);
                            break;
                        case "start":
                            go = MasterManager.NetworkInstanitate(_purple[4], targetPos, Quaternion.identity);
                            break;
                    }
                    break;
                case "":
                    break;


            }
        }

        _highlights.Add(go);


    }

    public void DestroyHighlight()
    {
        if (!_highlights.Any())
        {
            return;
        }

        foreach (GameObject obj in _highlights)
        {
            if (obj == null)
                continue;
            if (obj.gameObject.GetPhotonView() == null)
                continue;
            if (!obj.gameObject.GetPhotonView().IsMine)
                continue;

            Debug.Log("Destroy " + obj.name);
            PhotonNetwork.Destroy(obj);

        }

    }

    public void HideHighlights()
    {
        if (oldhighlights.Count == 0)
            return;

        foreach (GameObject go in oldhighlights) go.SetActive(false);
    }

}
