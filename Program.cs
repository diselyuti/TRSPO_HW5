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
            // Потік для сервера
            Thread serverThread = new Thread(ServerMain);
            serverThread.Start();

            // Потік для клієнта
            Thread clientThread = new Thread(ClientMain);
            clientThread.Start();

            // Чекаємо, доки обидва потоки завершать роботу
            serverThread.Join();
            clientThread.Join();
        }

        static void ServerMain()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int serverPort = 12345;

            TcpListener listener = new TcpListener(ipAddress, serverPort);
            listener.Start();
            Console.WriteLine("Server started. Waiting for connection...");

            TcpClient client = listener.AcceptTcpClient();

            NetworkStream stream = client.GetStream();

            while (true)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Server received message from client: " + message);

                string response = "Your message is " + message;
                byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBuffer, 0, responseBuffer.Length);
            }
        }

        static void ClientMain()
        {
            IPAddress serverIP = IPAddress.Parse("127.0.0.1");
            int serverPort = 12345;

            TcpClient client = new TcpClient();
            client.Connect(serverIP, serverPort);

            NetworkStream stream = client.GetStream();

            while (true)
            {
                Console.Write("Enter message to send to server: ");
                string message = Console.ReadLine();

                byte[] buffer = Encoding.UTF8.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);

                byte[] responseBuffer = new byte[1024];
                int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
                string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
                Console.WriteLine("Server response: " + response);
            }
        }
    }
}
