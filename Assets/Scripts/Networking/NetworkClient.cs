using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel.Design;
using System.Threading;
using System.IO;
using Assets.Server;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System.Collections.Concurrent;

public class NetworkClient
{
    public static NetworkClient instance = new NetworkClient();
    public int myId = 0;
    public string ip = "127.0.0.1";
    public int port = 9000;
    public UDP udpInstance;

    #region sequence_numbers
    public UInt16 RemoteSeqNum { get; private set; }
    public UInt16 LocalSeqNum { get; private set; }
    #endregion

    // Send and Receive buffers
    #region send_receive_buffers
    private static readonly int BufferSize = 1024;
    public UInt32[] ReceiveSequenceBuffer { get; private set; } = new UInt32[BufferSize];
    public UInt32[] SendSequenceBuffer { get; private set; } = new UInt32[BufferSize];
    public UDPAckPacket[] ReceiveBuffer { get; private set; } = new UDPAckPacket[BufferSize];
    public UDPAckPacket[] SendBuffer { get; private set; } = new UDPAckPacket[BufferSize];
    #endregion

    public ConcurrentQueue<Message> MessageQueue { get; private set; } = new ConcurrentQueue<Message>();

    public static NetworkClient GetInstance() { return instance; }

    public void Init()
    {
        #region initialize_buffers
        this.RemoteSeqNum = 0;
        this.LocalSeqNum = 0;
        this.ReceiveSequenceBuffer = new UInt32[BufferSize];
        this.SendSequenceBuffer = new UInt32[BufferSize];
        this.ReceiveBuffer = new UDPAckPacket[BufferSize];
        this.SendBuffer = new UDPAckPacket[BufferSize];

        for (int i = 0; i < BufferSize; i++)
        {
            this.ReceiveSequenceBuffer[i] = 0xFFFFFFFF;
            this.SendSequenceBuffer[i] = 0xFFFFFFFF;
        }
        #endregion

        this.udpInstance = new UDP();
        this.udpInstance.Connect(0);
    }

    public void FixedUpdate()
    {
        UDPPacket p = BuildPacket();

        this.SendBuffer[this.LocalSeqNum % BufferSize] = new UDPAckPacket
        {
            Acked = false,
            SendTime = Time.realtimeSinceStartup, // Note: we care about diff between send-ACK time for RTT calc
            Packet = p
        };
        
        this.SendSequenceBuffer[this.LocalSeqNum % BufferSize] = this.LocalSeqNum;

        this.LocalSeqNum += 1;

        this.udpInstance.SendPacket(p);

    }

    private UDPPacket BuildPacket()
    {
        UDPPacket p = new UDPPacket(this.LocalSeqNum, this.RemoteSeqNum);
        int space = p.SizeLeft();

        while (this.MessageQueue.Count > 0)
        {
            this.MessageQueue.TryPeek(out Message n);
            if (n.Size() < space)
            {
                this.MessageQueue.TryDequeue(out Message m);
                p.AddMessage(m);
            } 
            else
            {
                break;
            }
        }

        return p;
    }

    private int WrapArray(int a)
    {
        if(a < 0)
        {
            return BufferSize + a;
        } 
        else
        {
            return a;
        }
    }

    public void AckIncomingPacket(UDPPacket packet)
    {
        int i = packet.SequenceNumber % BufferSize;

        if (packet.SequenceNumber > this.RemoteSeqNum)
        {
            this.RemoteSeqNum = packet.SequenceNumber;
        }

        this.ReceiveSequenceBuffer[i] = packet.SequenceNumber;
        this.ReceiveBuffer[i].Acked = true;
        this.ReceiveBuffer[i].Packet = packet;


        i = packet.AckNumber % BufferSize;
        if (this.SendSequenceBuffer[i] == packet.AckNumber)
        {
            this.SendBuffer[i].Acked = true;
        }

        for (UInt16 offset = 1; offset <= 32; offset++)
        {
            if (packet.AckArray[offset - 1])
            {
                if (this.SendSequenceBuffer[WrapArray((packet.AckNumber - offset) % BufferSize)] == (packet.AckNumber - offset))
                {
                    this.SendBuffer[WrapArray((packet.AckNumber - offset) % BufferSize)].Acked = true;
                }
            }
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;
        public bool connected = false;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }
        
        public void Connect(int clientPort)
        {
            socket = new UdpClient();
            socket.Connect(endPoint);

            Task.Run(() =>
            {
                try
                {
                    if (this.connected == false)
                    {
                        this.Handshake();
                    }

                    while (true)
                    {
                        // TODO: Should try catch this as well / instead
                        byte[] data = new byte[1024];
                        //Debug.Log("feed me data");
                        data = this.socket.Receive(ref this.endPoint);
                        //Debug.Log("*nom* " + data);
                        HandleRawPacket(data);
                    }
                }
                catch(Exception e)
                {
                    Debug.LogError(e);
                }
            });
        }

        /*
         * BLOCKING - Must be called in a thread
         */
        public void Handshake()
        {
            int retries = 0;
            String knock = "Knock, knock";
            byte[] d = ASCIIEncoding.ASCII.GetBytes(knock);

            Task handshake_listener = Task.Run(() =>
            {
                byte[] data = new byte[1024];
                data = this.socket.Receive(ref this.endPoint);

                if(data.SequenceEqual(ASCIIEncoding.ASCII.GetBytes("VAMPIRES!")))
                {
                    Debug.Log("Successfully connected");
                    this.connected = true;
                    return;
                }

                if (retries >= 3) { return; }
            });

            while (this.connected == false)
            {
                if(retries >= 3) 
                {
                    Debug.LogWarning("Failed to connect");
                    break; 
                }

                Debug.Log("Trying to connect...");
                this.socket.Send(d, d.Length);
                retries += 1;

                Thread.Sleep(1000);
            }
        }

        public void HandleRawPacket(byte[] data)
        {
            UDPPacket packet = new UDPPacket(data);

            #region ackpacket
            NetworkClient.GetInstance().AckIncomingPacket(packet);
            #endregion

            List<Message> messages = packet.GetMessages();
            packet.PrintHeader();
            // TODO: Refactor out from here
            MessageVisitorGameStateUpdater v = new MessageVisitorGameStateUpdater();
            foreach (Message message in messages)
            {
                message.Accept(v);
            }
        }

        public void SendPacket(UDPPacket packet)
        {
            byte[] p = packet.Serialize();
            this.socket.Send(p, p.Length);
        }
    }
}