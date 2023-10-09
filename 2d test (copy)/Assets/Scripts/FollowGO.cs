using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGO : MonoBehaviour
{
    public Transform toFollow;
    public Vector3 Translate = new Vector3(0, 0, 0);

    void Update()
    {
        Vector3 toPostion = toFollow.position;
        toPostion = new Vector3(toPostion.x, toPostion.y, transform.position.z) + Translate;
        transform.position = toPostion;
    }
}
