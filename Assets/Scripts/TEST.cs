using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : NetworkBehaviour
{
    [SerializeField] private Transform Body;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            CmdSetColor();
        }
    }

    [ServerRpc]
    private void CmdSetColor()
    {
        Body.GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0f, 0f);
        RPCSetColor();
    }

    [ObserversRpc]
    private void RPCSetColor()
    {
        Body.GetComponent<MeshRenderer>().material.color = new Color(0f, 1f, 0f, 0f);
    }
}
