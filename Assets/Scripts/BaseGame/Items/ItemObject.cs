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

    public ItemType itemType { get => item.itemType; }
    public Sprite itemSprite { get => item.itemSprite; }
}

//[System.Serializable]
//public class SyncListItem : SyncList<Item> { }