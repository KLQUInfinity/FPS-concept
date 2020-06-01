using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class LevelPhotonManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    public static LevelPhotonManager Instance { private set; get; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [SerializeField] private string playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private GameObject player;
    private int myTeamIndex;

    private void Start()
    {
        player = Spawn();
    }

    public GameObject Spawn()
    {
        int index = Random.Range(0, spawnPoints.Length);
        return PhotonNetwork.Instantiate(playerPrefab, spawnPoints[index].position, spawnPoints[index].rotation);
    }

    public void InitPlayer(int teamIndex)
    {

        //player.GetPhotonView().RPC("SetPlayerName", RpcTarget.OthersBuffered, PhotonNetwork.NickName, teamIndex);
        //player.GetPhotonView().RPC("ActivatePlayer", RpcTarget.AllBuffered);
        player.GetPhotonView().RPC("SetTeamIndex", RpcTarget.AllBuffered, teamIndex);
        myTeamIndex = teamIndex;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { ImportantThings.TeamIndex, teamIndex } });
        player.GetComponent<PlayerController>().ActivatePlayer();

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag(ImportantThings.PlayerTag))
        {
            if (go != player)
            {
                int goTeamIndex = (int)go.GetPhotonView().Owner.CustomProperties[ImportantThings.TeamIndex];
                if (player.GetComponent<PlayerController>().GetTeamIndex() == goTeamIndex)
                {
                    go.GetComponent<PlayerController>().TogglePlayerName(true);
                }
                else
                {
                    go.GetComponent<PlayerController>().TogglePlayerName(false);
                }
            }
        }
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }
}
