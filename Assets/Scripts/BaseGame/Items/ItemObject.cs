using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object.Synchronizing;
using UnityEngine.UI;



[System.Serializable]
public class ItemObject
{
    [SerializeField] private Item item;
    [SerializeField] public int amount;

    public Item Item { get => item; }
    public int StackSize { get => item.stackSize; }
    public ItemType ItemType { get => item != null ? item.itemType : ItemType.Null; }
    public Sprite ItemSprite { get => item.itemSprite; }
    public int Amount { get => amount; set => amount = value; }

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