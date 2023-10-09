using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public float bodyDamage = 50;
    [HideInInspector]
    public PlayerControls firer;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Stats colliderStats = collision.GetComponent<Stats>();
        if (colliderStats != null)
        {
            colliderStats.currentHealth -= bodyDamage * Time.deltaTime;
            if (colliderStats.currentHealth <= 0)
            {
                StartCoroutine(firer.IncreaseXP(colliderStats.XP));
                collision.GetComponent<Animator>().SetTrigger("isDead");
            }
        }
    }
}
