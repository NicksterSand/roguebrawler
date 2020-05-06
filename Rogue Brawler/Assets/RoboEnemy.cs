using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboEnemy : Enemy {
    [SerializeField]
    GameObject fistL;
    [SerializeField]
    GameObject fistR;
    [SerializeField]
    GameObject punchBoxL;
    [SerializeField]
    GameObject punchBoxR;

    bool canAttack;

    private void Start() {
        //Initialize variables
        maxHealth = 30;
        health = 30;
        player = GameObject.FindWithTag("Player");

        punchBoxL.GetComponent<Attack>().Damage = 2f;
        punchBoxR.GetComponent<Attack>().Damage = 2f;

        canAttack = true;
    }

    public override IEnumerator Attack() {
        //Put fists out
        canAttack = false;
        punchBoxL.SetActive(true);
        fistL.transform.localPosition = new Vector3(fistL.transform.localPosition.x, fistL.transform.localPosition.y, 2.89f);
        punchBoxR.SetActive(true);
        fistR.transform.localPosition = new Vector3(fistR.transform.localPosition.x, fistR.transform.localPosition.y, 2.89f);

        yield return new WaitForSeconds(0.1f);

        //Return fists
        fistL.transform.localPosition = new Vector3(fistL.transform.localPosition.x, fistL.transform.localPosition.y, 2.28f);
        punchBoxL.SetActive(false);
        fistR.transform.localPosition = new Vector3(fistR.transform.localPosition.x, fistR.transform.localPosition.y, 2.28f);
        punchBoxR.SetActive(false);

        yield return new WaitForSeconds(.3f);

        //Allow for another attack
        canAttack = true;
    }

    private void Update() {
        Vector3 posDiff = transform.position - player.transform.position;
        float dist = Vector3.Distance(transform.position, player.transform.position);

        Vector3 newPos = transform.position;

        //Rotate to face the player in the x direction
        if(posDiff.x >= 0) {
            transform.localRotation = Quaternion.Euler(0, -90, 0);
        } else {
            transform.localRotation = Quaternion.Euler(0, 90, 0);
        }

        //Move towards the player in the x direction
        if (posDiff.x > 1.3f && posDiff.x < 10) {
            newPos.x -= 0.08f;
        }else if (posDiff.x < -1.3f) {
            newPos.x += 0.08f;
        }

        //Move towards the player in the z direction
        if (posDiff.z > .5f && posDiff.x < 10) {
            newPos.z -= .05f;
        }else if(posDiff.z < -.5f && posDiff.x < 10) {
            newPos.z += .05f;
        }

        //Attack when close to the player
        if(dist < 2 && canAttack) {
            StartCoroutine(Attack());
        }

        transform.position = newPos;
    }
}
