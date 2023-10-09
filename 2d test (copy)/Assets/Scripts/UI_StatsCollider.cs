using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_StatsCollider : MonoBehaviour
{
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void SetTrigger(bool opened)
    {
        animator.SetBool("Opened", opened);
    }
}
