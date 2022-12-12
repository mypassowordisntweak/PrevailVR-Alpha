using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItemBag : WorldObject
{
    public override bool Grab(ItemSocket device)
    {
        if(device.HeldWorldObject == null && transform.parent == null)
        {
            transform.parent = device.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            Rigidbody rb = transform.GetComponent<Rigidbody>();
            rb.isKinematic = true;
        
            device.HeldWorldObject = gameObject.GetComponent<WorldObject>();

            return true;
        }

        return false;
    }

    public override bool Release(ItemSocket device)
    {
        if(device.HeldWorldObject == this)
        {
            transform.parent = null;

            Rigidbody rb = transform.GetComponent<Rigidbody>();
            rb.isKinematic = false;

            device.HeldWorldObject = null;

            return true;
        }

        return false;
    }

    public override bool Interact(GameObject device)
    {

        return false;
    }

    public override void OnDestroy()
    {
        GameObject tempObj = new GameObject();
        tempObj.transform.position = transform.position;
        tempObj.name = "PICKUP SOUND";
        tempObj.AddComponent<AudioSource>().clip = audioClip;
        tempObj.GetComponent<AudioSource>().Play();
        Destroy(tempObj, 2);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CmdInteract(GameObject device)
    {
        Interact(device);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CmdGrab(Transform deviceNetworkObject)
    {
        ItemSocket tempDevice = deviceNetworkObject.GetComponent<ItemSocket>();
        if (tempDevice != null)
        {
            Grab(tempDevice);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CmdRelease(GameObject device)
    {
        ItemSocket tempDevice = device.GetComponent<ItemSocket>();
        if (tempDevice != null)
            Release(tempDevice);
    }
}
