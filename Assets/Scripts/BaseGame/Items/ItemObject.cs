using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object.Synchronizing;
using UnityEngine.UI;



[System.Serializable]
public class ItemObject
{
    public Item item;
    public int amount;

    public ItemType ItemType { get => item.itemType; }
    public Sprite ItemSprite { get => item.itemSprite; }
    public int Amount { get => amount; set => amount = value; }

    public ItemObject()
    {
        item = new Item();
        amount = 0;
    }

    public ItemObject(Item newItem)
    {
        item = newItem;
        amount = 0;
    }

    public ItemObject(ItemObject newItemObject)
    {
        item = newItemObject.item;
        amount = newItemObject.amount;
    }
}

//[System.Serializable]
//public class SyncListItem : SyncList<Item> { }