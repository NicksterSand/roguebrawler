using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveItem : MonoBehaviour, Item {
    int itemID { get; set; }
    SpriteRenderer render;

    //Load the sprite for this item
    public Sprite GetSprite() {
        return Resources.Load<Sprite>("Items/item" + itemID);
    }

    //Have player get item, then destroy the box
    public void Pickup(Player player) {
        player.SetActiveItem(itemID, GetSprite());
        GameObject.Destroy(gameObject);
    }

    //Setup the itembox
    public void Initialize(int ID) {
        itemID = ID;

        render = GetComponentInChildren<SpriteRenderer>();
        render.sprite = GetSprite();
    }
}
