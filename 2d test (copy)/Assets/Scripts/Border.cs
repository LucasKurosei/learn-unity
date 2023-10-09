using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;


public class Border : NetworkBehaviour
{
    public new Light2D light;
    public int numEdges;
    public float radius;
    public float speed;
    private EdgeCollider2D ec2d;
    private Animation anim;
    private AnimationCurve curve;
    //static NetworkVariableSettings animationNVS = new NetworkVariableSettings
    //{
    //    WritePermission = NetworkVariablePermission.ServerOnly,
    //    ReadPermission = NetworkVariablePermission.Everyone
    //};
    //private NetworkVariable<Vector3> toPosition = new NetworkVariable<Vector3>(animationNVS);
    //private NetworkVariable<float> scaleMul = new NetworkVariable<float>(animationNVS);
    //private NetworkVariable<float> dur = new NetworkVariable<float>(animationNVS);
    private float dur;
    

    // Use this for initialization
    void Start()
    {
        radius = transform.localScale.x / 2;
        ec2d = GetComponent<EdgeCollider2D>();
        ec2d.points = GenerateBorder(numEdges, .5f, transform.localPosition);
        anim = GetComponent<Animation>();
    }

    public override void NetworkStart()
    {
        if (IsServer)
        {
            StartCoroutine(ShrinkCR(10));
        }
        base.NetworkStart();
    }

    private void Update()
    {
        if (radius * 2 != transform.localScale.x)
        {
            radius = transform.localScale.x / 2;
            light.pointLightInnerRadius = radius * 22.5f / 25;
            light.pointLightOuterRadius = radius;
        }
    }

    IEnumerator ShrinkCR(float time)
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(time);

            float rot = Random.value * 360 - 180;
            Vector3 toPosition = transform.localPosition + (Random.value * radius * 5f / 6f + radius / 6f) * new Vector3(Mathf.Cos(rot), Mathf.Sin(rot));
            float scaleMul = Random.value / 4f + 1f / 2f;

            if (!NetworkManager.Singleton.IsHost)
            {
                Shrink(toPosition, scaleMul);
            }

            ShrinkClientRpc(toPosition, scaleMul);

            yield return new WaitForSeconds(dur);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject GO = collision.gameObject;
        Stats stats = GO.GetComponent<Stats>();
        if (stats.type == "Player")
        {
            stats.isInsideZone = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Transform GO = collision.transform;
        Stats stats = GO.GetComponent<Stats>();
        if (stats.type == "Player" && (GO.position - transform.position).magnitude <= transform.localScale.x * radius)
        {

            stats.isInsideZone = true;
        }
    }

    private Vector2[] GenerateBorder(int numEdges, float radius, Vector3 center)
    {
        Vector2[] points = new Vector2[numEdges + 1];

        for (int i = 0; i < numEdges; i++)
        {
            float angle = 2 * Mathf.PI * i / numEdges;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            points[i] = new Vector2(x, y);
        }
        points[numEdges] = points[0];

        return points;
    }



    private void Shrink(Vector3 toPosition, float scaleMul)
    {
        AnimationClip clip = new AnimationClip();
        clip.legacy = true;

        char[] c = { 'x', 'y', 'z' };
        float duration = (transform.position - toPosition).magnitude / speed;
        Keyframe[] keys;
        for (int i = 0; i < 3; i++)
        {
            keys = new Keyframe[2];
            keys[0] = new Keyframe(0.0f, transform.localPosition[i]);
            keys[1] = new Keyframe(duration, toPosition[i]);
            curve = new AnimationCurve(keys);
            clip.SetCurve("", typeof(Transform), "localPosition." + c[i], curve);
        }
        for (int i = 0; i < 3; i++)
        {
            keys = new Keyframe[2];
            keys[0] = new Keyframe(0.0f, transform.localScale[i]);
            keys[1] = new Keyframe(duration, transform.localScale[i] * scaleMul);
            curve = new AnimationCurve(keys);
            clip.SetCurve("", typeof(Transform), "localScale." + c[i], curve);
        }

        // now animate the GameObject
        anim.AddClip(clip, clip.name);
        anim.Play(clip.name);
        if (NetworkManager.Singleton.IsServer)
        {
            dur = clip.length;
        }
    }

    [ClientRpc]
    private void ShrinkClientRpc(Vector3 toPosition, float scaleMul)
    {
        Shrink(toPosition, scaleMul);
    }
}