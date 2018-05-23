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

        /// <summary>
        /// Supports the commands recents, send, me, and signout against the Graph API
        /// </summary>
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var message = Bot.Connector.Teams.ActivityExtensions.GetTextWithoutMentions(activity).ToLowerInvariant();

            if (message.Equals("me"))
            {
                // First ask Bot Service if it already has a token for this user
                var token = await context.GetUserTokenAsync(ConnectionName).ConfigureAwait(false);
                if (token != null)
                {
                    // await UplaodFileToSharepoint(context, token);
                    // use the token to do exciting things!
                    await ReplyProfileInfo(context, token);
                }
                else
                {
                    // If Bot Service does not have a token, send an OAuth card to sign in
                    await SendOAuthCardAsync(context, (Activity)context.Activity);
                }
            }
            else if (message.Equals("send"))
            {
                // First ask Bot Service if it already has a token for this user
                var token = await context.GetUserTokenAsync(ConnectionName).ConfigureAwait(false);
                if (token != null)
                {
                    // await UplaodFileToSharepoint(context, token);
                    // use the token to do exciting things!
                    // await GetAllUsersAndSendHelloMessage(context, token);
                    await ReplyProfileInfo(context, token);
                }
                else
                {
                    // If Bot Service does not have a token, send an OAuth card to sign in
                    await SendOAuthCardAsync(context, (Activity)context.Activity);
                }
            }
            else if (message.ToLowerInvariant().Equals("signout"))
            {
                // Sign the user out from AAD
                await Signout(context);
            }
            else if (message.Contains("upload"))
            {
                // Check the attachment and upload to specific folder
                await ReceiveAttachments(context, activity);

            }
            else
            {
                await context.PostAsync("You can type 'me', 'upload' & 'signout' commands.");
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task GetAllUsersAndSendHelloMessage(IDialogContext context, TokenResponse token)
        {
            try
            {

                var client = new SimpleGraphClient(token.Token);

                var allUsers = await client.GetAllUsers();
                var data = context.Activity.GetChannelData<TeamsChannelData>();

                var tenantId = data.Tenant.Id;

                var botAccount = new ChannelAccount(context.Activity.Recipient.Id, context.Activity.Recipient.Name);

                foreach (var user in allUsers)
                {
                    var userAccount = new ChannelAccount(user.Id, user.DisplayName);
                    await SendWelcomeMessage(context, botAccount, userAccount, tenantId);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }

        }

        private async Task SendWelcomeMessage(IDialogContext context, ChannelAccount from, ChannelAccount to, string tenantId)
        {

            //// var channelId = this.channelId;
            //var serviceURL = context.Activity.ServiceUrl;
            //var connector = new ConnectorClient(new Uri(serviceURL));
            ////var channelData = new Dictionary<string, string>();
            ////channelData["teamsChannelId"] = channelId;

            //// Create a new reply.
            //IMessageActivity newMessage = Activity.CreateMessageActivity();
            //newMessage.Type = ActivityTypes.Message;

            //// var card = GetHeroCard(); 
            ////var card = GetConnectorCard();
            ////newMessage.Attachments.Add(card.ToAttachment());

            //ConversationParameters conversationParams = new ConversationParameters(
            //    isGroup: true,
            //    bot: null,
            //    members: new[] { context.Activity.From },
            //    topicName: "Test Conversation",
            //    activity: (Activity)newMessage,
            //    channelData: null);
            //MicrosoftAppCredentials.TrustServiceUrl(serviceURL, DateTime.MaxValue);
            //await connector.Conversations.CreateConversationAsync(conversationParams);





            var connector = new ConnectorClient(new Uri(context.Activity.ServiceUrl));
            // Create or get existing chat conversation with user
            var response = connector.Conversations.CreateOrGetDirectConversation(from, to, tenantId);

            // Construct the message to post to conversation
            Activity newActivity = new Activity()
            {
                Text = "Hello",
                Type = ActivityTypes.Message,
                Conversation = new ConversationAccount
                {
                    Id = response.Id
                },
            };

            // Post the message to chat conversation with user
            await connector.Conversations.SendToConversationAsync(response.Id, newActivity);
        }

        private async Task SendOAuthCardAsync(IDialogContext context, Activity activity)
        {
            await context.PostAsync($"To do this, you'll first need to sign in.");

            var reply = await context.Activity.CreateOAuthReplyAsync(ConnectionName, "Please sign in", "Sign In", true).ConfigureAwait(false);
            //var fallbackUrl = (reply.Attachments[0].Content as SigninCard).Buttons[0].Value.ToString();

            //fallbackUrl = fallbackUrl.Substring(0, fallbackUrl.IndexOf("redirectUri")) + "redirectUri=" + HttpUtility.UrlEncode("https://token.botframework.com/.auth/web/redirect");
            //(reply.Attachments[0].Content as SigninCard).Buttons[0].Value += "&fallbackUrl="+ HttpUtility.UrlEncode(fallbackUrl);

            await context.PostAsync(reply);

            context.Wait(WaitForToken);
        }

        private async Task WaitForToken(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var tokenResponse = activity.ReadTokenResponseContent();
            if (tokenResponse != null)
            {
                // Use the token to do exciting things!
            }
            else
            {
                // Get the Activity Message as well as activity.value in case of Auto closing of pop-up
                string input = activity.Type == ActivityTypes.Message ? activity.Text : ((dynamic)(activity.Value)).state.ToString();
                if (!string.IsNullOrEmpty(input))
                {
                    tokenResponse = await context.GetUserTokenAsync(ConnectionName, input.Trim());
                    if (tokenResponse != null)
                    {
                        try
                        {
                            // await UplaodFileToSharepoint(context, tokenResponse);
                            await ReplyProfileInfo(context, tokenResponse);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                        context.Wait(MessageReceivedAsync);
                        return;
                    }
                }
                await context.PostAsync($"Hmm. Something went wrong. Let's try again.");
                await SendOAuthCardAsync(context, activity);
            }
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


        private static async Task ReplyProfileInfo(IDialogContext context, TokenResponse tokenResponse)
        {
            var client = new SimpleGraphClient(tokenResponse.Token);

            var me = await client.GetMe();

            var photo = await client.GetProfilePhoto();
            var fileName = me.Id + "-ProflePhoto.png";
            var imagePath = System.Web.Hosting.HostingEnvironment.MapPath("~/ProfilePhotos/" + fileName);

            using (var fileStream = File.Create(imagePath))
            {
                photo.Seek(0, SeekOrigin.Begin);
                photo.CopyTo(fileStream);
            }

            Attachment attachment = null;
            try
            {
                #region Send Attachment Code
                // Send Attachment Code
                //using (var connector = new ConnectorClient(new Uri(context.Activity.ServiceUrl)))
                //{
                //    var attachments = new Attachments(connector);
                //    var attachmentData = new AttachmentData
                //    {
                //        Name = fileName,
                //        OriginalBase64 = File.ReadAllBytes(imagePath),
                //        Type = "image/png"
                //    };
                //    // context.Activity.Conversation.Id
                //    var response = await connector.Conversations.UploadAttachmentAsync(
                //         context.Activity.Conversation.Id, attachmentData
                //        );

                //    var attachmentUri = attachments.GetAttachmentUri(response.Id);

                //    attachment = new Attachment
                //    {
                //        Name = fileName,
                //        ContentType = "image/png",
                //        ContentUrl = attachmentUri
                //    };
                //}

                #endregion

                // Image attachment inline.
                //attachment = new Attachment
                //{
                //    Name = fileName,
                //    ContentType = "image/png",
                //    ContentUrl = BaseUri + "/ProfilePhotos/" + fileName
                //};


                var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

                attachment = new Attachment
                {
                    Name = fileName,
                    ContentType = "image/png",
                    ContentUrl = $"data:image/png;base64,{imageData}"
                };


                var msg = context.MakeMessage();
                msg.Text = $"You are {me.DisplayName} and here is you profile photo.";
                msg.Attachments.Add(attachment);
                await context.PostAsync(msg);



            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }


        }


        /// <summary>
        /// Signs the user out from AAD
        /// </summary>
        public static async Task Signout(IDialogContext context)
        {
            await context.SignOutUserAsync(ConnectionName);
            await context.PostAsync($"You have been signed out.");
        }

        private static async Task ReceiveAttachments(IDialogContext context, Activity activity)
        {
            foreach (var attachment in activity.Attachments)
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    // Get the download URL
                    var url = (attachment.Content as JObject)["downloadUrl"].ToString();

                    var responseMessage = await httpClient.GetAsync(url);
                    var contentLenghtBytes = responseMessage.Content.Headers.ContentLength;

                    // Read the Stream.
                    Stream attachmentStream = await responseMessage.Content.ReadAsStreamAsync();
                    attachmentStream.Position = 0;

                    await context.PostAsync($"Attachment of {attachment.ContentType} " +
                        $"type and size of {contentLenghtBytes} bytes received. Here are your files: " +
                        $"<a href=\"{attachment.ContentUrl.Replace(attachment.Name, "")}\">Chat Files</a>");
                    try
                    {
                        await SendAdaptiveCard(context, attachmentStream);
                    }
                    catch (Exception)
                    {
                        await context.PostAsync($"Could not parse adaptive card. Please try a valid Json.");
                    }
                }
            }
        }

        private static async Task SendAdaptiveCard(IDialogContext context, Stream attachmentStream)
        {
            StreamReader reader = new StreamReader(attachmentStream);
            string text = reader.ReadToEnd();

            var results = AdaptiveCard.FromJson(text);
            var card = results.Card;
            var cardAttachment = new Attachment()
            {
                Content = card,
                ContentType = AdaptiveCard.ContentType,
                Name = "Card"
            };

            var reply = context.MakeMessage();
            reply.Text = "Here is your Adaptive Card";
            reply.Attachments.Add(cardAttachment);
            await context.PostAsync(reply);
        }
    }
}