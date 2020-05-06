using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Item {
    Sprite GetSprite();
    void Pickup(Player player);
    void Initialize(int ID);
}
