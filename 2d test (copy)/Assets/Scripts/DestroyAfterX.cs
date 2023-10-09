using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterX : MonoBehaviour
{
    public float X;

    void Update()
    {
        X -= Time.deltaTime;
        if (X<0)
        {
            GetComponent<Animator>().SetTrigger("isDead");
        }

    }

    public void die()
    {
        Destroy(gameObject);
    }
}
