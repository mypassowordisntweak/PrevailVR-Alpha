using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]

public class WorldButton : NetworkBehaviour, IGrabbable, ISlot
{
    public bool Hovering { get => isHovering; set => isHovering = value; }

    [SerializeField] private UnityEvent clicked;
    [SerializeField] private Image background;

    private bool isHovering;
    private bool isHoveringPrev;

    private AudioSource buttonAudio;

    public void Start()
    {
        buttonAudio = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (isHoveringPrev != isHovering && isHovering)
            buttonAudio.Play();

        if (isHovering)
        {
            //Debug.Log("WE SELECTED");
            Color c = background.color;
            c.g = 0.6f;
            background.color = c;
        }
        else
        {
            Color c = background.color;
            c.g = 0.431f;
            background.color = c;
        }

        isHoveringPrev = isHovering;
        isHovering = false;
    }
    public bool Grab(ItemSocket device)
    {
        if (clicked != null)
            clicked.Invoke();
        return true;
    }

    public bool Release(ItemSocket device)
    {
        throw new System.NotImplementedException();
    }

    [ServerRpc(RequireOwnership = false)]
    public void CmdGrab(Transform deviceNetworkObject)
    {
        ItemSocket tempDevice = deviceNetworkObject.GetComponent<ItemSocket>();
        if(tempDevice != null)
            Grab(tempDevice);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CmdRelease(GameObject device)
    {
        ItemSocket tempDevice = device.GetComponent<ItemSocket>();
        if (tempDevice != null)
            Release(tempDevice);
    }
}
