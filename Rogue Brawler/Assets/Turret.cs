using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Enemy {
    [SerializeField]
    GameObject firePoint;
    GameObject lazer;
    bool canFire;

    private void Start() {
        //Initialize variables
        maxHealth = 15;
        health = 15;
        player = GameObject.FindWithTag("Player");
        canFire = true;

        lazer = Resources.Load<GameObject>("Prefabs/Lazer");
    }
    void Update() {
        //Face towards player and attack
        Vector3 LookRot = Quaternion.LookRotation(player.transform.localPosition - transform.position, new Vector3(0, 1, 0)).eulerAngles + new Vector3(-90, 0, -90);
        transform.GetChild(1).transform.rotation = Quaternion.Euler(LookRot);

        if (canFire)
            StartCoroutine(Attack());
    }
    public override IEnumerator Attack() {
        //Make a lazer, then wait a random amount of time before firing again.
        canFire = false;

        GameObject newLazer = Instantiate(lazer);
        newLazer.transform.position = firePoint.transform.position;
        newLazer.transform.rotation = firePoint.transform.rotation;
        newLazer.GetComponent<Attack>().IsProjectile = true;
        newLazer.GetComponent<Attack>().Damage = 5;

        yield return new WaitForSeconds(Random.Range(3,5));

        canFire = true;
    }
}
