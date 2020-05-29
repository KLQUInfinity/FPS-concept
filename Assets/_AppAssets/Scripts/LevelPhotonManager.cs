using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LevelPhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        //Spawn();
    }

    public void Spawn()
    {
        int index = Random.Range(0, spawnPoints.Length);
        PhotonNetwork.Instantiate(playerPrefab, spawnPoints[index].position, spawnPoints[index].rotation);
    }
}
