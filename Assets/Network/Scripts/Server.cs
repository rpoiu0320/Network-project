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
            if (!ClientConnectCheck(client))            // ���� ���� Ȯ��
            {
                client?.Close();
                disConnectedClients.Add(client);
                continue;
            }

            NetworkStream stream = client.GetStream();
            
            if (stream.DataAvailable)
            {
                Debug.Log("ä�� Ȯ��");
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
        {   // ���� ���� ����
            Debug.Log("���� ���� ����");
            listener = new TcpListener(IPAddress.Any, 7777);
            listener.Start();
            IsOpened = true;
            listener.BeginAcceptTcpClient(OnAcceptClient, listener);

            // Ŭ���̾�Ʈ ���� ��� ����
        }
        catch (Exception e)
        {   // ���� ���� ����
            Debug.Log("���� ���� ����");
            Debug.Log(e.Message);
            CloseServer();
        }
    }

    public void CloseServer()
    {
        listener?.Stop();
        listener = null;
        IsOpened = false;
        Debug.Log("���� ����");
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
                    //Debug.Log("Ŭ���̾�Ʈ ������ Ȯ��2");
                    return true;
                }
            }
            else
            {
                Debug.Log("Ŭ���̾�Ʈ ������ Ȯ��1");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Ŭ���̾�Ʈ ���� Ȯ�� ����");
            Debug.Log(e.Message);
            CloseServer();
            return false;
        }
    }

    public void SendAll(string chat)
    {
        Debug.Log($"��ü Ŭ�󸮾�Ʈ���� {chat} ����");
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
        Debug.Log("Ŭ���̾�Ʈ ���� �õ��� ����");

        if (!IsOpened)
            return;

        if (listener == null)
            return;

        TcpClient client = listener.EndAcceptTcpClient(ar);
        connectedClients.Add(client);
        listener.BeginAcceptTcpClient(OnAcceptClient, listener);
    }
}
