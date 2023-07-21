using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;


// Network�� ������ ��û�ϰ� �ʹٸ� PhotonNetwork.~~~
// Network�� ������ �ް� �ʹٸ� MonoBehaviourPunCallbacks ��� �� �ش� ��Ȳ�� �´� �Լ� override

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public new UnityEvent OnConnected;
    public UnityEvent OnDisConnected;
    public UnityEvent OnJoinedRoomEvent;
    public UnityEvent OnJoinedRoomFailedEvent;
    public UnityEvent OnCreatedRoomFailedEvent;

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnected");
        OnConnected?.Invoke();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisConnected");
        OnDisConnected?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        OnJoinedRoomEvent?.Invoke();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        OnJoinedRoomFailedEvent?.Invoke();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        OnJoinedRoomFailedEvent?.Invoke();
    }
}
