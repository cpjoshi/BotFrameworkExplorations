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

namespace AdapativeCardExperiments.Bots
{
    public class CardsBot<T> : TeamsActivityHandler where T: Dialog
    {
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

        protected override async Task<TaskModuleResponse> OnTeamsTaskModuleSubmitAsync(
            ITurnContext<IInvokeActivity> turnContext, 
            TaskModuleRequest taskModuleRequest, 
            CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("OnTeamsTaskModuleSubmitAsync Value: " + taskModuleRequest);
            await turnContext.SendActivityAsync(reply);

            return new TaskModuleResponse
            {
                Task = new TaskModuleMessageResponse()
                {
                    Value = "Thanks!",
                },
            };
        }

    }
}
