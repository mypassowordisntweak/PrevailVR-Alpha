using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;

public class SlotController : MonoBehaviour
{
    public Image foreground;
    public Image background;
    public Text slotAmount;
    public int slotNumber;
    public bool isSelected;
    public bool isLocked;
    public Item heldItem;

    public bool isEquipmentSlot;
    public int slotSize;

    public string UniqueString;
    private float timeDelay;

    // Start is called before the first frame update
    void Start()
    {
        heldItem = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected) 
        {
            //Debug.Log("WE SELECTED");
            Color c = background.color;
            c.g = 0.6f;
            background.color = c;
        }
        else
        {
            Color c = background.color;
            c.g = 0.431f;
            background.color = c;
        }
    }

    public void UpdateSlot(Item item)
    {
        foreground.sprite = item.itemSprite;

        //isSelected = false;
        heldItem = item;

        if (heldItem.itemType != Item.ItemType.Null && !isEquipmentSlot)
        {
            if (heldItem.amount > 1)
            {
                slotAmount.text = heldItem.amount.ToString();
            }
            else
            {
                slotAmount.text = "";
            }
        }

        Color c = foreground.color;
        c.a = 1f;
        foreground.color = c;
    }

    public void EmptySlot()
    {
        //Debug.Log("EMPTYING SLOT");
        isSelected = false;
        heldItem = null;
        foreground.sprite = null;
        Color c = foreground.color;
        c.a = 0;
        foreground.color = c;

        if(!isEquipmentSlot)
        {
            slotAmount.text = "";
        }
    }
}
