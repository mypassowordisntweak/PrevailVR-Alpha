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
        EventManager.openInventory += CmdOpenInventory;
        audioSource = GetComponent<AudioSource>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(base.IsOwner)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Inventory Functions
    private void UpdateSlots()
    {
        List<SlotController> list = GetSlotList(inventorySlotTransform);

        foreach(KeyValuePair<int, ItemObject> pair in inventoryList)
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

    public bool AddItem(ItemObject item)
    {

        List<SlotController> list = GetSlotList(inventorySlotTransform);

        SlotController slot = (list.Where(x => x.HeldItem != null).Where(x => x.HeldItem.ItemType == item.ItemType)).FirstOrDefault();

        if (slot != null)
        {
            ItemObject tempItem = new ItemObject(item);
            tempItem.Amount += item.Amount;

            if (inventoryList.Where(x => x.Key == slot.Index) != null)
                inventoryList[slot.Index] = tempItem;
            else
                inventoryList.Add(slot.Index, tempItem);

        } 
        else if (slot == null)
        {
            slot = list.FirstOrDefault(x => x.HeldItem == null);

            if (inventoryList.Where(x => x.Key == slot.Index) != null)
                inventoryList[slot.Index] = item;
            else
                inventoryList.Add(slot.Index, item);
        }

        UpdateSlots();
        return true;
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
    private void CmdOpenInventory(NetworkConnection conn)
    {
        RPCOpenFull(conn);
    }

    [ObserversRpc]
    private void RPCOpenFull(NetworkConnection conn)
    {
        OpenFull(conn.FirstObject.gameObject);
    }
    #endregion
}
