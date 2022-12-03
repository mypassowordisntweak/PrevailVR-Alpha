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

        if (IsOwner)
            GamemodeTest.instance.LocalPlayer = base.Owner;
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
                }
            }
        }
    }

    public void InventoryPressed()
    {
        inventoryController.CmdOpenInventory(base.Owner);
    }

    public void GrabPressed(GameObject device)
    {
        GameObject deviceForward = isDesktop ? Camera.main.gameObject : device;

        Debug.DrawLine(deviceForward.transform.position, deviceForward.transform.position + (deviceForward.transform.forward * 10), Color.green, 10f);

        RaycastHit hit;
        if (Physics.Raycast(deviceForward.transform.position, deviceForward.transform.forward, out hit, 10f))
        {
            if (hit.transform != null)
            {
                if (hit.transform.GetComponent<IGrabbable>() != null)
                {
                    if (hit.transform.GetComponent<IGrabbable>().Grab(device))
                    {
                        device.GetComponent<ItemSocket>().HeldObject = hit.transform.gameObject;
                    }
                }
            }
        }
    }

    public void InteractPressed(GameObject device)
    {
        GameObject deviceForward = isDesktop ? Camera.main.gameObject : device;

        Debug.DrawLine(deviceForward.transform.position, deviceForward.transform.position + (deviceForward.transform.forward * 10), Color.green, 10f);

        RaycastHit hit;
        if (Physics.Raycast(deviceForward.transform.position, deviceForward.transform.forward, out hit, 10f))
        {
            if (hit.transform != null)
            {
                if (hit.transform.GetComponent<IInteractable>() != null)
                {
                    hit.transform.GetComponent<IInteractable>().Interact(device);
                }
            }
        }
    }
}
