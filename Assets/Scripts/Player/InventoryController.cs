using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryObject;
    [SerializeField] private Transform inventorySlotTransform;

    private Vector3 inventoryLastPos;
    private bool isInventoryOpen;
    private SlotController selectedSlot;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.openInventory += OpenFull;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Inventory Functions
    private List<SlotController> GetSlotList(Transform slotParent)
    {
        List<SlotController> slotList = new List<SlotController>();
        for (int i = 0; i < inventorySlotTransform.childCount; i++)
        {
            slotList.Add(slotParent.GetChild(i).gameObject.GetComponent<SlotController>());
        }
        return slotList;
    }

    private void AddItem(Item item)
    {
        List<SlotController> list = GetSlotList(inventorySlotTransform);

        SlotController slot;

        slot = list.Where(x => x.heldItem.itemType == item.itemType).FirstOrDefault();
        if(slot == null)
            slot = list.Where(x => x.heldItem == null).FirstOrDefault();

        if (slot != null)
            slot.heldItem = item;
    }
    #endregion

    #region Container Functions
    public void OpenFull(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        inventoryLastPos = Camera.main.transform.position + Camera.main.transform.forward;

        if (isInventoryOpen)
        {
            isInventoryOpen = false;
            inventoryObject.transform.parent = transform.root;
            inventoryObject.SetActive(false);
            if (selectedSlot != null)
            {
                selectedSlot.isSelected = false;
                selectedSlot = null;
            }
        }
        else
        {
            inventoryObject.transform.position = inventoryLastPos;
            inventoryObject.transform.LookAt(2 * inventoryObject.transform.position - Camera.main.transform.position);
            inventoryObject.transform.parent = null;
            inventoryObject.SetActive(true);
            isInventoryOpen = true;
        }
    }
    #endregion

}
