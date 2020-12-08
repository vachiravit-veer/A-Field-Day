using System;
using System.Collections.Generic;

public class MyPlayer
{
    public class Tiletypes
    {
        public int rices;
        public int trees;
        public int waters;
        public int home;
        public List<int> cards;
    }


    public int x { get; set; }
    public int y { get; set; }
    public int rnd { get; set; }

    public string color { get; set; }

    public bool isAI { get; set; }
    public bool isProtected { get; set; }
    public Tiletypes Inventory;
    //internal object stateManager;

    private List<int> temprnd;
    public MyPlayer()
    {
        isProtected = false;

        Inventory = new Tiletypes();
        Inventory.cards = new List<int>();
        var num = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

        for (int i = 0; i < 3; i++)
        {
            int rndNext = num[UnityEngine.Random.Range(0, num.Count)];
            Inventory.cards.Add(rndNext);
            num.Remove(rndNext);

        }

        Inventory.rices = 0;
        Inventory.trees = 0;
        Inventory.waters = 0;
        Inventory.home = 0;

    }
    virtual public void DoAI()
    {
    }
    public int GetInventoryCount()
    {
        return (Inventory.rices + Inventory.trees + Inventory.waters + Inventory.home);
    }
    public List<int> ReturnInventoryCard()
    {
        return Inventory.cards;
    }
    public void RemoveInventoryCard(int card)
    {
        Inventory.cards.Remove(card);
    }
    public bool FindInventoryCard(int id)
    {
        if (Inventory.cards.Contains(id))
        {
            return true;
        }
        return false;
    }

    public bool IsWinner()
    {
        if (Inventory.rices >= 2 && Inventory.trees >= 2 && Inventory.waters >= 2 && Inventory.home >= 1)
        {
            return true;
        }
        return false;
    }
}
