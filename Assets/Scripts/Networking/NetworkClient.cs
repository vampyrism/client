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

public class NetworkClient
{
    public static NetworkClient instance = new NetworkClient();
    public static int BufferSize = 1024;
    public int myId = 0;
    public string ip = "127.0.0.1";
    public int port = 9000;
    public UDP udpInstance;

    public static NetworkClient GetInstance() { return instance; }

    public void Init()
    {
        udpInstance = new UDP();
        udpInstance.Connect(0);
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
                if(this.connected == false)
                {
                    this.Handshake();
                }

                byte[] data = new byte[1024];
                data = this.socket.Receive(ref this.endPoint);
                Debug.Log(data);
                HandleRawPacket(data);
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
                if(retries >= 3) { break; }

                Debug.Log("Trying to connect...");
                this.socket.Send(d, d.Length);
                retries += 1;

                Thread.Sleep(1000);
            }

            Debug.LogWarning("Failed to connect");
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
            //HandleData(dataReceived);
            Console.WriteLine("Connection Accepted");

        }

        public void HandleRawPacket(byte[] data)
        {
            UDPPacket packet = new UDPPacket(data);
            List<Message> messages = packet.GetMessages();

            // TODO: Refactor out from here
            MessageVisitorGameStateUpdater v = new MessageVisitorGameStateUpdater();
            foreach (Message message in messages)
            {
                message.Accept(v);
            }
        }

        public void SendPacket(UDPPacket packet)
        {

        }
    }
}