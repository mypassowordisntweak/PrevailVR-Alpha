using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class InventoryController : NetworkBehaviour
{
    [SerializeField] private GameObject inventoryObject;
    [SerializeField] private Transform inventorySlotTransform;
    [SerializeField] private AudioClip openCloseSound;

    private Vector3 inventoryLastPos;
    private bool isInventoryOpen;
    private SlotController selectedSlot;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.openInventory += CmdOpenInventory;
        audioSource = GetComponent<AudioSource>();
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

    public bool AddItem(ItemObject item)
    {
        List<SlotController> list = GetSlotList(inventorySlotTransform);

        SlotController slot = (list.Where(x => x.HeldItem != null).Where(x => x.HeldItem.itemType == item.itemType)).FirstOrDefault();

        if (slot != null)
        {
            ItemObject tempItem = new ItemObject(slot.HeldItem);
            tempItem.amount += item.amount;
            slot.AddItemToSlot(tempItem);

            return true;
        }

        if (slot == null)
        {
            slot = list.FirstOrDefault(x => x.HeldItem == null);
            slot.AddItemToSlot(item);

            return true;
        }

        return false;
    }
    #endregion

    #region Container Functions
    public void OpenFull(GameObject player)
    {
        audioSource.clip = openCloseSound;
        audioSource.Play();

        PlayerController playerController = player.GetComponent<PlayerController>();
        inventoryLastPos = Camera.main.transform.position + Camera.main.transform.forward;

        if (isInventoryOpen)
        {
            isInventoryOpen = false;
            inventoryObject.transform.parent = transform.root;
            inventoryObject.SetActive(false);
            if (selectedSlot != null)
                selectedSlot = null;
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

    #region Network - Server
    [ServerRpc]
    void CmdOpenInventory(NetworkConnection conn)
    {
        Debug.Log("Server opening inventory for: " + conn.ClientId);
        RPCOpenFull(conn);
    }

    [ObserversRpc]
    private void RPCOpenFull(NetworkConnection conn)
    {
        Debug.Log("opening inventory for: " + conn.ClientId);
        OpenFull(conn.FirstObject.gameObject);
    }
    #endregion

}
