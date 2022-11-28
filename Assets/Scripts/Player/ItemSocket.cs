using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSocket : MonoBehaviour
{
    public ItemObject HeldItem { get => heldItem; set => heldItem = value; }
    public GameObject HeldObject { get => heldObject; set => heldObject = value; }

    private ItemObject heldItem;
    private GameObject heldObject;
}
