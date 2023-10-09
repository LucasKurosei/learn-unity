using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class Map : NetworkBehaviour
{
    public GameObject tankPrefab;
    [HideInInspector]
    public int obsticleNum = 0;
    public int maxObsticleNum = 600;
    public GameObject obsticle1;
    public GameObject obsticle2;
    public Border border;
    public override void NetworkStart()
    {
        if (IsServer)
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClients.Keys)
            {
                Destroy(NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject);
                GameObject tank = Instantiate(tankPrefab);
                float rot = Random.value * 360 - 180;
                tank.transform.position = Random.value * 25 * new Vector3(Mathf.Cos(rot), Mathf.Sin(rot));
                tank.transform.eulerAngles = new Vector3(0, 0, Random.value * 360 - 180);
                tank.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            }
        }
        for (int i = 0; i < 80; i++)
        {
            SpawnObsticle(obsticle1);
        }
        for (int i = 0; i < 15; i++)
        {
            SpawnObsticle(obsticle2);
        }
        base.NetworkStart();
    }
    void FixedUpdate()
    {
        if (Random.value < .008 && obsticleNum < maxObsticleNum)
        {
            SpawnObsticle(obsticle1);
        }
        if (Random.value < .001 && obsticleNum < maxObsticleNum)
        {
            SpawnObsticle(obsticle2);
        }
    }

    private void SpawnObsticle(GameObject obsticle)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GameObject ob = Instantiate(obsticle, transform);
            float rot = Random.value * 360 - 180;
            ob.transform.position = Random.value * border.transform.localScale.x / 2 * new Vector3(Mathf.Cos(rot), Mathf.Sin(rot)) + border.transform.localPosition;
            ob.transform.eulerAngles = new Vector3(0, 0, Random.value * 360 - 180);
            ob.GetComponent<NetworkObject>().Spawn();
            obsticleNum++;
        }
    }
}
