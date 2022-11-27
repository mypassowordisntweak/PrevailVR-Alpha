using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldButton : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEvent clicked;

    public void Interact()
    {
        Debug.Log("INTERACTED");

        if(clicked != null)
            clicked.Invoke();
    }
}
