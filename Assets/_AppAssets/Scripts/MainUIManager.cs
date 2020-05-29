using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class MainUIManager : MonoBehaviour
{
    #region MonoBehaviorMethods
    private void Awake()
    {
        mPhotonManager = GetComponent<PhotonManager>();
    }
    #endregion

    #region MainMenu
    [Header("Main Menu")]
    [SerializeField] private TMP_InputField usernameIN;

    private string profileUserName;

    private void VerifyUsername()
    {
        if (string.IsNullOrEmpty(usernameIN.text))
        {
            profileUserName = "RANDOM_USER_" + Random.Range(100, 1000);
        }
        else
        {
            profileUserName = usernameIN.text;
        }
    }

    public void ExitTheGame()
    {
#if !UNITY_EDITOR
        Application.Quit();
#else
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    #endregion

    #region RoomListMenu
    [Header("RoomList Menu")]
    [SerializeField] private GameObject roomBtnPrefab;
    [SerializeField] private ScrollRect roomListScrollView;
    [SerializeField] private Button joinBtn;

    private Transform selectedRoom = null;
    private RoomInfo selectedRoomInfo = null;

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        // Disable Join Room Btn
        joinBtn.interactable = false;

        // Add Rooms
        foreach (RoomInfo room in roomList)
        {
            int index = CheckFound(room.Name);
            if (index != -1) // For update
            {
                if (room.RemovedFromList == true || room.IsVisible == false || room.IsOpen == false) // if room destroied
                {
                    Destroy(roomListScrollView.content.GetChild(index).gameObject);
                }
                else // For update Room
                {
                    Transform roomTransform = roomListScrollView.content.GetChild(index);
                    roomTransform.name = room.Name;

                    // Update room Data
                    PutDataOnRoomTransform(roomTransform, room);
                }
            }
            else // For New
            {
                GameObject go = Instantiate(roomBtnPrefab, roomListScrollView.content);
                go.name = room.Name;

                // Put Room Data
                PutDataOnRoomTransform(go.transform, room);

                // Add Listener
                go.GetComponent<Button>().onClick.AddListener(() => SelectToJoin(go.transform, room));
            }
        }
    }

    private void PutDataOnRoomTransform(Transform roomTransform, RoomInfo room)
    {
        // Name
        roomTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;

        // Map
        if (room.CustomProperties.ContainsKey(ImportantThings.MapKey))
        {
            roomTransform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = mPhotonManager.GetMapName((int)room.CustomProperties[ImportantThings.MapKey]);
        }
        else
        {
            roomTransform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "-----";
        }

        // Player Count
        roomTransform.GetChild(2).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + " / " + room.MaxPlayers;
    }

    private void SelectToJoin(Transform roomTransform, RoomInfo room)
    {
        ClearSeLector();
        roomTransform.GetChild(roomTransform.childCount - 1).gameObject.SetActive(true);
        selectedRoomInfo = room;
        joinBtn.interactable = true;
    }

    public void JoinSelectedRoom()
    {
        if (selectedRoomInfo != null)
        {
            VerifyUsername();

            RoomInfo room = selectedRoomInfo;

            mPhotonManager.JoinRoom(room);
        }
    }

    public void ClearSeLector()
    {
        foreach (Transform i in roomListScrollView.content)
        {
            i.GetChild(i.childCount - 1).gameObject.SetActive(false);
        }
    }

    public void ClearRoomSelected()
    {
        selectedRoomInfo = null;
    }

    private void RemoveAllRooms()
    {
        foreach (Transform i in roomListScrollView.content)
        {
            Destroy(i.gameObject);
        }
    }

    private int CheckFound(string roomName)
    {
        foreach (Transform i in roomListScrollView.content)
        {
            if (roomName.Equals(i.name))
            {
                return i.GetSiblingIndex();
            }
        }
        return -1;
    }
    #endregion

    #region CreateMenu
    [Header("Create Menu")]
    [SerializeField] private TMP_InputField roomNameIN;
    [SerializeField] private TextMeshProUGUI mapNameTxt;
    [SerializeField] private TMP_InputField roomMaxPlayerCount;

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameIN.text) && !string.IsNullOrWhiteSpace(roomNameIN.text)
            && !string.IsNullOrEmpty(roomMaxPlayerCount.text) && !string.IsNullOrWhiteSpace(roomMaxPlayerCount.text))
        {
            mPhotonManager.Create(roomNameIN.text, int.Parse(roomMaxPlayerCount.text));
        }
        else
        {
            // Open Notification
            print("errorOnCreate");
        }
    }

    public void ChangeMap()
    {
        mapNameTxt.text = "MAP: " + mPhotonManager.ChangeMap();
    }

    public void ResetAllCreateData()
    {
        roomNameIN.text = "";
        roomMaxPlayerCount.text = "8";
        mapNameTxt.text = mPhotonManager.ResetMapIndex();
    }
    #endregion

    #region LoadingBar
    [Header("Loading Bar")]
    [SerializeField] private Canvas loadingCanvas;
    [SerializeField] private Slider loadingSlider;

    public void ActivateLoadingMenu()
    {
        loadingCanvas.enabled = true;
    }

    public void SetLoadingProgress(float progress)
    {
        loadingSlider.value = progress;
    }
    #endregion

    #region Others
    private PhotonManager mPhotonManager;
    #endregion
}
