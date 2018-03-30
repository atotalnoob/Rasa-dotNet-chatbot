using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Runtime.InteropServices;

public class MongoDbContext

{


    //when class is created, get database instance for mongo
    public MongoDbContext([Optional] string mongoConnectionString)
    {

        //test connection string being provided
        if (string.IsNullOrEmpty(mongoConnectionString))
        {
            //declare default connection
            mongoConnectionString = "mongodb://mongoServer:27017";
        }

        //attempt to get database
        try
        {
            var _client = new MongoClient(mongoConnectionString);
            CurrentMongoDatabase = _client.GetDatabase("CustomLogs");
        }
        catch (Exception e)
        {
            throw e;
        }


    }

    public long DropCollection(string collectionName)
    {

        var count = CurrentMongoDatabase.GetCollection<BsonDocument>(collectionName).Count(_ => true);
        CurrentMongoDatabase.DropCollection(collectionName);
        return count;
    }


    public void AddDocumentToCollection(string collectionName, BsonDocument NewBsonDocument)
    {
        try
        {
            //get collection reference from database
            var collection = CurrentMongoDatabase.GetCollection<BsonDocument>(collectionName);
            //add document to collection
            collection.InsertOne(NewBsonDocument); 
        }
        catch (Exception e)
        {
            //handle
            throw e;
        }


    }

    //public bool IsValidAPIKey(string apiKey)
    //{

    //    try
    //    {
    //     var result = CurrentMongoDatabase.GetCollection<LogAPI.Models.APIKeyInfo>("keys").Find(_ => true).ToList();

    //        var apiFound = (from itm in result
    //                        where itm.keyValue == apiKey
    //                        where itm.keyEnabled == true
    //                        select itm).FirstOrDefault<LogAPI.Models.APIKeyInfo>();

           
    //        if (apiFound != null)
    //        {
    //            return true;

    //        }
    //        else
    //        {
    //            return false;
    //        }



    //    }
    //    catch (Exception)
    //    {
    //        return false;
    //    }


      

    //}

    //public bool IsAdminAPIKey(string apiKey)
    //{

    //    try
    //    {
    //        var result = CurrentMongoDatabase.GetCollection<LogAPI.Models.APIKeyInfo>("keys").Find(_ => true).ToList();

    //        var apiFound = (from itm in result
    //                        where itm.keyValue == apiKey
    //                        where itm.keyEnabled == true
    //                        select itm).FirstOrDefault<LogAPI.Models.APIKeyInfo>();


    //        if (apiFound != null)
    //        {
    //            return apiFound.isAdministrator;

    //        }
    //        else
    //        {
    //            return false;
    //        }
        
    //    }
    //    catch (Exception)
    //    {
    //        return false;
    //    }




    //}


    //hold database instance

    public IMongoDatabase CurrentMongoDatabase { get; set; }


}
