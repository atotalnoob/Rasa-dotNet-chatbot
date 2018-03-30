using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using RestSharp;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace chatbotHelper
{
    public class Tools
    {
        public static string chatbotURL = "http://ServerName:5005/parse?q=";
        public static string chatbotProject = "&project=chatbotDefault";
        public static string utteranceFilePath = @"\rasa\utterances\utterances.json";
        public static State stateSaver = State.none;
        public static ChatAPIRequest SendChatRequest(string userMessage, string convoID)
        {

            ChatAPIRequest chatRequest = new ChatAPIRequest(userMessage);
            chatRequest.conversationID = convoID;
            chatRequest.DateTime = DateTime.Now;
            IRestResponse resp;
            try
            {
                //send to rasa NLU, parse and return
                string message = "\"" + userMessage + "\"";
                string url = chatbotURL + message + chatbotProject;
                IRestRequest request = new RestRequest(Method.GET);
                RestClient client = new RestClient(url);
                resp = client.Execute(request);
                if (resp.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    chatRequest.Error = "Error, NLU server responded with an incorrect status code: " + resp.StatusCode.ToString();
                    return chatRequest;
                }

            }
            catch (Exception e)
            {
                chatRequest.Error = "Error in GET request from NLU server: " + e.ToString();
                return chatRequest;
            }

            chatRequest.Response = new ChatAPIResponse(resp.Content.ToString());


            //TODO: Refactor
            //purpose: Save/load states 
            if (stateSaver != State.none)
            {
                chatRequest.Response.previousState = stateSaver;
            }

            if (chatRequest.Response.nextState != State.none)
            {
                stateSaver = chatRequest.Response.nextState;
            }
            //end refactor


            //stateCheck.check(ref chatRequest);

            // Survey.saveInModel(chatRequest);
            return chatRequest;
        }

        //for admin view
        //    public static string GetLatestUtteranceFile()
        //    {
        //        var directory = new System.IO.DirectoryInfo(GetUtteranceDir());
        //        var requiredFile = directory.GetFiles()
        //         .OrderByDescending(f => f.LastWriteTime)
        //         .First();
        //        return requiredFile.FullName;
        //    }
        //    public static string CreateUtteranceFile()
        //    {
        //        var directory = GetUtteranceDir();
        //        var fileName = "utterances-" + DateTime.Now.ToString("MMddyy.hhmmss") + ".json";
        //        return System.IO.Path.Combine(directory, fileName);

        //    }

        //    private static string GetUtteranceDir()
        //    {
        //        return AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + @"\rasa\utterances\";
        //    }
        //}

        #region ChatBot Request and Response

        public class ChatAPIRequest
        {
            [BsonId]
            public MongoDB.Bson.ObjectId _id { get; set; }
            public string conversationID { get; set; }
            public string UserRequest { get; set; }
            public DateTime DateTime { get; set; }
            public string UserName { get; set; }
            public string Error { get; set; }
            public ChatAPIResponse Response { get; set; }
            public ChatAPIRequest(string userRequest)
            {
                UserRequest = userRequest;
            }
        }
        public enum State
        {
            none, quit, skip, startSurvey, id, region, suggestionName, selectOrg, suggestionDescription, suggestionOwners, suggestionTime, suggestionSave, suggestionQA, suggestionDecom, suggestionFrequency, suggestionInteractions
        }
        public class ChatAPIResponse
        {

            public string Intent { get; set; }
            public List<string> Entities { get; set; }
            public string RawUtterance { get; set; }
            public string suggestionedUtterance { get; set; }
            public string Error { get; set; }
            public State nextState { get; set; }
            public State previousState { get; set; }
            public ChatAPIResponse(string responseContent)
            {
                if (Tools.stateSaver != State.none)
                {
                    nextState = Tools.stateSaver;
                }

                //parse object
                try
                {
                    string intent = FindSingleJsonObject(responseContent, "intent");
                    List<string> entities = FindManyJsonObject(responseContent, "entities");

                    //set default response values
                    Intent = intent;
                    Entities = new List<string>();
                    Entities.AddRange(entities);

                    //parse intent/entities -> output
                    suggestionUtterance();

                }
                catch (Exception ex)
                {
                    this.Error = "FATAL suggestionING ERROR - " + ex.ToString();
                }
            }

            private static string FindSingleJsonObject(string response, string itemToFind)
            {
                Newtonsoft.Json.Linq.JObject jsonObject = Newtonsoft.Json.Linq.JObject.Parse(response);
                Newtonsoft.Json.Linq.JToken tokens = jsonObject[itemToFind];

                Newtonsoft.Json.Linq.JToken firstToken = tokens.FirstOrDefault();
                string name = firstToken.First.ToString();


                return name;
            }
            private static List<string> FindManyJsonObject(string stringToParse, string itemToFind)
            {
                List<string> stringList = new List<string>();
                try
                {
                    Newtonsoft.Json.Linq.JObject jsonObject = Newtonsoft.Json.Linq.JObject.Parse(stringToParse);
                    Newtonsoft.Json.Linq.JToken tokens = jsonObject[itemToFind];

                    foreach (var item in tokens.Children())
                    {
                        stringList.Add(item.Value<string>("value") ?? "");
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                return stringList;
            }

            private List<UserIntent> GenerateIntents()
            {
                var intentList = new List<UserIntent>();

                //default intents
                //add more if for some reason it can't fin d the utterance file
                intentList.Add(new UserIntent("greet", "", new string[] { "hello!", "hey!", "Howdy", "How's it going?", "Hi" }));
                intentList.Add(new UserIntent("affirm", "", new string[] { "got it!", "sure!" }));
                //sample utterance generation
                //System.IO.File.WriteAllText(@"C:\Users\JB84440\Desktop\utterances.json", JsonConvert.SerializeObject(intentList));

                return intentList;
            }
            private List<UserIntent> GenerateIntents(string pathToJsonFile)
            {
                var intentList = new List<UserIntent>();

                if (System.IO.File.Exists(pathToJsonFile))
                {
                    try
                    {
                        intentList = (List<UserIntent>)JsonConvert.DeserializeObject(System.IO.File.ReadAllText(pathToJsonFile), typeof(List<UserIntent>));
                    }
                    catch (Exception)
                    {
                        throw new Exception("Error Deserializing JSON file - " + pathToJsonFile);
                    }

                }
                else
                {
                    throw new Exception("Intent File not found at " + pathToJsonFile);
                }
                return intentList;
            }
            private void suggestionUtterance()
            {
                string utteranceFile = AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + Tools.utteranceFilePath;

                List<UserIntent> intentList;
                UserIntent intent = new UserIntent();

                if (System.IO.File.Exists(utteranceFile))
                {
                    intentList = GenerateIntents(utteranceFile);
                }
                else
                {
                    intentList = GenerateIntents();
                }

                Match match = null;


                //intent override, if you need to make a linear survey like use case
                intent.Intent = Survey.check(this);


                //linq query to get the correct intent match with many utterances
                if (Entities.Count == 0 || match.Success)
                {
                    intent = intentList.Where(f => f.Intent.ToLower() == Intent.ToLower()).FirstOrDefault();
                }
                else
                {
                    intent = intentList.Where(f => (f.Intent.ToLower() == Intent.ToLower() && f.Entity.ToLower() == Entities[0].ToLower())).FirstOrDefault();
                }


                if (intent != null)
                {
                    //picks a specific one out of many utterances
                    string rawUtterance = intent.GetUtterance();
                    this.RawUtterance = rawUtterance;

                    //turning utterance nextstate string into the next state enum and saving it to the object
                    State state;
                    if (Enum.TryParse(intent.nextState, out state))
                    {
                        this.previousState = this.nextState;
                        this.nextState = state;
                    }

                    string suggestionedUtterance = rawUtterance;
                    var potentialEntities = suggestionedUtterance.Split('{', '}');


                    foreach (var potentialEntity in potentialEntities)
                    {
                        var currentEntity = potentialEntity;

                        if ((currentEntity.Length < 7) || (currentEntity == ""))
                        {
                            continue;
                        }

                        //TODO: Does not work, line: string strEntityIndex = currentEntity.Substring(7, 1);
                        //check and replace utterance

                        if (currentEntity.Substring(0, 7) == "ENTITY[")
                        {
                            string sourceString = "{" + currentEntity + "}";
                            string strEntityIndex = currentEntity.Substring(7, 1);
                            int entityIndex = int.Parse(strEntityIndex);
                            string replacementString;

                            try
                            {
                                replacementString = Entities[entityIndex];
                                suggestionedUtterance = suggestionedUtterance.Replace(sourceString, replacementString);
                            }
                            catch (Exception)
                            {
                                this.Error = "ENTITY ERROR - Index of required Entity Not Found! " + sourceString;
                                return;
                            }
                        }
                    }

                    this.suggestionedUtterance = suggestionedUtterance;

                }
                else
                {
                    this.Error = "INTENT ERROR - Could not find the intent";
                }

            }

        }

        public class UserIntent
        {
            public string Intent { get; set; }
            public string Entity { get; set; }
            public List<string> Utterances { get; set; }
            public string nextState { get; set; }
            public UserIntent()
            {
                //initialize items by default
                Utterances = new List<string>();

            }
            public UserIntent(string intent, string entity, string[] utterances)
            {
                //initialize items by default
                Intent = intent;
                Entity = entity;
                Utterances = new List<string>();
                Utterances.AddRange(utterances);
            }
            public string GetUtterance()
            {
                if (Utterances.Count == 0)
                {
                    //return that we failed to find an item
                    return "UTTERANCES ERROR - I could not find any utterances for the intent!";
                }
                else
                {
                    //get random utterance
                    Random rnd = new Random();
                    int itemNumber = rnd.Next(0, Utterances.Count - 1); // creates a number between 1 and 12
                    return Utterances[itemNumber];
                }

            }

        }

        #endregion

    }
}
