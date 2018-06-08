using System;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using AdaptiveCards;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Teams.Samples.HelloWorld.Web.Helper;
using System.IO;


namespace Microsoft.Teams.Samples.HelloWorld.Web.Dialogs
{

    /// <summary>
    /// This Dialog enables the user to issue a set of commands against AAD
    /// to do things like list recent email, send an email, and identify the user
    /// This Dialog also makes use of the GetTokenDialog to help the user login
    /// </summary>
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        /// <summary>
        /// This is the name of the OAuth Connection Setting that is configured for this bot
        /// </summary>
        private static string ConnectionName = ConfigurationManager.AppSettings["ConnectionName"];
        private static string BaseUri = ConfigurationManager.AppSettings["BaseUri"];

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        // This sample dialog shows two simple flows:
        // 1) A silly example of receiving a file from the user, processing the key elements,
        //    and then constructing the attachment and sending it back.
        // 2) Creating a new file consent card requesting user permission to upload a file.
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var replyMessage = context.MakeMessage();
            Attachment returnCard;

            var message = await result as Activity;

            // Check to see if the user is sending the bot a file.
            if (message.Attachments != null && message.Attachments.Any())
            {
                var attachment = message.Attachments.First();

                if (attachment.ContentType == FileDownloadInfo.ContentType)
                {
                    FileDownloadInfo downloadInfo = (attachment.Content as JObject).ToObject<FileDownloadInfo>();
                    if (downloadInfo != null)
                    {
                        returnCard = CreateFileInfoAttachment(downloadInfo, attachment.Name, attachment.ContentUrl);
                        replyMessage.Attachments.Add(returnCard);
                    }
                }
            }
            else
            {
                // Illustrates creating a file consent card.
                returnCard = CreateFileConsentAttachment();
                replyMessage.Attachments.Add(returnCard);
            }
            await context.PostAsync(replyMessage);
        }


        private static Attachment CreateFileInfoAttachment(FileDownloadInfo downloadInfo, string name, string contentUrl)
        {
            FileInfoCard card = new FileInfoCard()
            {
                FileType = downloadInfo.FileType,
                UniqueId = downloadInfo.UniqueId
            };

            Attachment att = card.ToAttachment();
            att.ContentUrl = contentUrl;
            att.Name = name;

            return att;
        }

        private static Attachment CreateFileConsentAttachment()
        {
            JObject acceptContext = new JObject();
            // Fill in any additional context to be sent back when the user accepts the file.

            JObject declineContext = new JObject();
            // Fill in any additional context to be sent back when the user declines the file.


            var filePath = @"E:\Wajeed\To Upload\Resume - Wajeed Shaikh.docx";  //@"F:\Projects\TestProjects\MSTeamsSample\MSTeamsSample\Files\Test File.docx";
            var fileName = Path.GetFileName(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            FileConsentCard card = new FileConsentCard()
            {
                AcceptContext = acceptContext,
                DeclineContext = declineContext,
                SizeInBytes = fileStream.Length,
                Description = "This is my resume."
            };

            Attachment att = card.ToAttachment();
            att.Name = "Resume - Wajeed Shaikh.docx";

            return att;
        }

        private static async Task UplaodFileToSharepoint(IDialogContext context, TokenResponse tokenResponse)
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

            var filePath = @"F:\Projects\TestProjects\MSTeamsSample\MSTeamsSample\Files\Test File.docx";
            var fileName = Path.GetFileName(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var endpoint = $"https://graph.microsoft.com/beta/groups/6f94967e-7b15-48d6-8cdd-d31fc85389db/drive/items/root:/General/{fileName}:/content";

            // Create an HTTPClient instance to communicate with the REST API of OneDrive
            using (var client = new HttpClient())
            {
                // Load the content to upload
                using (var content = new StreamContent(fileStream))
                {
                    // Indicate that we're sending binary data
                    content.Headers.Add("Content-Type", MimeMapping.GetMimeMapping(fileName));

                    // Construct the PUT message towards the webservice
                    using (var request = new HttpRequestMessage(HttpMethod.Put, endpoint))
                    {
                        // Set the content to upload
                        request.Content = content;

                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

                        // Request the response from the webservice
                        using (var response = await client.SendAsync(request))
                        {
                            // Check the response.
                        }

                    }
                }
            }
        }


    }
}