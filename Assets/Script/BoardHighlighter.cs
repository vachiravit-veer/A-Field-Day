using System.Collections.Generic;
using UnityEngine;

public class BoardHighlighter : MonoBehaviour
{

    public GameObject highlightPrefab, windPrefab,swapPrefab;
    public GameObject[] red, yellow, portalPrefab;
    public static BoardHighlighter Instance { get; set; }

    private List<GameObject> highlights, highlights2;
    private int count;
    private void Start()
    {
        Instance = this;
        highlights = new List<GameObject>();
        highlights2 = new List<GameObject>();
        count = 0;
    }

    private GameObject GetHighlightObject()
    {
        GameObject go = highlights.Find(g => !g.activeSelf);
        if (go == null)
        {
            go = Instantiate(highlightPrefab);

            highlights.Add(go);
        }
        return go;
    }

    public void HighlightAllowedMoves(List<GameObject> moves)
    {
        Debug.Log("moves " + moves.Count);

        foreach (GameObject obj in moves)
        {

            GameObject go = GetHighlightObject();
            go.SetActive(true);
            if (go == null)
                Debug.Log("create highlight error");
            go.transform.position = new Vector3(obj.transform.position.x, 2.5f, obj.transform.position.z);

        }
    }

    public void HighlightAllowedSingleObject(GameObject obj, string type)
    {
        GameObject go = new GameObject();
        switch (type)
        {
            case "portal":
                switch(count)
                {
                    case 0:
                        go = Instantiate(portalPrefab[0]);
                        break;
                    case 1:
                        go = Instantiate(portalPrefab[1]);
                        break;
                    case 2:
                        go = Instantiate(portalPrefab[2]);
                        break;
                    case 3:
                        go = Instantiate(portalPrefab[3]);
                        break;
                }
                count++;
                Debug.Log("create portal prefab");
                break;
            case "wind":
                 go = Instantiate(windPrefab);
                break;
            case "swap":
                  go = Instantiate(swapPrefab);              
                 break;
        }
        go.SetActive(true);
        go.transform.position = new Vector3(obj.transform.position.x, 2.5f, obj.transform.position.z);
    }

    public void HighlightAllowedMove(GameObject obj, string c, string t)
    {
        Debug.Log("create Prefab    " + c + t);
        GameObject go = new GameObject();
        if (t == "swap")
        {
            go = Instantiate(swapPrefab);
        }
        else if (t == "wind")
        {
            go = Instantiate(windPrefab);
        }
        else if (t == "port")
        {
            go = Instantiate(portalPrefab[0]);
        }
        else
        {
            switch (c)
            {
                case "red":
                    switch (t)
                    {
                        case "home":
                            go = Instantiate(red[0]);
                            break;
                        case "rice":
                            go = Instantiate(red[1]);
                            break;
                        case "water":
                            go = Instantiate(red[2]);
                            break;
                        case "tree":
                            go = Instantiate(red[3]);
                            break;
                    }

                    break;
                case "yellow":
                    switch (t)
                    {
                        case "home":
                            go = Instantiate(yellow[0]);
                            break;
                        case "rice":
                            go = Instantiate(yellow[1]);
                            break;
                        case "water":
                            go = Instantiate(yellow[2]);
                            break;
                        case "tree":
                            go = Instantiate(yellow[3]);
                            break;
                    }
                    break;
                case "null":
                    break;


            }
        }

        highlights2.Add(go);
        go.SetActive(true);
        go.transform.position = new Vector3(obj.transform.position.x, 2.5f, obj.transform.position.z);
    }
    public void DestroyHighlights()
    {
        Debug.Log("destroying highlight" + highlights2.Count);
        foreach (GameObject go in highlights2) go.SetActive(false);
    }
    public void HideHighlights()
    {

        foreach (GameObject go in highlights) go.SetActive(false);
    }
}
