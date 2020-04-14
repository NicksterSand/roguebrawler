using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{
    [SerializeField]
    Camera mainCam;
    [SerializeField]
    GameObject punchBox;

    const float gravity = -0.1f;

    float vertVelocity = 0;
    float jumpHeight = 1;
    float walkSpeed = 0.1f;
    float runSpeed = 0.2f;

    bool canPunch = true;
    private void Start() {
        
    }
    void FixedUpdate() {
        #region Movement
        //Run when holding Shift
        Vector2 movement;
        if (Input.GetButton("Fire3"))
            movement = new Vector2(Input.GetAxis("Horizontal") * runSpeed, Input.GetAxis("Vertical") * runSpeed);
        else
            movement = new Vector2(Input.GetAxis("Horizontal") * walkSpeed, Input.GetAxis("Vertical") * walkSpeed);
        
        if(movement.x > 0) {
            transform.rotation = Quaternion.Euler(0,0,0);
        } else if(movement.x < 0) {
            transform.rotation = Quaternion.Euler(0,180,0);
        }

        if (Input.GetButtonDown("Jump"))
            vertVelocity = jumpHeight;

        vertVelocity += gravity;
        if(vertVelocity < -20)
            vertVelocity = -20;

        Vector3 newPos = new Vector3(transform.position.x + movement.x, transform.position.y + vertVelocity, transform.position.z + movement.y);
        if (newPos.y < 1)
            newPos.y = 1;

        Vector3 camPos = mainCam.transform.position;
        if (camPos.x - newPos.x < -4) {
            mainCam.transform.position = new Vector3(newPos.x - 4, camPos.y, camPos.z);
        }

        transform.position = newPos;
        #endregion
        #region Actions
        if (Input.GetButtonDown("Fire1") && canPunch) 
            StartCoroutine(Punch());
        #endregion
    }

    private IEnumerator Punch() {
        canPunch = false;
        punchBox.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        punchBox.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        canPunch = true;
    }
}