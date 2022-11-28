using FishNet.Connection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager
{
    public static event Action<NetworkConnection> openInventory;

    public static void OpenInventory(NetworkConnection player)
    {
        openInventory?.Invoke(player);
    }
}
