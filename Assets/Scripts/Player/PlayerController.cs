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
                    CmdCallGrabFromInventory(inventoryController.SelectedSlot.Index, device.GetComponent<NetworkObject>());
                }
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(deviceForward.transform.position, deviceForward.transform.forward, out hit, 10f))
                {
                    if (hit.transform != null)
                    {
                        if (hit.transform.GetComponent<WorldObject>() != null)
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
                if (device.HeldWorldObject.heldItem != null)
                {
                    if (inventoryController.SelectedSlot != null)
                    {
                        CmdCallReleaseIntoInventory(device.GetComponent<NetworkObject>());
                    }
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
    public void CmdCallGrabFromInventory(int SlotIndex, NetworkObject device)
    {
        GameObject worldGameObject = Instantiate(HersuaGlobal.CreateDroppedItem(), device.transform.position, device.transform.rotation);
        base.Spawn(worldGameObject);
        WorldObject worldObject = worldGameObject.GetComponent<WorldObject>();
        worldObject.HeldItem = inventoryController.SelectedSlot.HeldItem;
        worldObject.Grab(device.GetComponent<ItemSocket>());
        device.GetComponent<ItemSocket>().HeldWorldObject = worldObject;

        inventoryController.RemoveItem(inventoryController.SelectedSlot.HeldItem, inventoryController.SelectedSlot.Index);

        RpcCallGrabFromInventory(SlotIndex, device, worldGameObject.GetComponent<NetworkObject>());
    }

    [ObserversRpc]
    public void RpcCallGrabFromInventory(int SlotIndex, NetworkObject device, NetworkObject instantiatedObject)
    {
        if (base.IsServer)
            return;

        GameObject worldGameObject = instantiatedObject.gameObject;
        WorldObject worldObject = worldGameObject.GetComponent<WorldObject>();
        worldObject.HeldItem = inventoryController.SelectedSlot.HeldItem;
        worldObject.Grab(device.GetComponent<ItemSocket>());
        device.GetComponent<ItemSocket>().HeldWorldObject = worldObject;
    }


    [ServerRpc]
    public void CmdCallReleaseIntoInventory(NetworkObject device)
    {
        ItemSocket deviceItemSocket = device.GetComponent<ItemSocket>();

        Debug.Log("Device " + device);
        Debug.Log("WorldObject " + device.GetComponent<ItemSocket>().HeldWorldObject);

        Debug.Log("InventoryController " + inventoryController);
        Debug.Log("SelectedSlot " + inventoryController.SelectedSlot);

        inventoryController.AddItem(deviceItemSocket.HeldWorldObject.HeldItem, inventoryController.SelectedSlot.Index);
        Destroy(deviceItemSocket.HeldWorldObject.gameObject);
        deviceItemSocket.HeldWorldObject = null;

        RpcCallReleaseIntoInventory(device);
    }

    [ObserversRpc]
    public void RpcCallReleaseIntoInventory(NetworkObject device)
    {
        ItemSocket deviceItemSocket = device.GetComponent<ItemSocket>();
        deviceItemSocket.HeldWorldObject = null;
    }

    [ServerRpc]
    public void CmdCallGrab(NetworkObject device, NetworkObject itemToGrab)
    {
        WorldObject worldObject = itemToGrab.transform.GetComponent<WorldObject>();
        worldObject.Grab(device.GetComponent<ItemSocket>());
        device.GetComponent<ItemSocket>().HeldWorldObject = worldObject;

        RpcCallGrab(device, itemToGrab);
    }

    [ObserversRpc]
    public void RpcCallGrab(NetworkObject device, NetworkObject itemToGrab)
    {
        WorldObject worldObject = itemToGrab.transform.GetComponent<WorldObject>();
        worldObject.Grab(device.GetComponent<ItemSocket>());
        device.GetComponent<ItemSocket>().HeldWorldObject = worldObject;
    }

    [ServerRpc]
    public void CmdCallRelease(NetworkObject device, NetworkObject itemToGrab)
    {
        WorldObject worldObject = itemToGrab.transform.GetComponent<WorldObject>();
        worldObject.Release(device.GetComponent<ItemSocket>());
        device.GetComponent<ItemSocket>().HeldWorldObject = null;

        RpcCallRelease(device, itemToGrab);
    }

    [ObserversRpc]
    public void RpcCallRelease(NetworkObject device, NetworkObject itemToGrab)
    {
        WorldObject worldObject = itemToGrab.transform.GetComponent<WorldObject>();
        worldObject.Release(device.GetComponent<ItemSocket>());
        device.GetComponent<ItemSocket>().HeldWorldObject = null;
    }
}
