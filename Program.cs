using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TRSPO_HW_4_5
{
    internal class Program
    {
        static void Main()
        {
            // Створюємо потік для сервера
            Thread serverThread = new Thread(ServerMain);
            serverThread.Start();

            // Даємо серверу час на запуск
            Thread.Sleep(1000);

            // Створюємо потік для клієнта
            Thread clientThread = new Thread(ClientMain);
            clientThread.Start();

            // Чекаємо завершення обох потоків
            serverThread.Join();
            clientThread.Join();

            Console.ReadLine();
        }

        static void ServerMain()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 12345;

            TcpListener listener = new TcpListener(ipAddress, port);
            listener.Start();
            Console.WriteLine("Server started. Waiting for connection...");

            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected.");

            NetworkStream stream = client.GetStream();

            byte[] receiveBuffer = new byte[4];
            int bytesRead;

            while (true)
            {
                bytesRead = stream.Read(receiveBuffer, 0, 4);
                if (bytesRead == 0)
                    break;

                int messageSize = BitConverter.ToInt32(receiveBuffer, 0);

                byte[] messageBuffer = new byte[messageSize];
                bytesRead = stream.Read(messageBuffer, 0, messageSize);

                if (bytesRead == 0)
                    break;

                string message = Encoding.UTF8.GetString(messageBuffer);

                Console.WriteLine("Client: " + message);

                // Відправляємо відповідь клієнту
                string response = "Server: Message recieved.";
                byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                int responseSize = responseBuffer.Length;

                stream.Write(BitConverter.GetBytes(responseSize), 0, 4);
                stream.Write(responseBuffer, 0, responseSize);
            }
        }

        static void ClientMain()
        {
            IPAddress serverIP = IPAddress.Parse("127.0.0.1");
            int serverPort = 12345;

            TcpClient client = new TcpClient();
            client.Connect(serverIP, serverPort);
            NetworkStream stream = client.GetStream();

            for (int i = 0; i < 100; i++)
            {
                string message = "Message " + i;

                byte[] messageBuffer = Encoding.UTF8.GetBytes(message);
                int messageSize = messageBuffer.Length;

                stream.Write(BitConverter.GetBytes(messageSize), 0, 4);
                stream.Write(messageBuffer, 0, messageSize);

                byte[] responseSizeBuffer = new byte[4];
                int bytesRead = stream.Read(responseSizeBuffer, 0, 4);
                if (bytesRead == 0)
                    break;

                int responseSize = BitConverter.ToInt32(responseSizeBuffer, 0);

                byte[] responseBuffer = new byte[responseSize];
                bytesRead = stream.Read(responseBuffer, 0, responseSize);

                string response = Encoding.UTF8.GetString(responseBuffer);

                Console.WriteLine("Server: " + response);
            }

            client.Close();
        }
    }
}
