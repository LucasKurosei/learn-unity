using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Messaging;

public class Stats : NetworkBehaviour
{
    public string type;
    public float sight = 10;
    public float maxHealth = 20;
    public float currentHealth;
    public float regen = 1;
    public float speed;
    public float bulletDamage;
    public float bulletSpeed;
    public float recoil;
    public float XP;
    public float currentXP;
    public float bodyDamage;
    public float reload;
    [HideInInspector]
    public bool isInsideZone = true;

    public GameObject healthBarPrefab;
    [HideInInspector]
    public GameObject healthBar;

    void Start()
    {
        currentXP = XP;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (currentHealth <= 0 || currentHealth >= maxHealth)
        {
            if (healthBar != null)
            {
                healthBar.GetComponent<Animator>().SetBool("isVisible", false);
                Destroy(healthBar, .5f);
            }
            if (currentHealth <= 0)
                GetComponent<Animator>().SetBool("isDead", true);
        }
        else if (currentHealth < maxHealth && type == "Player")
        {
            if (healthBar == null)
            {
                healthBar = Instantiate(healthBarPrefab);
                healthBar.GetComponent<FollowGO>().toFollow = transform;
                healthBar.GetComponent<Animator>().SetBool("isVisible", true);
            }
            healthBar.transform.GetChild(0).GetComponent<Slider>().value = GetComponent<Stats>().currentHealth / GetComponent<Stats>().maxHealth;
            if (isInsideZone)
            {
                currentHealth += regen * Time.deltaTime;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void die2ServerRpc()
    {
        Destroy(gameObject);
    }

    public void die1()
    {
        Destroy(gameObject.GetComponent<Collider2D>());
    }
}
