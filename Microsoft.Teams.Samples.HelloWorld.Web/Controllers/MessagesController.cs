using System;
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
                }

            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }


    }
}
