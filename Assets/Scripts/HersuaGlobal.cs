using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HersuaGlobal
{
    public static GameObject CreateDroppedItem()
    {
        return Resources.Load("DroppedItem") as GameObject;
    }
}
