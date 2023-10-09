using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerControls : NetworkBehaviour
{
    public GameObject camPrefab;
    public GameObject canvasPrefab;
    public GameObject lanternPrefab;
    public GameObject bulletGO;

    private Camera cam;
    private LevelBar levelBar;
    private UI_Stats ui_stats;
    private GameObject lantern;

    private NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone,
        SendTickrate = 20
    });
    private NetworkVariable<Vector3> Rot = new NetworkVariable<Vector3>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone,
        SendTickrate = 60
    });

    private float latestPosUpdate;
    private Vector3 v;
    private Vector3 direction = new Vector3();
    private Coroutine interpolationCR;

    [SerializeField] private float neededXP;
    [HideInInspector] public int points = 0;
    private int level = 0;
    private Rigidbody2D player;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public Stats stats;

    public override void NetworkStart()
    {
        latestPosUpdate = Time.time;
        Position.OnValueChanged += InterpolatePosition;
        base.NetworkStart();
    }

    void InterpolatePosition(Vector3 prePos, Vector3 newPos)
    {
        if (!IsServer && !IsOwner)
        {
            if (interpolationCR != null)
            {
                StopCoroutine(interpolationCR);
            }
            interpolationCR = StartCoroutine(InterpolatePositionCR(prePos, newPos));
            latestPosUpdate = Time.time;
        }
    }

    IEnumerator InterpolatePositionCR(Vector3 to, Vector2 vel)
    {
        //player.velocity = new Vector2();
        Vector3 dir = (Vector2)(to - transform.position);
        float In = dir.magnitude / vel.magnitude;
        for (float remaining = In; remaining > 0; remaining -= Time.deltaTime / In)
        {
            if (dir.sqrMagnitude > .0025)
            {
                transform.position += dir * Mathf.Min(Time.deltaTime, In) / In;
                yield return null;
            }
        }
        print("finished");
        //player.AddForce((Vector2)(prePos - newPos) / (prePos - newPos)[2], ForceMode2D.Impulse);
    }

    void Start()
    {
        stats = GetComponent<Stats>();
        animator = GetComponent<Animator>();
        player = GetComponent<Rigidbody2D>();
        animator.SetFloat("reload", 1 / stats.reload);

        if (IsLocalPlayer)
        {
            GameObject camera = Instantiate(camPrefab);
            camera.GetComponent<FollowGO>().toFollow = transform;
            cam = camera.GetComponent<Camera>();

            GameObject canvas = Instantiate(canvasPrefab);
            ui_stats = canvas.transform.GetChild(0).GetChild(0).GetComponent<UI_Stats>();
            ui_stats.player = this;
            levelBar = canvas.transform.GetChild(1).GetComponent<LevelBar>();
            lantern = Instantiate(lanternPrefab, transform);
            SetSight(stats.sight);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (cam.transform.position.z < -2 && scroll > 0)
            {
                cam.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, scroll * 10), ForceMode.Impulse);
            }
            else if (cam.transform.position.z > -115 && scroll < 0)
            {
                cam.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, scroll * 10), ForceMode.Impulse);
            }
            direction[0] = Input.GetAxis("Horizontal");
            direction[1] = Input.GetAxis("Vertical");
            direction = direction.normalized;

            v = (Input.mousePosition - cam.WorldToScreenPoint(transform.position)).normalized;
            float rotation = Vector3.SignedAngle(new Vector3(1, 0, 0), v, new Vector3(0, 0, 1));
            transform.eulerAngles = new Vector3(0, 0, rotation);
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetBool("isShooting", true);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                animator.SetBool("isShooting", false);
            }
            Rot.Value = transform.eulerAngles;
        }
        transform.eulerAngles = Rot.Value;
        if (IsServer)
        {
            Vector3 Pos = transform.position;
            Position.Value = Pos;
        }
    }

    void FixedUpdate()
    {
        player.AddForce(direction * stats.speed, ForceMode2D.Impulse);
        if (IsOwner)
            UpdateDirectionServerRpc(direction);
    }

    [ClientRpc]
    void MoveClientRpc(Vector3 to, Vector2 vel)
    {
        if (interpolationCR != null)
        {
            StopCoroutine(interpolationCR);
        }
        interpolationCR = StartCoroutine(InterpolatePositionCR(to, vel));
    }

    [ServerRpc]
    void UpdateDirectionServerRpc(Vector3 dir)
    {
        if (dir != direction)
        {
            direction = dir;
        }
    }

    [ServerRpc]
    public void ShootServerRpc()
    {
        Vector3 muzzelPosition = transform.GetChild(0).GetChild(0).position;
        GameObject bullet = Instantiate(bulletGO);
        bullet.GetComponent<Transform>().position = muzzelPosition;
        bullet.GetComponent<Rigidbody2D>().AddForce(v * stats.bulletSpeed, ForceMode2D.Impulse);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
        bullet.GetComponent<Damage>().bodyDamage = stats.bulletDamage;
        bullet.GetComponent<Damage>().firer = this;
        bullet.GetComponent<NetworkObject>().Spawn();
        player.AddForce(-v * stats.recoil, ForceMode2D.Impulse);
    }

    public void SetSight(float sight)
    {
        stats.sight = sight;
        this.lantern.transform.localScale = new Vector3(sight, sight, 1);
        this.lantern.GetComponent<Light2D>().pointLightOuterRadius = sight / 2;
        this.lantern.GetComponent<Light2D>().pointLightInnerRadius = sight / 10;
    }

    public IEnumerator IncreaseXP(float i)
    {
        float passed = 1;
        while (passed > 0)
        {
            stats.currentXP += i * Time.deltaTime;
            stats.XP += i * Time.deltaTime;
            passed -= Time.deltaTime;
            if (stats.currentXP >= neededXP)
            {
                level++;
                levelBar.LevelUp(level);
                points++;
                if (points == 1)
                {
                    ui_stats.Activate();
                }
                stats.currentXP -= neededXP;
                neededXP *= 1.25f;
            }
            levelBar.SetValue(stats.currentXP / neededXP);
            yield return null;
        }
    }

}
