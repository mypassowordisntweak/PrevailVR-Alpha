using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabbable
{
    public bool Grab(ItemSocket device);
    public bool Release(ItemSocket device);
}
