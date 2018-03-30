using System;


namespace chatbotHelper
{
    public static class Survey
    {
        
        public static string check(Tools.ChatAPIResponse chatResponse)
        {
            chatResponse.previousState = chatResponse.nextState;
            switch (chatResponse.nextState)
            {
                case Tools.State.suggestionName:
                    {
                        chatResponse.Intent = "suggestionName";
                        break;
                    }
                case Tools.State.selectOrg:
                    {
                        chatResponse.Intent = "selectOrg";
                        break;
                    }
                case Tools.State.suggestionDescription:
                    {
                        chatResponse.Intent = "suggestionDescription";
                        break;
                    }
                case Tools.State.suggestionOwners:
                    {
                        chatResponse.Intent = "suggestionOwners";
                        break;
                    }
                case Tools.State.suggestionTime:
                    {
                        chatResponse.Intent = "suggestionTime";
                        break;
                    }
                case Tools.State.suggestionSave:
                    {
                        chatResponse.Intent = "suggestionSave";
                        break;
                    }
                case Tools.State.suggestionQA:
                    {
                        chatResponse.Intent = "suggestionQA";
                        break;
                    }
                case Tools.State.suggestionDecom:
                    {
                        chatResponse.Intent = "suggestionDecom";
                        break;
                    }
                case Tools.State.suggestionFrequency:
                    {
                        chatResponse.Intent = "suggestionFrequency";
                        break;
                    }
                case Tools.State.suggestionInteractions:
                    {
                        chatResponse.Intent = "suggestionInteractions";
                        break;
                    }
                default:
                    break;
            }

            return chatResponse.Intent;
        }
        public static void saveInModel(Tools.ChatAPIRequest chatRequest)
        {
            //add saving to data model here
            //After completion you can push to your datastore
        }
    }

}
