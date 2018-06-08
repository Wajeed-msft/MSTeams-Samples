using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;

namespace Microsoft.Teams.Samples.HelloWorld.Web.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            using (var connector = new ConnectorClient(new Uri(activity.ServiceUrl)))
            {
                if (activity.IsComposeExtensionQuery())
                {
                    var response = MessageExtension.HandleMessageExtensionQuery(connector, activity);
                    return response != null
                        ? Request.CreateResponse<ComposeExtensionResponse>(response)
                        : new HttpResponseMessage(HttpStatusCode.OK);
                }
                else if (activity.Type == ActivityTypes.Message)
                {
                    await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                }
                else if (activity.Type == ActivityTypes.Invoke)
                {
                    if (activity.Name == "signin/verifyState")
                    {
                        await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                    }
                    if (activity.Name == "fileConsent/invoke")
                    {

                        FileConsentCardResponse downloadInfo = (activity.Value as Newtonsoft.Json.Linq.JObject).ToObject<FileConsentCardResponse>();
                        if (downloadInfo != null)
                        {
                            try
                            {
                                await UplaodFileToSharepoint(downloadInfo);
                            }
                            catch (Exception cx)
                            {

                                Console.WriteLine(cx);
                                //HeroCard card = new HeroCard();
                                //card.Buttons = new List<CardAction>();
                                //card.Buttons.Add( new CardAction())
                            }

                            
                            //returnCard = CreateFileInfoAttachment(downloadInfo, attachment.Name, attachment.ContentUrl);
                            //replyMessage.Attachments.Add(returnCard);
                        }
                    }
                }

            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }


        private static async Task UplaodFileToSharepoint(FileConsentCardResponse info)
        {


            // var bytes = System.IO.File.ReadAllText(fileInfo.FullName);


            //    var fileContent = new StreamContent(stream);

            //    // var fileContent = new StringContent(bytes);// ByteArrayContent(bytes);
            //    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse( MimeMapping.GetMimeMapping(fileInfo.Name));
            //// MediaTypeHeaderValue.Parse("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            //    var requestContent = new MultipartFormDataContent();
            //    requestContent.Add(fileContent, "File", fileInfo.Name);

            //    var request = new HttpRequestMessage(HttpMethod.Put, endpoint);
            //    request.Headers.Authorization =
            //new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
            //    request.Content = fileContent;
            //    var client = new HttpClient();
            //    var response = client.SendAsync(request).Result;
            //    Console.WriteLine(response);

            // 6f94967e-7b15-48d6-8cdd-d31fc85389db
            // FileInfo fileInfo = new FileInfo(@"E:\Wajeed\To Upload\Resume - Wajeed Shaikh.docx");
            // @"E:\Wajeed\To Upload\Resume - Wajeed Shaikh.docx";

            var filePath = @"E:\Wajeed\To Upload\Resume - Wajeed Shaikh.docx";  //@"F:\Projects\TestProjects\MSTeamsSample\MSTeamsSample\Files\Test File.docx";
            var fileName = Path.GetFileName(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var length = fileStream.Length;
            var endpoint = info.UploadInfo.UploadUrl; //$"https://graph.microsoft.com/beta/groups/6f94967e-7b15-48d6-8cdd-d31fc85389db/drive/items/root:/General/{fileName}:/content";

            // Create an HTTPClient instance to communicate with the REST API of OneDrive
            using (var client = new HttpClient())
            {
                // Load the content to upload
                using (var content = new StreamContent(fileStream))
                {
                    // Indicate that we're sending binary data
                    content.Headers.Add("Content-Type", System.Web.MimeMapping.GetMimeMapping(fileName));

                    content.Headers.Add("Content-Length", length.ToString());
                    // content.Headers.Add("Content-Range", $"bytes 0-{length}/{length}");
                    // content.Headers.Add Add("Content-Range", $"bytes 0-{length}/{length}");
                    content.Headers.Add("Content-Range", $"bytes 0 -{length}/{length}");
                    //     request.Content.Headers.ContentRange =
                    //new ContentRangeHeaderValue(this.RangeBegin, this.RangeEnd, this.TotalSessionLength);

                    // Construct the PUT message towards the webservice
                    using (var request = new HttpRequestMessage(HttpMethod.Put, endpoint))
                    {
                        // Set the content to upload
                       

                        // request.Headers.Add("Content-Range", length.ToString());

                        // Content - Range

                        // Request the response from the webservice
                        try
                        {
                            request.Content = content;

                            var token = await new MicrosoftAppCredentials().GetTokenAsync();
                            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                            // request.Headers.Add("Content-Range", $"bytes 0 -{length}/{length}");
                            using (var response = await client.SendAsync(request))
                            {
                                // Check the response.
                            }
                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine(ex);
                        }


                    }
                }
            }
        }

    }
}
