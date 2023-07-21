using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class StatePanel : MonoBehaviour
{
    [SerializeField] RectTransform content;
    private ClientState state;
    private TMP_Text textPrefab;

    private void Awake()
    {
        textPrefab = Resources.Load<TMP_Text>("LogText");
    }

    private void Update()
    {
        if (state == PhotonNetwork.NetworkClientState)
            return;

        state =  PhotonNetwork.NetworkClientState;
        Debug.Log($"[Photon] {state}");
        TMP_Text text = Instantiate(textPrefab, content);
        text.text = string.Format($"[Photon] {0} : {1}", System.DateTime.Now.ToString("HH:mm:ss.ff"), state);
    }
}
