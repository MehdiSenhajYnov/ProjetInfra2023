using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace MatchMakingServer
{
    public static class DatabaseGestor
    {
        public static MongoClient client = new MongoClient("mongodb://Mehdi:root@5.196.23.143:27017");
        public static IMongoDatabase database => client.GetDatabase("YnovInfraGame");
        public static void Init() {
            var collection = client.GetDatabase("YnovInfraGame").GetCollection<BsonDocument>("movies");
            var filter = Builders<BsonDocument>.Filter.Eq("title", "Back to the Future");
            var document = collection.Find(filter).First();
            Console.WriteLine(document);
        }



        public static async Task<SavesNameDocument?> GetAllSavesNameByUsername(string whatsearch, string username)
        {
            Console.WriteLine("Getting Save By Username ...");

            var collection = database.GetCollection<SavesNameDocument>("SaveName");

            var filter = Builders<SavesNameDocument>.Filter.Eq(whatsearch, username);
            var documents = await collection.Find(filter).ToListAsync();
            
            foreach (var item in documents)
            {
                return item;
            } 
            
            Console.WriteLine("Not save found");
            return null;
        }

        public static async Task<SavesNameDocument?> GetSaveNameByID(string saveID)
        {
            Console.WriteLine("Getting All Saves Name by SaveID ...");
            var collection = database.GetCollection<SavesNameDocument>("SaveName");

            var filter = Builders<SavesNameDocument>.Filter.Eq("SaveID", saveID);
            var documents = await collection.Find(filter).ToListAsync();
            
            foreach (var item in documents)
            {
                return item;
            }
            Console.WriteLine("no Save Found");
            return null;
        }

        public static async Task<List<SavesNameDocument>> GetAllSaves()
        {
            Console.WriteLine("Getting All Saves Name by SaveName ...");
            var collection = database.GetCollection<BsonDocument>("SaveName");

            var documents = await collection.Find(new BsonDocument()).ToListAsync();

            var savesNameDocument = new List<SavesNameDocument>();
            foreach (var document in documents)
            {
                var personne = BsonSerializer.Deserialize<SavesNameDocument>(document);
                savesNameDocument.Add(personne);
            }

            return savesNameDocument;
        }

        public static async Task<string> AddSave(string usernameOne, string usernameTwo, byte[] newSave, string oldSaveID, int SaveRound) 
        {
            Console.WriteLine("Adding Save ...");
            if (string.IsNullOrEmpty(oldSaveID))
            {
                string result = await NewSave(usernameOne, usernameTwo, newSave, SaveRound);
                Console.WriteLine("ADDSAVE End SaveID: " + result);
                return result;
            }
            Console.WriteLine("Checking if SaveID exist ...");
            var saves = await GetSaveNameByID(oldSaveID);
            if (saves == null) {
                string result = await NewSave(usernameOne, usernameTwo, newSave, SaveRound);
                Console.WriteLine("ADDSAVE End SaveID: " + result);
                return result;
            } else {
                UpdateSave(usernameOne, usernameTwo, newSave, oldSaveID, SaveRound);
            }
            Console.WriteLine("ADDSAVE End SaveID: " + oldSaveID);
            return oldSaveID;
        }

        public static async void UpdateSave(string usernameOne, string usernameTwo, byte[] newSave, string oldSaveName, int SaveRound)
        {
            Console.WriteLine("UpdateSave");
            // saveName unique
            var collection = database.GetCollection<SavesNameDocument>("SaveName");
            SavesNameDocument save = new SavesNameDocument(usernameOne, usernameTwo, newSave, SaveRound);


            var filter = Builders<SavesNameDocument>.Filter.Eq("SaveName", oldSaveName);
            var update = Builders<SavesNameDocument>.Update.Set("Save", newSave);

            var result = await collection.UpdateOneAsync(filter, update);
        }


        public static async Task<string> NewSave(string usernameOne, string usernameTwo, byte[] newSave, int SaveRound) 
        {
            // saveName unique
            var collection = database.GetCollection<SavesNameDocument>("SaveName");
            SavesNameDocument save = new SavesNameDocument(usernameOne, usernameTwo, newSave, SaveRound);

            await collection.InsertOneAsync(save);
            return save.SaveID;
        }
    }

    public class SavesNameDocument 
    {

        public ObjectId Id { get; set; }
        public string SaveID { get; set; }

        public string UsernamePlyrOne { get; set; }
        public string UsernamePlyrTwo { get; set; }
        public byte[] Save { get; set; }
        public int SaveRound { get; set; }
        

        public SavesNameDocument(string newUsernameOne, string newUsernameTwo, byte[] newSave, int newSaveRound) { 
            UsernamePlyrOne = newUsernameOne;
            UsernamePlyrTwo = newUsernameTwo;
            Save = newSave;
            SaveRound = newSaveRound;
            SaveID = Guid.NewGuid().ToString();
        }

        public SavesNameDocument()
        {
        }
    }


    
}