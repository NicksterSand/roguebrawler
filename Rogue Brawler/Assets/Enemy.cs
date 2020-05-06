using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    GameObject healthBar;

    protected GameObject player;

    protected float maxHealth = 30f;
    protected float health = 30f;

    private bool canPunch = true;

    public abstract IEnumerator Attack();

    private void Start() {
        player = GameObject.FindWithTag("Player");
    }

    private void Update() {
        if (canPunch)
            StartCoroutine(Attack());
    }

    //Healthbar always faces forward, regardless of enemy rotation
    private void LateUpdate() {
        healthBar.transform.parent.rotation = Quaternion.Euler(0, 0, 0);
    }

    //Take damage when colliding with an attack from the player
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("playerAttack")) {
            GetHit(other.GetComponent<Attack>().Damage);
        }
    }

    //Make the healthbar show the enemy's current health
    private void UpdateHealth() {
        float healthPercent = (health / maxHealth);
        if (health <= 0)
            healthPercent = 0;
        else if (healthPercent > 1)
            healthPercent = 1;

        healthBar.transform.localScale = new Vector3(healthPercent * 0.95f, healthBar.transform.localScale.y, healthBar.transform.localScale.z);

        healthBar.transform.localPosition = new Vector3((1 - healthPercent) * -0.477f, healthBar.transform.localPosition.y, healthBar.transform.localPosition.z);

        if (health <= 0)
            Die();
    }

    //Take damage
    public virtual void GetHit(float damage) {
        health -= damage;
        UpdateHealth();
    }

    public virtual void Die() {
        GameObject.Destroy(gameObject);
    }
}