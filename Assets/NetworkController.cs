using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public GameObject loadingMenu;

    // Start is called before the first frame update
    void Start() {
      PhotonNetwork.ConnectUsingSettings();
      Screen.SetResolution(1920, 1080, false);
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinOrCreateRoom("room", new RoomOptions { MaxPlayers = 100 }, null);

    public override void OnJoinedRoom()
    {
        loadingMenu.SetActive(false);
        PhotonNetwork.Instantiate("Player", new Vector3(0, 4, 0), Quaternion.identity);
    }
}
