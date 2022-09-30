using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : ItemCommon
{
    public ItemData data;
    void Start() {
        this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = data.data[itemIndex].Image;
    }
    public override void GotIt(StatusSaver status) {
        status.items.Add(itemIndex);
        Destroy(gameObject);
    }
}
