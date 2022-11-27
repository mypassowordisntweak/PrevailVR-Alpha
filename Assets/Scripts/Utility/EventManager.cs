using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager
{
    public static event Action<GameObject> openInventory;

    public static void OpenInventory(GameObject player)
    {
        openInventory?.Invoke(player);
    }
}
