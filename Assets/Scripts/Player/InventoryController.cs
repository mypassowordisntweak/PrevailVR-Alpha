using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class InventoryController : NetworkBehaviour
{
    public SlotController SelectedSlot { get => selectedSlot; set => selectedSlot = value; }

    [SyncObject] private readonly SyncDictionary<int, ItemObject> inventoryList = new SyncDictionary<int, ItemObject>();

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
        audioSource = GetComponent<AudioSource>();

        inventoryList.OnChange += UpdateSlots;
        UpdateSlots(SyncDictionaryOperation.Complete, -1, null, false);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(!base.IsOwner)
        {
            inventoryObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Inventory Functions
    private void UpdateSlots(SyncDictionaryOperation op, int key, ItemObject itemObject, bool asServer)
    {
        List<SlotController> list = GetSlotList(inventorySlotTransform);

        foreach(SlotController slot in list)
        {
            if(!inventoryList.ContainsKey(slot.Index))
            {
                slot.EmptySlot();
            }
        }

        foreach (KeyValuePair<int, ItemObject> pair in inventoryList)
        {
            list[pair.Key].SetItemObject(pair.Value);
        }
    }

    private List<SlotController> GetSlotList(Transform slotParent)
    {
        List<SlotController> slotList = new List<SlotController>();
        for (int i = 0; i < inventorySlotTransform.childCount; i++)
        {
            SlotController slot = slotParent.GetChild(i).gameObject.GetComponent<SlotController>();
            slotList.Add(slot);
            slot.Index = i;

        }
        return slotList;
    }

    public bool AddItem(ItemObject item, int slotIndex = -1)
    {
        SlotController slot = null;
        List<SlotController> list = GetSlotList(inventorySlotTransform);
        List<SlotController> subList = list.Where(x => x.HeldItem != null).ToList();
        List<SlotController> listWithValidMatchingSlots = list.Where(x => x.HeldItem != null && x.HeldItem.ItemType == item.ItemType && x.HeldItem.Amount < x.HeldItem.StackSize).ToList();
        List<SlotController> listWithValidEmptySlots = list.Where(x => x.HeldItem == null).ToList();

        if(slotIndex > -1)
        {
            slot = GetSlotList(inventorySlotTransform).Where(x => x.Index == slotIndex).First();
        }

        //If we already selected a slot then make sure it isn't already occupied by a different item
        if(slot != null)
            if(slot.HeldItem != null)
                if(slot.HeldItem.ItemType != item.ItemType)
                    slot = null;

        //If we haven't already selected a slot then select one
        if(slot == null)
        {
            if (listWithValidMatchingSlots.Count > 0)
                slot = listWithValidMatchingSlots.First();
            else if (listWithValidEmptySlots.Count > 0)
                slot = listWithValidEmptySlots.First();
        }

        if (slot != null) //If we have a valid slot
        {
            if(slot.HeldItem != null) //If our valid slot does not have an item
            {
                ItemObject tempItem = new ItemObject(item);

                if ((slot.HeldItem.Amount + item.Amount) <= slot.HeldItem.StackSize)
                {
                    tempItem.Amount += item.Amount;

                    if (inventoryList.Where(x => x.Key == slot.Index).Count() > 0)
                        inventoryList[slot.Index] = tempItem;
                    else
                        inventoryList.Add(slot.Index, tempItem);
                }
                else
                {
                    int newAmount = slot.HeldItem.StackSize - slot.HeldItem.amount;
                    slot.HeldItem.amount = slot.HeldItem.StackSize;
                    item.Amount -= newAmount;

                    AddItem(item);
                }
            }
            else
            {
                if (inventoryList.Where(x => x.Key == slot.Index).Count() > 0)
                    inventoryList[slot.Index] = item;
                else
                    inventoryList.Add(slot.Index, item);
            }
        }
        else //If we do not have a valid slot
        {
            //Drop Item
        }

        return true;
    }

    public bool RemoveItem(ItemObject itemObject, int slotIndex = -1)
    {
        SlotController slot = null;
        if(slotIndex >= 0)
        {
            List<SlotController> slots = GetSlotList(inventorySlotTransform);
            slot = slots.Where(x => x.HeldItem != null && x.HeldItem.ItemType == itemObject.ItemType && x.HeldItem.Amount == itemObject.Amount).First();
        }

        var list = inventoryList.Where(x => x.Key == slot.Index);
        if(list.Count() > 0)
        {
            inventoryList.Remove(list.First());
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
        inventoryLastPos = playerController.PlayerCamera.transform.position + playerController.PlayerCamera.transform.forward;

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
            inventoryObject.transform.LookAt(2 * inventoryObject.transform.position - playerController.PlayerCamera.transform.position);
            inventoryObject.transform.parent = null;
            inventoryObject.SetActive(true);
            isInventoryOpen = true;
        }
    }
    #endregion

    #region Network - Server
    [ServerRpc]
    public void CmdOpenInventory(NetworkConnection conn = null)
    {
        RPCOpenFull(conn);
    }

    [ObserversRpc(BufferLast = true)]
    private void RPCOpenFull(NetworkConnection conn)
    {
        OpenFull(conn.FirstObject.gameObject);
    }

    [ServerRpc]
    public void CmdAddItem(ItemObject item, int slotIndex = -1)
    {
        AddItem(item, slotIndex);
    }

    [ServerRpc]
    public void CmdRemoveItem(ItemObject item, int slotIndex)
    {
        RemoveItem(item, slotIndex);
    }
    #endregion
}
