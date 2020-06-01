using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TeamVoiceSelect : MonoBehaviour
{
    Recorder rc;

    private static TeamVoiceSelect _instance;
    public static TeamVoiceSelect Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        rc = FindObjectOfType<Recorder>();
    }

    public void setVoiceTeam(byte team)
    {
        rc.InterestGroup = team;
        PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[0], new byte[1] { team });
        Debug.Log("aha");
    }
}
