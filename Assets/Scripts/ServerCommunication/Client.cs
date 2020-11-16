using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel.Design;
using System.Threading;
using System.IO;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int BufferSize = 1024;
    public int myId = 0;
    public string ip = "127.0.0.1";
    public int port = 4500;
    public UDP udpInstance;
    private static Dictionary<int, PacketHandler> packetHandlers;
    private delegate void PacketHandler(Packet packet);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }


    public void ServerConnect()
    {
        udpInstance = new UDP();
        InitializePacketHandlers();
    }


    private void InitializePacketHandlers()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandler.TestPacketReceived }
        };
    }

    public void SendDataUDP(Packet packet)
    {
        packet.WriteLength();
        Client.instance.udpInstance.SendPacket(packet);
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }
        
        public void Connect(int clientPort)
        {
            socket = new UdpClient(clientPort);
            socket.Connect(endPoint);
            socket.BeginReceive(CallbackUponReceive, null);
            using (Packet packet = new Packet())
            {
                SendPacket(packet);
            }

        }

        public void CallbackUponReceive(IAsyncResult result)
        {
            byte[] dataReceived = socket.EndReceive(result, ref endPoint);
            socket.BeginReceive(CallbackUponReceive, null);
            //Size of full package?
            if(dataReceived.Length < 8){
                return;
            }

            //Handle data
            HandleData(dataReceived);
            Console.WriteLine("Connection Accepted");

        }

        public void HandleData(byte[] data)
        {
            using (Packet packet = new Packet(data))
            {
                int length = packet.ReadInt();
                data = packet.ReadBytes(length);
            }

            using (Packet packet = new Packet(data))
            {
                int packetID = packet.ReadInt();
                packetHandlers[packetID](packet);
            }
        }

        public void SendPacket(Packet packet)
        {
            packet.WriteIntHead(instance.myId);
            if(socket != null)
            {
                socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
            }
        }
    }




}