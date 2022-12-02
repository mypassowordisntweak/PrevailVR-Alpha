using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{
    public ItemType itemType;
    public int stackSize;
    public string itemName;
    public string itemDescription;
    public Sprite itemSprite;
}
