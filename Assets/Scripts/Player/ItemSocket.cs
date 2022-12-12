using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSocket : NetworkBehaviour
{
    public WorldObject HeldWorldObject { get => heldWorldObject; set => heldWorldObject = value; }
    public ItemObject HeldItem { get => heldWorldObject != null ? heldWorldObject.heldItem : null; }

    private WorldObject heldWorldObject;

    public void Start()
    {
        heldWorldObject = null;
    }
}
