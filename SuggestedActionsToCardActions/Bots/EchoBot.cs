// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace SuggestedActionsToCardActions.Bots
{
    public class EchoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("Choose one of the following");
            switch (turnContext.Activity.Text)
            {
                case "Start":
                    {
                        Activity welcome = WelcomeMessage();
                        await turnContext.SendActivityAsync(welcome, cancellationToken);
                        return;
                    }
                case "ViewMyProfile":
                    {
                        IMessageActivity cardReply = GetProfileCard();
                        cardReply.SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                                new CardAction() {Title = "Home", Type=ActionTypes.ImBack, Value="Home"},
                                new CardAction() {Title = "Cancel", Type=ActionTypes.ImBack, Value="Cancel"}
                            }
                        };

                        await turnContext.SendActivityAsync(cardReply, cancellationToken);
                        return;
                    }

                case "EmployeeProfile":
                    {
                        reply.SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                                new CardAction() { Title = "View My Profile", Type = ActionTypes.ImBack, Value = "ViewMyProfile" },
                                new CardAction() { Title = "Create Profile", Type = ActionTypes.ImBack, Value = "CreateProfile" },
                                new CardAction() { Title = "Update Profile", Type = ActionTypes.ImBack, Value = "UpdateProfile" },
                                new CardAction() { Title = "Delete Profile", Type = ActionTypes.ImBack, Value = "DeleteProfile" },
                            },
                        };
                    }
                    break;

                case "ApplyLeave":
                    {
                        reply.SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                                new CardAction() { Title = "Maternity Leave", Type = ActionTypes.ImBack, Value = "MaternityLeave" },
                                new CardAction() { Title = "Paternity Leave", Type = ActionTypes.ImBack, Value = "PaternityLeave" },
                                new CardAction() { Title = "Sick Leave", Type = ActionTypes.ImBack, Value = "SickLeave" },
                            },
                        };
                    }
                    break;

                case "SeePaySlip":
                    {
                        reply.SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                                new CardAction() { Title = "For this month", Type = ActionTypes.ImBack, Value = "Forthismonth" },
                                new CardAction() { Title = "For last month", Type = ActionTypes.ImBack, Value = "Forlastmonth" },
                            },
                        };
                    }
                    break;

                default:
                    reply = MessageFactory.Text("End", "End");
                    break;
            }
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            Activity reply = WelcomeMessage();

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }
        }

        private static Activity WelcomeMessage()
        {
            var reply = MessageFactory.Text("Hello and welcome, choose one of the actions below?");

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "Employee Profile", Type = ActionTypes.ImBack, Value = "EmployeeProfile" },
                    new CardAction() { Title = "Apply Leave", Type = ActionTypes.ImBack, Value = "ApplyLeave" },
                    new CardAction() { Title = "See Payslip", Type = ActionTypes.ImBack, Value = "SeePayslip" },
                },
            };
            return reply;
        }

        private static IMessageActivity GetProfileCard()
        {
            var card = new HeroCard()
            {
                Title = "Your Profile - Card with suggested actions",
                Subtitle = "Name: C P Joshi",
                Buttons = new List<CardAction>
                            {
                                new CardAction
                                {
                                    Type= ActionTypes.MessageBack,
                                    Title = "One",
                                    Text = "One",
                                },
                                new CardAction
                                {
                                    Type= ActionTypes.MessageBack,
                                    Title = "Two",
                                    Text = "Two",
                                },
                                new CardAction
                                {
                                    Type= ActionTypes.MessageBack,
                                    Title = "Three",
                                    Text = "Three",
                                }
                            }
            };

            var cardReply = MessageFactory.Attachment(card.ToAttachment());
            return cardReply;
        }



    }
}
