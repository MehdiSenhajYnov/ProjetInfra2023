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
        static Game gameServer;

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

        }

        static async void serverloop() {

            while (true)
            {
                gameServer = new Game(matchMakingClient);
                MatchMakingClient.game = gameServer;
                Console.WriteLine("Starting New Game Server");
                await gameServer.GameBegin();
            }
        }

        static async void MatchMakingLoop() {
            matchMakingClient.SetupClient();

        }
    }




}
