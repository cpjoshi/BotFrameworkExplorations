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
using Microsoft.Bot.Schema;

namespace AdapativeCardExperiments.Bots
{
    public class CardsBot : ActivityHandler
    {
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

                case "image":
                    {
                        var result = AdaptiveCard.FromJson(File.ReadAllText(@".\Cards\json\ImageCard.json"));
                        card = result.Card;
                    }
                    break;
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
    }
}
