using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemList", menuName = "ScriptableObjects/ItemList", order = 2)]
public class ItemList : ScriptableObject
{
    public List<Item> itemList;
}
