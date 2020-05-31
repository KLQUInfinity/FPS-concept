using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

[RequireComponent(typeof(MainUIManager))]
public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<string> maps;

    private MainUIManager uiManager;
    private int mapIndex;

    private void Awake()
    {
        mapIndex = 0;
        uiManager = GetComponent<MainUIManager>();
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
    }

    public void Connect()
    {
        PhotonNetwork.GameVersion = "0.0.0";
        PhotonNetwork.ConnectUsingSettings();
    }

    //public void Join()
    //{
    //    PhotonNetwork.JoinRandomRoom();
    //}

    public void StartGame()
    {
        //VerifyUsername();

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            StartCoroutine(loadAsyncLevel(1));
        }
    }

    public void JoinRoom(RoomInfo room, string profileUserName)
    {
        //PhotonNetwork.LocalPlayer.SetCustomProperties()
        PhotonNetwork.NickName = profileUserName;
        PhotonNetwork.JoinRoom(room.Name);
    }

    IEnumerator loadAsyncLevel(int sceneNum)
    {
        uiManager.ActivateLoadingMenu();
        PhotonNetwork.LoadLevel(1);
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            uiManager.SetLoadingProgress(PhotonNetwork.LevelLoadingProgress);
            yield return new WaitForEndOfFrame();
        }
    }

    public void Create(string roomName, int maxPlayerCount, string profileUserName)
    {
        PhotonNetwork.NickName = profileUserName;

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)maxPlayerCount;

        options.CustomRoomPropertiesForLobby = new string[] { ImportantThings.MapKey };

        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties.Add(ImportantThings.MapKey, mapIndex);

        options.CustomRoomProperties = properties;

        PhotonNetwork.CreateRoom(roomName, options);
    }

    #region CallBacks
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        base.OnConnectedToMaster();
    }

    public override void OnJoinedRoom()
    {
        StartGame();

        base.OnJoinedRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {

        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {

        base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomList.Count > 0)
        {
            uiManager.UpdateRoomList(roomList);
        }
        base.OnRoomListUpdate(roomList);
    }
    #endregion

    #region Helper
    public string GetMapName(int index)
    {
        return maps[index];
    }

    public string ChangeMap()
    {
        mapIndex = (mapIndex + 1) % maps.Count;
        return maps[mapIndex];
    }

    public string ResetMapIndex()
    {
        mapIndex = 0;
        return maps[mapIndex];
    }
    #endregion
}
