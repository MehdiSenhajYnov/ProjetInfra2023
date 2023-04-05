using System;
using GameNetServer;
using MatchMakingServer;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Serveur
{
    class Program
    {
        static MatchMakingClient matchMakingClient = new MatchMakingClient("127.0.0.1", 8888);

        static void Main(string[] args)
        {

            Console.Clear();


            Task.Run(() => MatchMakingLoop());

            while (MatchMakingClient.client == null || !MatchMakingClient.client.IsConnected)
            {
                Thread.Sleep(250);
            }

            Task.Run(() => serverloop());



            while (true)
            {
                
            }

            // Idea : Connect to mathcmaking server when server is disponible, disconnect when not
        }

        static async void serverloop() {

            while (true)
            {
                Console.WriteLine("Starting New Game Server");
                Game gameServer = new Game(matchMakingClient);
                await gameServer.GameBegin();
            }
        }

        static async void MatchMakingLoop() {
            matchMakingClient.SetupClient();

        }
    }




}
