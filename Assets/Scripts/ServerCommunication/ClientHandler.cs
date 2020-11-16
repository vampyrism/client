using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System;


public class ClientHandler : MonoBehaviour
{
    public static void TestPacketReceived(Packet packet)
    {
        string y = packet.ReadString();
        int x = packet.ReadInt();
        Client.instance.myId = x;
        ClientSend.TestPacketSend()
        //Client.instance.udp.Connect(((IPEndPoint)ClientHandler.instance))
    }
    

}
