using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]

public class WorldButton : MonoBehaviour, IInteractable, ISlot
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

    public void Interact(GameObject device)
    {
        if(clicked != null)
            clicked.Invoke();
    }
}
