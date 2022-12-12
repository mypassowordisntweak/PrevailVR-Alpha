using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : NetworkBehaviour, IGrabbable, IInteractable
{
    public ItemObject HeldItem { get => heldItem; set => heldItem = value; }

    public ItemObject heldItem;
    [SerializeField] protected  AudioClip audioClip;

    public virtual bool Grab(ItemSocket device) { return false; }
    public virtual bool Release(ItemSocket device) { return false; }
    public virtual bool Interact(GameObject device) { return false; }
    public virtual void OnDestroy() { }

}
