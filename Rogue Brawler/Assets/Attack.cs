using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public bool IsProjectile { get; set; }
    public float Damage { get; set; }

    int counter;

    private void Start() {
        //Set the default damage for any uninitialized attack
        if(Damage == 0) {
            Damage = 3;
        }
    }

    private void Update() {
        //Move forward if this is a projectile
        if (IsProjectile) {
            transform.position += transform.forward * 0.1f;
            counter++;
            if(counter > 800) {
                GameObject.Destroy(gameObject);
            }
        }
    }
}
