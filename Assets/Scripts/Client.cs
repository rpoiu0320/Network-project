using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using System;

public class Client : MonoBehaviour
{
    // Socket
    private TcpClient client;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    public bool IsConnected { get; private set; } = false;

    private void OnDisable()
    {
        DisConnect();
    }

    private void Update()
    {
        if (IsConnected && stream.DataAvailable)    // NetworkStream.DataAvailable �����͸� ���� �� �ִ��� ����
            ReceiveChat();
    }

    public void Connect()
    {
        if (IsConnected)
            return;

        try
        {   // ���� ����
            client = new TcpClient("127.0.0.1", 7777);      // 127.0.0.1 == localhost, �ڰ�ȸ��
            stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            Debug.Log("���� ����");
            IsConnected = true;
        }
        catch (Exception e)
        {   // ���� ����
            Debug.Log(e.Message);
            DisConnect();
        }
    }

    public void DisConnect()
    {
        reader?.Close();
        reader = null;
        writer?.Close();
        writer = null;
        stream?.Close();
        stream = null;
        client?.Close();
        client = null;
        IsConnected = false;
    }

    public void SendChat(string chat)
    {
        if (!IsConnected)
        {
            Debug.Log("���ӵ� ������ �����ϴ�.");
            return;
        }

        // �⺻������ byte�迭�� �����͸� ������ ����
        try
        {   // �۽� ����
            writer.WriteLine(chat);                 // WriteLine�� StreamWriter._charBuffer�� ���ڿ��� �����ϴ� �Լ�
            writer.Flush();                         // Flush()�� StreamWriter._charBuffer���� BaseStream._buffer�� �Ű� ��� �Լ���.
                                                    // ���������� StreamWriter�� Close()�� �� Flush()�� ȣ��ȴ�.
                                                    //StreamWriter._charBuffer�� ���� ��Ʈ���� �ű�� �� �ӽõ����Ͷ� �� �� �ִ�.
                                                    //����ڰ� ���Ƿ� Flush()�� �����ϰڴٴ� �ǹ̴�
                                                    //Close()���� �ʰ� ���� �Էµ� ���۸� BaseStream�� �Űܴ�ڴٴ� ���̴�.
                                                    //Flush()�� ��Ȯ�� ������ _charPos�� 0���� ���½�Ű�� ���̴�.
                                                    //_charBuffer�� Ŭ���� ���� �ʴ´�.
                                                    //_charPos�� ������ �޸��� �� Ŀ���� ����.
                                                    //_charPos�� ����Ű�� ���� ��ġ�������� �����Ͱ� ä������ �����̴�.
        }
        catch (Exception e)
        {   // �۽� ����
            Debug.Log("ä�� ������ ����");
            Debug.Log(e.Message);
        }
    }

    public void ReceiveChat()
    {
        if (IsConnected)
            return;

        try
        {   // ���� ����
            string chat = reader.ReadLine();
            Chat.instance.AddMessage(chat);
        }
        catch (Exception e)
        {   // ���� ����
            Debug.Log(e.Message);
        }
    }
}
