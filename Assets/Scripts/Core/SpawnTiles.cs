using UnityEngine;
using Photon.Pun;
using System.Linq;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;


public class SpawnTiles : MonoBehaviour
{


    public Entity[,] Tiles;

    public static GameObject[,] TilesName;

    private int MaxSize;

    private void Awake()
    {
        MaxSize = (int)PhotonNetwork.CurrentRoom.CustomProperties["maxSize"];
        if(PhotonNetwork.IsMasterClient)
            Tiles = new Entity[MaxSize, MaxSize];


        TilesName = new GameObject[MaxSize, MaxSize];

    }

    private List<string[]> SpawnInside(string[] c, string[] t)
    {
        List<string[]> temp = new List<string[]>();
        for (int i = 0; i < c.Length; i++)
        {
            for (int j = 0; j < t.Length; j++)
            {
                string[] multiArr = new string[2];
                multiArr[0] = c[i];
                multiArr[1] = t[j];
                temp.Add(multiArr);
            }

        }
        return temp.OrderBy(g => Guid.NewGuid()).ToList();

    }


    public void Spawn()
    {

        string[] c = new string[] { "red", "yellow" };
        string[] t = new string[] { "home","home", "water","water", "water", "rice", "rice", "rice","tree", "tree" ,"tree"};
        //22 14
        List<string[]> rng = SpawnInside(c, t);

        List<string> typerandom = new List<string> { "wind", "wind", "wind", "wind", "wind", "wind", "wind","wind","portal", "portal", "portal", "portal", "swapCharacter", "swapCharacter" };
        typerandom = typerandom.OrderBy(g => Guid.NewGuid()).ToList();

        int index;

        int a = 0;
        int b = 0;

        foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("Boards").OrderBy(obj => obj.name, new AlphanumComparatorFast()).ToArray())
        {

     
            if (b > 5)
            {

                a++;
                b = 0;

            }

            TilesName[a, b] = Obj;


            if (PhotonNetwork.IsMasterClient)
            {

                Tiles[a, b] = new Entity();
                if (Random.value < 0.6f | typerandom.Count == 0 && rng.Count > 0)
                {                 
                    string[] _t = rng[Random.Range(0, rng.Count)];
                    Tiles[a, b].color = _t[0];
                    Tiles[a, b].Tiletype = _t[1];
                    rng.Remove(_t);
                    
                }
                else
                {

                    index = Random.Range(0, typerandom.Count);
                    Tiles[a, b].Tiletype = typerandom[index];
                    Tiles[a, b].color = string.Empty;
                    typerandom.RemoveAt(index);
                }

                Tiles[a, b].isAvaliable = true;
               
            }
      
            b++;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
            prop.Add("TilesData", ProtobufManager.Serialize(Tiles));
            PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
        }


    }

    public void SpawnOld()
    {
    
        string[] color = new string[] { "red", "yellow", "black", "purple" };
        string[] type = new string[] { "home", "water", "water", "rice", "rice", "tree", "tree" };

        List<string[]> rng = SpawnInside(color, type);

        string[] multiArr = new string[2];


        List<string> typerandom = new List<string> { "wind", "wind", "portal", "portal", "portal", "portal", "swapCharacter", "swapCharacter" };
        typerandom = typerandom.OrderBy(g => Guid.NewGuid()).ToList();

        int index;
        int a = 0;
        int b = 0;

        string[] outsideTiletype = new string[] { "water", "rice", "tree", "start" };
        string[] outsideSpecialTiletype = new string[] { "swapPosition", "swapPosition", "swapCharacter", "rain", "rain", "grass", "grass","grass" };

        List<string[]> _spawnoutside = new List<string[]>();
        for (int i = 0; i < color.Length; i++)
        {

            for (int j = 0; j < outsideTiletype.Length; j++)
            {
                multiArr = new string[2];
                multiArr[0] = color[i];
                multiArr[1] = outsideTiletype[j];
                _spawnoutside.Add(multiArr);
            }

        }

        for (int i = 0; i < outsideSpecialTiletype.Length; i++)
        {
            multiArr = new string[2];
            multiArr[0] = string.Empty;
            multiArr[1] = outsideSpecialTiletype[i];
            _spawnoutside.Add(multiArr);

        }
        
        foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("Boards").OrderBy(obj => obj.name, new AlphanumComparatorFast()).ToArray())
        {
            
            if (b > 7)
            {
                a++;
                b = 0;
            }

  
            if (PhotonNetwork.IsMasterClient)
            {
                Tiles[a, b] = new Entity();
                

                if (a==0 && b==0)
                {
                    Tiles[0, 0].color = string.Empty;
                    Tiles[0, 0].Tiletype = string.Empty;
                }
                else if (a == 0 && b == 7)
                {
                    Tiles[0, 7].color = string.Empty;
                    Tiles[0, 7].Tiletype = string.Empty;
                }
                else if(a == 7 && b == 0)
                {
                    Tiles[7, 0].color = string.Empty;
                    Tiles[7, 0].Tiletype = string.Empty;
                }
                else if(a == 7 && b == 7)
                {
                    Tiles[7, 7].color = string.Empty;
                    Tiles[7, 7].Tiletype = string.Empty;
                }
                else if(a == 0 && Enumerable.Range(1, 6).Contains(b) || a == 7 && Enumerable.Range(1, 6).Contains(b) || Enumerable.Range(1, 6).Contains(a) && b == 7 || Enumerable.Range(1, 6).Contains(a) && b == 0)
                {
                    string[] _t = _spawnoutside[Random.Range(0, _spawnoutside.Count)];
                    Tiles[a, b].color = _t[0];
                    Tiles[a, b].Tiletype = _t[1];
                }
                else if (Random.value < 0.5f | typerandom.Count == 0 && rng.Count > 0)
                {
                    //Debug.Log("rng count" + rng.Count);
                    string[] _t = rng[Random.Range(0, rng.Count)];
                    Tiles[a, b].color = _t[0];
                    Tiles[a, b].Tiletype = _t[1];
                    rng.Remove(_t);
                }
                else
                {
                    index = Random.Range(0, typerandom.Count);
                    Tiles[a, b].Tiletype = typerandom[index];
                    Tiles[a, b].color = string.Empty;
                    typerandom.RemoveAt(index);
                }

                Tiles[a, b].isAvaliable = true;

            }

            Debug.Log("Spawn " + Tiles[a, b].color + ","+ Tiles[a, b].Tiletype + "on" + Obj.name);

            TilesName[a, b] = Obj;

            b++;
        }


        if (PhotonNetwork.IsMasterClient)
        {      
            ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
            prop.Add("TilesData", ProtobufManager.Serialize(Tiles));
            PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
        }   

    }

  
}
