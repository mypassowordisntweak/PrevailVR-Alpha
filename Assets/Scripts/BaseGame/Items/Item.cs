using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object.Synchronizing;

[System.Serializable]
public class Item
{
    public enum ItemType
    {
        Null,
        Wood,
        Stone,
        Charcoal,
        IronOre,
        IronPureChunk,
        IronIngot,
        IronShard,
        StonePickaxe,
        StoneHatchet,
        Hammer,
        BaseHeart,
        LargeContainer,
        Blueprint,
        Rock,
        AssaultRifle,
        RifleAmmoStandard,
    }
    public ItemType itemType;

    public int amount;
    public int stackSize;
    public int slotNumber = -1;
    public bool isEquiptable;
    public bool isCraftable;
    public bool isUseable;
    public bool isAmmo;
    public bool isGun;
    public string itemName;
    public string itemDescription;
    public Sprite itemSprite;
}

//[System.Serializable]
//public class SyncListItem : SyncList<Item> { }