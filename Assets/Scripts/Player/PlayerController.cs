using FishNet.Object;
using FishNet.Object.Prediction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public struct ReplicateData
    {
        public int? _selectedSlotIndex;
        public int selectedSlotIndex { get { return _selectedSlotIndex ?? -1; } set { _selectedSlotIndex = value; } }
    }

    public struct ReconcileData
    {
        public int? _selectedSlotIndex;

        public int selectedSlotIndex { get { return _selectedSlotIndex ?? -1; } set { _selectedSlotIndex = value; } }
    }

    public string PlayerName { get => playerName; }
    public Camera PlayerCamera { get => IsOwner ? Camera.main : (isDesktop ? GetComponent<DesktopMovement>().FPCamera : Camera.main); }
    public SlotController SelectedSlot { get => selectedSlot; set => selectedSlot = value; }


    private bool isDesktop;
    private string playerName;

    private InventoryController inventoryController;

    private SlotController selectedSlot;
    private int _selectedSlotIndexQueue;

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

    //Client Side Prediction ------------------------------------------------------------------------------------------------------------------------
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        base.TimeManager.OnTick += TimeManager_OnTick;
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        if (base.TimeManager != null)
            base.TimeManager.OnTick -= TimeManager_OnTick;
    }

    private void TimeManager_OnTick()
    {
        if (base.IsOwner)
        {
            Reconcile(default, false);
            BuildActions(out ReplicateData md);
            Replicate(md, false);
        }

        if (base.IsServer)
        {
            Replicate(default, true);
            ReconcileData rd = new ReconcileData()
            {
                selectedSlotIndex = selectedSlot != null ? selectedSlot.Index : -1
            };
            Reconcile(rd, true);
        }
    }

    private void BuildActions(out ReplicateData moveData)
    {
        moveData = default;
        moveData.selectedSlotIndex = _selectedSlotIndexQueue;

        //Unset queued values.
        _selectedSlotIndexQueue = -1;
    }

    [Replicate]
    private void Replicate(ReplicateData moveData, bool asServer, bool replaying = false)
    {
        if (inventoryController == null)
            return;

        //If jumping move the character up one unit.
        if (moveData.selectedSlotIndex > -1)
        {
            if(selectedSlot != null)
            {
                if(selectedSlot.Index != moveData.selectedSlotIndex)
                {
                    selectedSlot.Hovering = false;
                }
            }

            selectedSlot = inventoryController.GetSlot(moveData.selectedSlotIndex);
            selectedSlot.Hovering = true;

            if (asServer || IsServer)
                RpcUpdateSelectedSlot(moveData.selectedSlotIndex);
        }
        else
        {
            if(selectedSlot != null)
            {
                selectedSlot.Hovering = false;
                selectedSlot = null;
            }
        }
    }

    [Reconcile]
    private void Reconcile(ReconcileData recData, bool asServer)
    {
        //Reset the client to the received position. It's okay to do this
        //even if there is no de-synchronization.

        if (selectedSlot != null)
            if (selectedSlot != inventoryController.GetSlot(recData.selectedSlotIndex))
                selectedSlot.Hovering = false;

        SlotController slot = inventoryController.GetSlot(recData.selectedSlotIndex);
        selectedSlot = slot;
        if(slot != null)
            selectedSlot.Hovering = slot.Hovering;

    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------


    public void CheckHover(GameObject device)
    {
        RaycastHit hit;
        if (Physics.Raycast(device.transform.position, device.transform.forward, out hit, 10f))
        {
            if (hit.transform != null)
            {
                if (hit.transform.GetComponent<ISlot>() != null)
                {
                    if (hit.transform.root.GetComponent<NetworkObject>() != null)
                        if (hit.transform.root != inventoryController.InventoryObject.transform)
                            return;

                    if (hit.transform.GetComponent<SlotController>() != null)
                    {
                        _selectedSlotIndexQueue = hit.transform.GetComponent<SlotController>().Index;

                        //selectedSlot = hit.transform.GetComponent<SlotController>();
                    }
                }
                else
                {
                    _selectedSlotIndexQueue = -1;

                    //selectedSlot = null;
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
            if (selectedSlot != null)
            {
                if (selectedSlot.HeldItem != null && device.HeldItem == null)
                {
                    CmdCallGrabFromInventory(selectedSlot.Index, device.GetComponent<NetworkObject>());
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
                    if (selectedSlot != null)
                    {
                        CmdCallReleaseIntoInventory(device.GetComponent<NetworkObject>());
                    }
                    else
                    {
                        CmdCallRelease(device.NetworkObject);
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

    [ObserversRpc]
    public void RpcUpdateSelectedSlot(int SlotIndex)
    {
        if (selectedSlot != null)
            if (selectedSlot != inventoryController.GetSlot(SlotIndex))
                selectedSlot.Hovering = false;

        selectedSlot = inventoryController.GetSlot(SlotIndex);
        selectedSlot.Hovering = true;
    }

    [ServerRpc]
    public void CmdCallGrabFromInventory(int SlotIndex, NetworkObject device)
    {
        GameObject worldGameObject = Instantiate(HersuaGlobal.CreateDroppedItem(), device.transform.position, device.transform.rotation);
        base.Spawn(worldGameObject);
        WorldObject worldObject = worldGameObject.GetComponent<WorldObject>();
        worldObject.HeldItem = selectedSlot.HeldItem;
        worldObject.Grab(device.GetComponent<ItemSocket>());
        device.GetComponent<ItemSocket>().HeldWorldObject = worldObject;

        inventoryController.RemoveItem(selectedSlot.HeldItem, selectedSlot.Index);

        RpcCallGrabFromInventory(SlotIndex, device, worldGameObject.GetComponent<NetworkObject>());
    }

    [ObserversRpc]
    public void RpcCallGrabFromInventory(int SlotIndex, NetworkObject device, NetworkObject instantiatedObject)
    {
        if (base.IsServer)
            return;

        GameObject worldGameObject = instantiatedObject.gameObject;
        WorldObject worldObject = worldGameObject.GetComponent<WorldObject>();
        worldObject.HeldItem = selectedSlot.HeldItem;
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
        Debug.Log("SelectedSlot " + selectedSlot);

        inventoryController.AddItem(deviceItemSocket.HeldWorldObject.HeldItem, selectedSlot.Index);
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
    public void CmdCallRelease(NetworkObject device)
    {
        WorldObject worldObject = null;
        if (device.GetComponent<ItemSocket>().HeldWorldObject != null)
            worldObject = device.GetComponent<ItemSocket>().HeldWorldObject;
        else
            return;

        worldObject.Release(device.GetComponent<ItemSocket>());
        device.GetComponent<ItemSocket>().HeldWorldObject = null;

        RpcCallRelease(device);
    }

    [ObserversRpc]
    public void RpcCallRelease(NetworkObject device)
    {
        WorldObject worldObject = null;
        if (device.GetComponent<ItemSocket>().HeldWorldObject != null)
            worldObject = device.GetComponent<ItemSocket>().HeldWorldObject;
        else
            return;

        worldObject.Release(device.GetComponent<ItemSocket>());
        device.GetComponent<ItemSocket>().HeldWorldObject = null;
    }
}
