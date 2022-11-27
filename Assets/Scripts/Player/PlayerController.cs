using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isDesktop;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<DesktopMovement>() != null)
            isDesktop = true;
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
                if (hit.transform.GetComponent<SlotController>() != null)
                {
                    hit.transform.GetComponent<SlotController>().Hovering = true;
                }
            }
        }
    }

    public void InventoryPressed()
    {
        EventManager.OpenInventory(transform.root.gameObject);
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
                if(hit.transform.GetComponent<IInteractable>() != null)
                {
                    Debug.Log("CALLING INTERACT");
                    hit.transform.GetComponent<IInteractable>().Interact();
                }
            }
        }
    }

    public void InteractPressed(GameObject gameObject)
    {

    }
}
