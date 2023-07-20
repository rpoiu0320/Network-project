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
        if (IsConnected && stream.DataAvailable)    // NetworkStream.DataAvailable 데이터를 읽을 수 있는지 여부
            ReceiveChat();
    }

    public void Connect()
    {
        if (IsConnected)
            return;

        try
        {   // 접속 성공
            client = new TcpClient("127.0.0.1", 7777);      // 127.0.0.1 == localhost, 자가회신
            stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            Debug.Log("접속 성공");
            IsConnected = true;
        }
        catch (Exception e)
        {   // 접속 실패
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
            Debug.Log("접속된 서버가 없습니다.");
            return;
        }

        // 기본적으로 byte배열로 데이터를 보내고 받음
        try
        {   // 송신 성공
            writer.WriteLine(chat);                 // WriteLine은 StreamWriter._charBuffer에 문자열을 저장하는 함수
            writer.Flush();                         // Flush()는 StreamWriter._charBuffer값을 BaseStream._buffer에 옮겨 담는 함수다.
                                                    // 내부적으로 StreamWriter가 Close()될 때 Flush()가 호출된다.
                                                    //StreamWriter._charBuffer는 실제 스트림에 옮기기 전 임시데이터라 볼 수 있다.
                                                    //사용자가 임의로 Flush()를 실행하겠다는 의미는
                                                    //Close()하지 않고 현재 입력된 버퍼를 BaseStream에 옮겨담겠다는 뜻이다.
                                                    //Flush()의 정확한 동작은 _charPos를 0으로 리셋시키는 것이다.
                                                    //_charBuffer를 클리어 하진 않는다.
                                                    //_charPos의 역할은 메모장 내 커서와 같다.
                                                    //_charPos가 가리키는 버퍼 위치에서부터 데이터가 채워지기 때문이다.
        }
        catch (Exception e)
        {   // 송신 실패
            Debug.Log("채팅 보내기 실패");
            Debug.Log(e.Message);
        }
    }

    public void ReceiveChat()
    {
        if (IsConnected)
            return;

        try
        {   // 수신 성공
            string chat = reader.ReadLine();
            Chat.instance.AddMessage(chat);
        }
        catch (Exception e)
        {   // 수신 실패
            Debug.Log(e.Message);
        }
    }
}
