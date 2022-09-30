using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListItem : ListCommon
{
    public ItemData data;

    // Update is called once per frame
    void Update()
    {
        if (status.items.Count > index) {
            int itemIndex = status.items[index];
            ItemData.Items itemData = data.data[itemIndex];
            ValueSetUp(itemData.Image, itemData.Name, itemData.Description);

            bool isEnabled = false;
            foreach (ItemData.Effects effect in data.data[itemIndex].effects) {
                if (
                    (
                        effect.parameter == ItemData.Parameter.HP || 
                        effect.parameter == ItemData.Parameter.FP || 
                        effect.parameter == ItemData.Parameter.Discard
                    ) && !isEnabled
                ) {
                    isEnabled = true;
                }
            }

            Color enableColor;
            if (isEnabled) {
                enableColor = Color.white;
            } else {
                enableColor = Color.gray;
            }
            this.GetComponent<Image>().color = enableColor;
        }
    }
}
