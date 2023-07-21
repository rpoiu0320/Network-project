using Photon.Pun;
using TMPro;
using UnityEngine;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    private string playerNickName;

    private void OnEnable()
    {
        inputField.text = $"Player {Random.Range(1000, 10000)}";
    }

    public void OnLoginButtonClicked()
    {
        playerNickName = inputField.text;

        if (playerNickName.Trim() == "")
        {
            Debug.LogError("Invalid Player NickName");
            return;
        }

        PhotonNetwork.LocalPlayer.NickName = playerNickName;
        PhotonNetwork.ConnectUsingSettings();
    }

}
