using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeManager : ItemCommon
{
    public BadgeData data;
    void Start() {
        this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = data.data[itemIndex].Image;
    }
    public override void GotIt(StatusSaver status) {
        status.badges.Add(new StatusSaver.BadgeStatus(itemIndex));
        Destroy(gameObject);
    }
}
