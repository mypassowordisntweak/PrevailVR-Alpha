using FishNet.Serializing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CustomSerializers
{
    public static void WriteItemObject(this Writer writer, ItemObject itemObject)
    {
        writer.WriteByte((byte)itemObject.ItemType);
        writer.WriteInt32(itemObject.Amount);
    }
    
    public static ItemObject ReadItemObject(this Reader reader)
    {
        ItemType itemType = (ItemType)reader.ReadByte();
        List<Item> itemList = GamemodeTest.instance.ListOfItems.itemList;
        Item tempItem = GamemodeTest.instance.ListOfItems.itemList.Where(x => x.itemType == itemType).FirstOrDefault();
        ItemObject tempItemObject = new ItemObject(tempItem);

        tempItemObject.Amount = reader.ReadInt32();

        return tempItemObject;
    }
}
