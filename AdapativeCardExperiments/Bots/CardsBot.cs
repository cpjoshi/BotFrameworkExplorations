// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json.Linq;

namespace AdapativeCardExperiments.Bots
{
    public class CardsBot<T> : TeamsActivityHandler where T: Dialog
    {
        private const string taskUrl = "https://botexplorations.azurefd.net/message?host=msteams";
        private Dialog _promptDialog;
        private ConversationState _conversationState;
        public CardsBot(T dialog, ConversationState conversationState)
        {
            _promptDialog = dialog;
            _conversationState = conversationState;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            turnContext.Activity.RemoveRecipientMention();
            var commandName = turnContext.Activity.Text?.Trim();

            if(string.IsNullOrEmpty(commandName))
            {
                var value = turnContext.Activity.Value;
                return;
            }

            AdaptiveCard card = null;
            switch (commandName)
            {
                case "marks":
                    {
                        var result = AdaptiveCard.FromJson(File.ReadAllText(@".\Cards\json\MarksForm.json"));
                        card = result.Card;
                    }
                    break;

                case "img-base64":
                    {
                        var result = AdaptiveCard.FromJson(File.ReadAllText(@".\Cards\json\ImageCard.json"));
                        card = result.Card;
                    }
                    break;
                
                case "img":
                    {
                        var result = AdaptiveCard.FromJson(File.ReadAllText(@".\Cards\json\ImageCardUrl.json"));
                        card = result.Card;
                    }
                    break;

                case "prompts":
                    {
                        await _promptDialog.RunAsync(turnContext, 
                            _conversationState.CreateProperty<DialogState>(nameof(DialogState)), 
                            cancellationToken);
                        return;
                    }

                case "carousel":
                    {
                        Activity replyToConversation = MessageFactory.Text("Should see in carousel format");
                        replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        replyToConversation.Attachments = new List<Attachment>();

                        var card1 = AdaptiveCard.FromJson(File.ReadAllText(@".\Cards\json\card1.json"));
                        var card2 = AdaptiveCard.FromJson(File.ReadAllText(@".\Cards\json\card2.json"));

                        replyToConversation.Attachments.Add(CreateApativeCardAttachment(card1.Card));
                        replyToConversation.Attachments.Add(CreateApativeCardAttachment(card2.Card));

                        await turnContext.SendActivityAsync(replyToConversation);
                        return;
                    }

                case "invoke":
                    {
                        var c = new HeroCard
                        {
                            Title = "Task Module Demo",
                            Text = "Click the below button to launch Task Module",

                            Buttons = new List<CardAction>
                            {
                                new CardAction
                                {
                                    Type= "invoke",
                                    Title = "Open Task Module",
                                    Text = "Open Task Module",
                                    Value = "{\"type\": \"task/fetch\", \"data\": \"alertform\"}"
                                },
                                new CardAction
                                {
                                    Type = "openUrl",
                                    Title = "StageViewDeeplink",
                                    Value = "https://teams.microsoft.com/l/stage/2a527703-1f6f-4559-a332-d8a7d288cd88/0?context={\"contentUrl\":\"https%3A%2F%2Fmicrosoft.sharepoint.com%2Fteams%2FLokisSandbox%2FSitePages%2FSandbox-Page.aspx\", \"websiteURL\":\"https%3A%2F%2Fmicrosoft.sharepoint.com%2Fteams%2FLokisSandbox%2FSitePages%2FSandbox-Page.aspx\", \"title\":\"Contoso\"}",
                                }
                            }
                        };
                        await turnContext.SendActivityAsync(MessageFactory.Attachment(c.ToAttachment()));
                        return;
                    }

                default:
                    await turnContext.SendActivityAsync(MessageFactory.Text("Complete!"), cancellationToken);
                    return;
            }
            await turnContext.SendActivityAsync(
                MessageFactory.Attachment(CreateApativeCardAttachment(card)), 
                cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
        private Attachment CreateApativeCardAttachment(AdaptiveCard card)
        {
            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = card
            };
            return adaptiveCardAttachment;

        }

        protected override Task<TaskModuleResponse> OnTeamsTaskModuleFetchAsync(ITurnContext<IInvokeActivity> turnContext, 
            TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        {
            var asJobject = JObject.FromObject(taskModuleRequest.Data);
            var formname = asJobject["data"].ToString();

            TaskModuleResponse taskModuleResponse = null;
            switch (formname)
            {
                case "alertform":
                    {
                        var taskInfo = new TaskModuleTaskInfo();
                        taskInfo.Url = taskUrl;
                        taskInfo.Width = 510;
                        taskInfo.Height = 500;
                        taskInfo.Title = "Fetched from bot";

                        taskModuleResponse = new TaskModuleResponse
                        {
                            Task = new TaskModuleContinueResponse()
                            {
                                Value = taskInfo,
                            },
                        };
                    }
                    break;
                default:
                    break;
            }


            return Task.FromResult(taskModuleResponse);
        }

        protected override async Task<TaskModuleResponse> OnTeamsTaskModuleSubmitAsync(
            ITurnContext<IInvokeActivity> turnContext, 
            TaskModuleRequest taskModuleRequest, 
            CancellationToken cancellationToken)
        {
            var asJobject = JObject.FromObject(taskModuleRequest.Data);
            var data = asJobject.ToString();

            var reply = MessageFactory.Text("OnTeamsTaskModuleSubmitAsync Data Sent: " + data);
            _ = turnContext.SendActivityAsync(reply);

            return null;
        }

    }
}
