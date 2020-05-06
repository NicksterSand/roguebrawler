using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
    #region Fields
    [SerializeField]
    Camera mainCam;
    [SerializeField]
    GameObject punchBoxL;
    [SerializeField]
    GameObject punchBoxR;
    [SerializeField]
    GameObject fistL;
    [SerializeField]
    GameObject fistR;
    [SerializeField]
    RectTransform healthBar;
    [SerializeField]
    TextMeshProUGUI healthText;
    [SerializeField]
    GameObject passiveItemsUI;
    [SerializeField]
    Image activeItemSprite;
    [SerializeField]
    GameObject gameOver;

    const float gravity = -0.08f;

    float vertVelocity = 0;
    float jumpHeight = 0.7f;
    float walkSpeed = 0.1f;
    float runSpeed = 0.2f;

    float maxHealth = 100f;
    float health = 100f;

    bool canPunchL = true;
    bool canPunchR = true;
    bool activeReady = true;
    bool grounded = true;
    bool blockedLeft = false;
    bool dead = false;

    public bool tooFarLeft { get; set; }

    List<int> passiveItems;
    int activeItem;
    #endregion

    private void Start() {
        //Initialize variables
        passiveItems = new List<int>();
        blockedLeft = false;
        UpdateUI();
    }

    void FixedUpdate() {
        #region Movement
        //Run when holding Shift
        Vector2 movement;
        if (Input.GetButton("Fire3"))
            movement = new Vector2(Input.GetAxis("Horizontal") * runSpeed, Input.GetAxis("Vertical") * runSpeed);
        else
            movement = new Vector2(Input.GetAxis("Horizontal") * walkSpeed, Input.GetAxis("Vertical") * walkSpeed);
        
        //Rotate in the direction of horizontal movement
        if(movement.x > 0) {
            transform.rotation = Quaternion.Euler(0,90,0);
        } else if(movement.x < 0) {
            transform.rotation = Quaternion.Euler(0,-90,0);
        }

        //Jump when space is pressed
        if (Input.GetButtonDown("Jump") && grounded) {
            vertVelocity = jumpHeight;
            grounded = false;
        }

        //Fall with gravity
        vertVelocity += gravity;
        if(vertVelocity < -20)
            vertVelocity = -20;

        //Find out where the next position of the player will be
        Vector3 newPos = new Vector3(transform.position.x + movement.x, transform.position.y + vertVelocity, transform.position.z + movement.y);

        //If on ground, allow for jumping and don't fall any further
        if (newPos.y < 0) {
            grounded = true;
            newPos.y = 0;
        }

        //Don't go off of the pavement
        if (newPos.z > 3.6f)
            newPos.z = 3.6f;
        else if (newPos.z < -4.3f)
            newPos.z = -4.3f;

        //Don't go too far left
        if((blockedLeft || tooFarLeft) && newPos.x < transform.position.x) {
            newPos.x = transform.position.x;
        }

        //Update camera position
        Vector3 camPos = mainCam.transform.position;
        if (camPos.x - newPos.x < -3) {
            mainCam.transform.position = new Vector3(newPos.x - 3, camPos.y, camPos.z);
        } else if (camPos.x - newPos.x > 5) {
            mainCam.transform.position = new Vector3(newPos.x + 5, camPos.y, camPos.z);
        }

        //Set adjusted position
        transform.position = newPos;
        #endregion

        #region Actions
        //Punch with mouse buttons
        if (Input.GetButtonDown("Fire1") && canPunchL) 
            StartCoroutine(PunchL());
        if (Input.GetButtonDown("Fire2") && canPunchR)
            StartCoroutine(PunchR());
        //Use active item with Q, if the player has one
        if (Input.GetButton("ActiveItem") && activeReady && activeItem != 0)
            StartCoroutine(ActiveItem());
        //Press space to restart on game over screen
        if (Input.GetButton("Jump") && dead) 
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Press Escape to quit
        if (Input.GetButton("Cancel"))
            Application.Quit();
        #endregion
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("enemyAttack")) {
            //Take damage when hit with enemy attack
            health -= other.GetComponent<Attack>().Damage;
            if (other.GetComponent<Attack>().IsProjectile) {
                GameObject.Destroy(other.gameObject);
            }
            UpdateUI();
        } else if (other.CompareTag("blockLeft")) {
            //Disallow going past the left boundry at the start
            blockedLeft = true;
        } else if (other.CompareTag("item")) {
            //Pickup item on contact
            other.GetComponent<Item>().Pickup(this);
        }
    }

    private void OnTriggerExit(Collider other) {
        //Allow to move left again after leaving leftward boundary
        if (other.CompareTag("blockLeft")) {
            blockedLeft = false;
        }
    }

    private IEnumerator PunchL() {
        //Extend fist
        canPunchL = false;
        punchBoxL.SetActive(true);
        fistL.transform.localPosition = new Vector3(fistL.transform.localPosition.x, fistL.transform.localPosition.y, 2.89f);

        yield return new WaitForSeconds(0.1f);

        //Retract fist
        fistL.transform.localPosition = new Vector3(fistL.transform.localPosition.x, fistL.transform.localPosition.y, 2.28f);
        punchBoxL.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        //Allow for punching again
        canPunchL = true;
    }

    private IEnumerator PunchR() {
        //Extend fist
        canPunchR = false;
        punchBoxR.SetActive(true);
        fistR.transform.localPosition = new Vector3(fistR.transform.localPosition.x, fistR.transform.localPosition.y, 2.89f);

        yield return new WaitForSeconds(0.1f);

        //Retract fist
        punchBoxR.SetActive(false);
        fistR.transform.localPosition = new Vector3(fistR.transform.localPosition.x, fistR.transform.localPosition.y, 2.28f);

        yield return new WaitForSeconds(0.1f);

        //Allow for punching again
        canPunchR = true;
    }

    //Use active item (item in top right corner)
    private IEnumerator ActiveItem() {
        activeReady = false;

        //Use effect of current item (there is currently only one active item)
        switch (activeItem) {
            case 3:
                Enemy[] enemies = FindObjectsOfType<Enemy>();
                foreach (Enemy enemy in enemies) {
                    enemy.GetHit(4);
                }
                break;
            default:
                Debug.Log("Error: Active Item #" + activeItem + " not Implemented");
                break;
        }

        //Wait one second before being able to use it again
        yield return new WaitForSeconds(1);
        activeReady = true;
    }

    private void UpdateUI() {
        //Update healthbar
        float healthPercent = health / maxHealth;
        if (health < 0)
            healthPercent = 0;

        healthBar.localScale = new Vector3(healthPercent, 1, 1);
        healthBar.anchoredPosition = new Vector3(healthPercent * 100, healthBar.anchoredPosition.y);

        healthText.text = health + "/" + maxHealth;

        //Die if health is below zero
        if(health <= 0) {
            gameOver.SetActive(true);
            dead = true;
        }
    }

    //Get effect of picked up passive item
    public void AddPassiveItem(int itemID, Sprite sprite) {
        //Add item to the UI
        passiveItems.Add(itemID);
        if (passiveItems.Count < 9) {
            GameObject itemUI = passiveItemsUI.transform.GetChild(passiveItems.Count - 1).gameObject;
            itemUI.SetActive(true);
            itemUI.GetComponent<Image>().sprite = sprite;
        }

        //Get the effect of the item
        switch (itemID) {
            case 0:
                //Fire fist
                fistL.GetComponent<ParticleSystem>().Play();
                fistR.GetComponent<ParticleSystem>().Play();
                punchBoxL.GetComponent<Attack>().Damage += 5;
                punchBoxR.GetComponent<Attack>().Damage += 5;
                break;
            case 1:
                //Health Up
                maxHealth += 20;
                health = maxHealth;
                UpdateUI();
                break;
            case 2:
                //Speed Up
                runSpeed *= 1.5f;
                walkSpeed *= 1.5f;
                break;
            default:
                //Item not implemented
                Debug.Log("Error: Passive Item #" + itemID + " not Implemented");
                break;
        }
    }

    //Pick up an active item
    public void SetActiveItem(int itemID, Sprite sprite) {
        activeItem = itemID;
        activeItemSprite.gameObject.SetActive(true);
        activeItemSprite.sprite = sprite;
    }
}