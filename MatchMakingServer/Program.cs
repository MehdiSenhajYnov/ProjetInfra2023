using System;

namespace MatchMakingServer
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Clear();

/*
            Task.Run(async () => {
                Console.WriteLine("TEST ADD");
                string mystr = await DatabaseGestor.AddSave("TestOne","TestTwo",new byte[] {1,8,1,6,78,4,18},"");
                Console.WriteLine("MYSTR : " + mystr);
                });
*/
            MMServer mm = new MMServer();
            mm.SetupServer();


        }
    }
}
