using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace BizSpark
{
    [Serializable]
    public class SimpleDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(ActivityReceivedAsync);
        }

        private async Task ActivityReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var reply2 = activity.CreateReply();
            reply2.Attachments = new List<Attachment>();

            if (activity.Text.Contains("how many teams"))
            {

                await context.PostAsync($"There are teams.");
            }
            else if (activity.Text.Contains("How can I activate")|| activity.Text.Contains("activate"))
            {
            

                reply2.Attachments.Add(new Attachment()
                {
                    
                ContentUrl = "https://s-media-cache-ak0.pinimg.com/originals/ff/2b/b6/ff2bb6fa6a49798b3630dcbe551b2171.jpg",
                    ContentType = "image/jpg",
                    Name = "QualityControl.jpg"
                });
                await context.PostAsync(reply2);
                context.Wait(ActivityReceivedAsync);
            
           }
            else
            {

                if (activity.Type == ActivityTypes.Message)
                {
                    ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    var responseString = String.Empty;
                    var responseMsg = "";

                    //De-serialize the response
                    QnAMakerResult QnAresponse;

                    // Send question to API QnA bot
                    if (activity.Text.Length > 0)
                    {
                        var knowledgebaseId = "e7e838d2-35a5-4cb1-83ef-c8fc1f91bb8f"; // Use knowledge base id created.
                        var qnamakerSubscriptionKey = "ee9e28c9d0204de788f504dbe3d5448a"; //Use subscription key assigned to you.

                        //Build the URI
                        Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v1.0");
                        var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{knowledgebaseId}/generateAnswer");

                        //Add the question as part of the body
                        var postBody = $"{{\"question\": \"{activity.Text}\"}}";

                        //Send the POST request
                        using (WebClient client = new WebClient())
                        {
                            //Set the encoding to UTF8
                            client.Encoding = System.Text.Encoding.UTF8;

                            //Add the subscription key header
                            client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
                            client.Headers.Add("Content-Type", "application/json");
                            responseString = client.UploadString(builder.Uri, postBody);
                        }

                        try
                        {
                            QnAresponse = JsonConvert.DeserializeObject<QnAMakerResult>(responseString);
                            responseMsg = QnAresponse.Answer.ToString();
                        }
                        catch
                        {
                            throw new Exception("Unable to deserialize QnA Maker response string.");
                        }
                    }

                    // return our reply to the user
                    Activity reply = activity.CreateReply(responseMsg);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
                context.Wait(ActivityReceivedAsync);
            }
        }
    }
}