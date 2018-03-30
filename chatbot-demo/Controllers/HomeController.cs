using System;
using System.Web.Mvc;
using chatbotHelper;


namespace chatbot_demo.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
           
            //GUID generated here
            var ConversationID = Guid.NewGuid();
            ViewBag.ConversationID = ConversationID;
            return View(); // pass into page view
        }

        //admin view
        //public ActionResult Admin()
        //{
        //    return View();
        //}
        //private void suggestionUtterance()
        //{
        //    string utteranceFile = Tools.GetLatestUtteranceFile();
        //}

        //    public string GetFile(string file)
        //{

        //    if (file == "utterances")
        //    {
        //        //create references
        //        var requiredFile = Tools.GetLatestUtteranceFile();

        //        //get data from file
        //        var utteranceData = System.IO.File.ReadAllText(requiredFile);

        //        //return file data
        //        return utteranceData;

        //    }

        //    else
        //    {
        //        throw new Exception("File Not Found!");
        //    }
        //}

        //[ValidateInput(false)]
        //public string SaveFile(string file, string jsonString)
        //{

        //    if (file == "utterances")
        //    {
        //        //create references
        //        var destFile = Tools.CreateUtteranceFile();

        //        //write file
        //        System.IO.File.WriteAllText(destFile, jsonString);

        //        return "File Saved Successfully!";
        //    }
        //    else
        //    {
        //        return ("Source File Not Defined!");
        //    }
        //}


        public string chat(string userInput, string conversationID)
        {
            //try
            //{
            //    MongoDbContext mongoDatabase = new MongoDbContext();
            //    var collection = new List<chatbotHelper.ChatAPIRequest>();
            //    collection = mongoDatabase.CurrentMongoDatabase.GetCollection<ChatAPIRequest>("conversations").Find(_ => true).ToList();
            //}
            //catch (Exception e)
            //{

            //    throw;
            //}

            var apiRequest = Tools.SendChatRequest(userInput, conversationID);


          

           // Tools.stateSaver = apiRequest.Response.nextState;

            //MongoDbContext mongoDatabase = new MongoDbContext();


            //        var collection = new List<Models.CustomLogModel>();
            //        collection = mongoDatabase.CurrentMongoDatabase.GetCollection<Models.CustomLogModel>("logs." + environmentType).Find(_ => true).ToList();


            ////update logs and apply server data from request 
            //        model.loggedOn = DateTime.Now;
            //        model.callerIPAddress = HttpContext.Current.Request.UserHostAddress;
            //        model.callerAgent = HttpContext.Current.Request.UserAgent;
            //        model.callerHostName = HttpContext.Current.Request.UserHostName;
            //        model.calledURL = HttpContext.Current.Request.Url.OriginalString;
            //        //create bson document 
            //        var bsonDocument = model.ToBsonDocument();
            //        //add document to database 
            //        mongoDatabase.AddDocumentToCollection("logs." + model.environmentType, bsonDocument);

            if (apiRequest.Error != null)
            {
                //return from HTTP or equivalent exception
                return apiRequest.Error;
            }
            else if (apiRequest.Response.Error != null)
            {
                //return from Intent suggestioning error
                return apiRequest.Response.Error;
            }
            else if (apiRequest.Response.nextState == Tools.State.startSurvey)
            {
                Tools.chatbotProject = "&project=second";
                Tools.utteranceFilePath = @"\rasa\utterances\utterances-survey.json";
                return apiRequest.Response.suggestionedUtterance;
            }
            else
            {
                //return parsed value
                return apiRequest.Response.suggestionedUtterance;
            }
            
        }

    }
}