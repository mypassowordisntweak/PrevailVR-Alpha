using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public string PlayerName { get => playerName; }
    public Camera PlayerCamera { get => IsOwner ? Camera.main : (isDesktop ? GetComponent<DesktopMovement>().FPCamera : Camera.main); }

    private bool isDesktop;
    private string playerName;

    private InventoryController inventoryController;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<DesktopMovement>() != null)
            isDesktop = true;

        playerName = Random.Range(1, 10000).ToString();
        gameObject.name = playerName;

        inventoryController = GetComponent<InventoryController>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CheckHover(GameObject device)
    {
        RaycastHit hit;
        if (Physics.Raycast(device.transform.position, device.transform.forward, out hit, 10f))
        {
            if (hit.transform != null)
            {
                if (hit.transform.GetComponent<ISlot>() != null)
                {
                    hit.transform.GetComponent<ISlot>().Hovering = true;
                    if (hit.transform.GetComponent<SlotController>() != null)
                    {
                        inventoryController.SelectedSlot = hit.transform.GetComponent<SlotController>();
                    }
                }
                else
                {
                    inventoryController.SelectedSlot = null;
                }
            }
        }
    }

    public void InventoryPressed()
    {
        inventoryController.CmdOpenInventory(base.Owner);
    }

    public void GrabPressed(bool IsDown, ItemSocket device)
    {
        GameObject deviceForward = isDesktop ? Camera.main.gameObject : device.gameObject;

        Debug.DrawLine(deviceForward.transform.position, deviceForward.transform.position + (deviceForward.transform.forward * 10), Color.green, 10f);

        if (IsDown)
        {
            if (inventoryController.SelectedSlot != null)
            {
                if (inventoryController.SelectedSlot.HeldItem != null && device.HeldItem == null)
                {
                    DroppedItemBag worldObject = Instantiate(inventoryController.SelectedSlot.HeldItem.Item.itemGameObject, device.transform.position, device.transform.rotation).GetComponent<DroppedItemBag>();
                    worldObject.HeldItem = inventoryController.SelectedSlot.HeldItem;
                    worldObject.CmdGrab(device.transform);

                    inventoryController.CmdRemoveItem(inventoryController.SelectedSlot.HeldItem, inventoryController.SelectedSlot.Index);
                }
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(deviceForward.transform.position, deviceForward.transform.forward, out hit, 10f))
                {
                    if (hit.transform != null)
                    {
                        if (hit.transform.GetComponent<DroppedItemBag>() != null)
                        {
                            CmdCallGrab(device.GetComponent<NetworkObject>(),  hit.transform.GetComponent<NetworkObject>());
                        }
                    }
                }
            }
        }
        else
        {
            if(device.HeldWorldObject != null)
            {
                if(inventoryController.SelectedSlot != null)
                {
                    inventoryController.CmdAddItem(device.HeldWorldObject.HeldItem, inventoryController.SelectedSlot.Index);
                    Destroy(device.HeldWorldObject.gameObject);
                }
                else if (device.HeldWorldObject.Release(device))
                {
                    device.HeldWorldObject = null;
                }
            }
        }
    }

    public void InteractPressed(bool IsDown, ItemSocket device)
    {
        if(device.HeldWorldObject != null)
        {
            device.HeldWorldObject.Interact(device.gameObject);
        }
    }

    [ServerRpc]
    public void CmdCallGrab(NetworkObject device, NetworkObject itemToGrab)
    {
        DroppedItemBag worldObject = itemToGrab.transform.GetComponent<DroppedItemBag>();
        worldObject.Grab(device.GetComponent<ItemSocket>());
        device.GetComponent<ItemSocket>().HeldWorldObject = worldObject;
    }
}
