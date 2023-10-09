using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Stats : MonoBehaviour
{
    public PlayerControls player;
    public UI_StatsCollider col;
    private List<LevelBar> levelBars = new List<LevelBar>();
    void Start()
    {
        foreach (Transform child in transform)
        {
            levelBars.Add(child.GetComponent<LevelBar>());
        }
    }
    public void Activate()
    {
        col.SetTrigger(true);
        col.GetComponent<EventTrigger>().enabled = false;
        foreach (LevelBar bar in levelBars)
        {
            bar.Activate();
        }
    }

    public void Deactivate()
    {
        col.GetComponent<EventTrigger>().enabled = true;
        foreach (LevelBar bar in levelBars)
        {
            bar.Deactivate();
        }
    }

    public void LevelUpHealth(float i)
    {
        player.stats.currentHealth += player.stats.currentHealth * i / player.stats.maxHealth;
        player.stats.maxHealth += i;
        player.points--;
        if (player.points == 0)
        {
            Deactivate();
        }
    }

    public void LevelUpRegen(float i)
    {
        player.stats.regen += i;
        player.points--;
        if (player.points == 0)
        {
            Deactivate();
        }
    }
    public void LevelUpBodyDamage(float i)
    {
        player.stats.bodyDamage += i;
        player.points--;
        if (player.points == 0)
        {
            Deactivate();
        }
    }

    public void LevelUpSight(float i)
    {
        player.SetSight(player.stats.sight + i);
        player.points--;
        if (player.points == 0)
        {
            Deactivate();
        }
    }
    public void LevelUpReload(float i = .75f)
    {
        player.stats.reload *= i;
        player.animator.SetFloat("reload", 1 / player.stats.reload);
        player.points--;
        if (player.points == 0)
        {
            Deactivate();
        }

    }
    public void LevelUpBulletDamage(float i)
    {
        player.stats.bulletDamage += i;
        player.points--;
        if (player.points == 0)
        {
            Deactivate();
        }
    }
    public void LevelUpSpeed(float i)
    {
        player.stats.speed += i;
        player.points--;
        if (player.points == 0)
        {
            Deactivate();
        }
    }
}
