using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour, IGrabbable
{
    public ItemObject HeldItem { get => heldItem; set => heldItem = value; }

    [SerializeField] private ItemObject heldItem;

    public bool Grab(GameObject device)
    {
        InventoryController playerInventory = device.transform.root.GetComponent<InventoryController>();

        if(playerInventory != null)
        {

        }

        return false;
    }
}
