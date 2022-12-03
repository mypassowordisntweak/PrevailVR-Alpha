using FishNet.Connection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager
{
    public static event Action<NetworkConnection> actionTemplate;

    public static void TemplateAction(NetworkConnection player)
    {
        actionTemplate?.Invoke(player);
    }
}
