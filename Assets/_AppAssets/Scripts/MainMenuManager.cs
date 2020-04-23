using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject loadingMenu;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI loadingTxt;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
    }

    public void Connect()
    {
        PhotonNetwork.GameVersion = "0.0.0";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Join()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void Create()
    {
        PhotonNetwork.CreateRoom("");
    }

    public void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            StartCoroutine(loadAsyncLevel(1));
        }
    }

    IEnumerator loadAsyncLevel(int sceneNum)
    {
        loadingMenu.SetActive(true);
        PhotonNetwork.LoadLevel(1);
        int countOfDots = 0;
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            loadingSlider.value = PhotonNetwork.LevelLoadingProgress;
            loadingTxt.text = "Loading " + CreateDotForLoading((countOfDots + 1) % 4);
            yield return new WaitForEndOfFrame();
        }
    }

    private string CreateDotForLoading(int count)
    {
        string dots = "";
        for (int i = 0; i < count; i++)
        {
            dots += ".";
        }
        return dots;
    }

    #region CallBacks
    public override void OnConnectedToMaster()
    {
        Join();

        base.OnConnectedToMaster();
    }

    public override void OnJoinedRoom()
    {
        StartGame();

        base.OnJoinedRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Create();

        base.OnJoinRandomFailed(returnCode, message);
    }
    #endregion
}
