using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour, IGrabbable
{
    public ItemObject HeldItem { get => heldItem; set => heldItem = value; }

    [SerializeField] private ItemObject heldItem;
    [SerializeField] private AudioClip audioClip;

    public bool Grab(GameObject device)
    {
        InventoryController playerInventory = device.transform.root.GetComponent<InventoryController>();

        if (playerInventory != null)
        {
            if(playerInventory.AddItem(heldItem))
            {
                GameObject tempObj = new GameObject();
                tempObj.transform.position = transform.position;
                tempObj.name = "PICKUP SOUND";
                tempObj.AddComponent<AudioSource>().clip = audioClip;
                tempObj.GetComponent<AudioSource>().Play();
                Destroy(tempObj, 2);
                Destroy(gameObject);
                return true;
            }
        }

        return false;
    }
}
