using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class Server : MonoBehaviour
{
    TcpListener listener;
    List<TcpClient> connectedClients;
    List<TcpClient> disConnectedClients;

    public bool IsOpened { get; private set; } = false;


    private void Update()
    {
        if (!IsOpened)
            return;

        foreach (TcpClient client in connectedClients)
        {
            if (!ClientConnectCheck(client))            // 접속 여부 확인
            {
                client?.Close();
                disConnectedClients.Add(client);
                continue;
            }

            NetworkStream stream = client.GetStream();
            
            if (stream.DataAvailable)
            {
                Debug.Log("채팅 확인");
                StreamReader reader = new StreamReader(stream);
                string chat = reader.ReadLine();
                SendAll(chat);
            }
        }

        foreach (TcpClient client in disConnectedClients)
        {
            connectedClients.Remove(client);
        }

        disConnectedClients.Clear();
    }

    private void OnDisable()
    {
        CloseServer();
    }

    public void OpenServer()
    {
        if (IsOpened)
            return;

        connectedClients = new List<TcpClient>();
        disConnectedClients = new List<TcpClient>();

        try
        {   // 서버 열기 성공
            Debug.Log("서버 열기 성공");
            listener = new TcpListener(IPAddress.Any, 7777);
            listener.Start();
            IsOpened = true;
            listener.BeginAcceptTcpClient(OnAcceptClient, listener);

            // 클라이언트 접속 대기 성공
        }
        catch (Exception e)
        {   // 서버 열기 실패
            Debug.Log("서버 열기 실패");
            Debug.Log(e.Message);
            CloseServer();
        }
    }

    public void CloseServer()
    {
        listener?.Stop();
        listener = null;
        IsOpened = false;
        Debug.Log("서버 닫음");
    }

    public bool ClientConnectCheck(TcpClient client)
    {
        try
        {
            if (client != null && client.Client != null && client.Connected)
            {
                if (client.Client.Poll(0, SelectMode.SelectRead))
                {
                    //if (client.Client.Poll(0, SelectMode.SelectRead))
                    return !(client.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                    //return true;
                }
                else
                {
                    //Debug.Log("클라이언트 무반응 확인2");
                    return true;
                }
            }
            else
            {
                Debug.Log("클라이언트 무반응 확인1");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.Log("클라이언트 접속 확인 실패");
            Debug.Log(e.Message);
            CloseServer();
            return false;
        }
    }

    public void SendAll(string chat)
    {
        Debug.Log($"전체 클라리언트에게 {chat} 전달");
        foreach (TcpClient client in connectedClients)
        {
            Stream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(chat);
            writer.Flush();
        }
    }

    private void OnAcceptClient(IAsyncResult ar)
    {
        Debug.Log("클라이언트 접속 시도를 감지");

        if (!IsOpened)
            return;

        if (listener == null)
            return;

        TcpClient client = listener.EndAcceptTcpClient(ar);
        connectedClients.Add(client);
        listener.BeginAcceptTcpClient(OnAcceptClient, listener);
    }
}
