using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_InputField maxPlayerInputField;

    public void OnLogOutButtonClicked()
    {
        PhotonNetwork.Disconnect();
    }

    public void OnCreatRoomButtonClicked()
    {
        string roomName = roomNameInputField.text;

        if (roomName == "")
            roomName = $"Room {Random.Range(1000, 10000)}";

        string maxPlayerStr = maxPlayerInputField.text;
        int maxPlayer;

        if (maxPlayerStr == "")
        {
            maxPlayer = 8;
        }
        else
        {
            maxPlayer = int.Parse(maxPlayerInputField.text);
            maxPlayer = Mathf.Clamp(maxPlayer, 1, 8);
        }

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayer };
        PhotonNetwork.CreateRoom(roomName, options);
    }
}
