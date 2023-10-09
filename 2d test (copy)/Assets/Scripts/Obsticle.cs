using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Obsticle : MonoBehaviour
{
    private Stats stats;

    private void Start()
    {
        stats = GetComponent<Stats>();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Stats playerStats = collision.gameObject.GetComponent<Stats>();
        if (playerStats != null)
        {
            if (playerStats.type == "Player")
            {
                PlayerControls pc = collision.gameObject.GetComponent<PlayerControls>();
                playerStats.currentHealth -= stats.bodyDamage * Time.deltaTime;
                stats.currentHealth -= playerStats.bodyDamage * Time.deltaTime;

                if (stats.currentHealth <= 0)
                {
                    StartCoroutine(pc.IncreaseXP(stats.XP));
                }
            } else
            {
                Physics2D.IgnoreCollision(collision.collider, collision.otherCollider, true);
            }
        } 
    }

    public void die()
    {
        transform.parent.GetComponent<Map>().obsticleNum--;
    }
}
