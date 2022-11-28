using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;

public class SlotController : MonoBehaviour, ISlot
{
    public bool Hovering { get => isHovering; set => isHovering = value; }
    public ItemObject HeldItem { get => heldItem; set => heldItem = value; }

    [SerializeField] private Image foreground;
    [SerializeField] private Image background;
    [SerializeField] private Text slotAmount;

    private bool isEquipmentSlot;
    private bool isHovering;
    private bool isHoveringPrev;
    private bool isLocked;
    private float timeDelay;
    private ItemObject heldItem;

    private AudioSource slotAudio;

    // Start is called before the first frame update
    void Start()
    {
        heldItem = null;
        slotAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isHoveringPrev != isHovering && isHovering)
            slotAudio.Play();

        if (isHovering) 
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

        isHoveringPrev = isHovering;
        isHovering = false;
    }

    public void UpdateSlot(ItemObject item)
    {
        foreground.sprite = item.itemSprite;

        //isSelected = false;
        heldItem = item;

        if (heldItem.itemType != ItemType.Null && !isEquipmentSlot)
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
        isHovering = false;
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

    public void SlotClick()
    {

    }
}
