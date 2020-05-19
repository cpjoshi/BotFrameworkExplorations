// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

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

                case "Help":
                    {
                        reply = MessageFactory.Text("Did the resolution work for you?");
                        reply.SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                                new CardAction() {Title = "Yes", Type=ActionTypes.ImBack, Value="Yes"},
                                new CardAction() {Title = "No", Type=ActionTypes.ImBack, Value="No"}
                            }
                        };
                    }
                    break;

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
                                new CardAction() { Title = "20-Buttons", Type = ActionTypes.ImBack, Value = "20-Buttons" },
                            },
                        };
                    }
                    break;

                case "20-Buttons":
                    {
                        reply.SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                                new CardAction() { Title = "Button-1", Type = ActionTypes.ImBack, Value = "Button-1" },
                                new CardAction() { Title = "Button-2", Type = ActionTypes.ImBack, Value = "Button-2" },
                                new CardAction() { Title = "Button-3", Type = ActionTypes.ImBack, Value = "Button-3" },
                                new CardAction() { Title = "Button-4", Type = ActionTypes.ImBack, Value = "Button-4" },
                                new CardAction() { Title = "Button-5", Type = ActionTypes.ImBack, Value = "Button-5" },
                                new CardAction() { Title = "Button-6", Type = ActionTypes.ImBack, Value = "Button-6" },
                                new CardAction() { Title = "Button-7", Type = ActionTypes.ImBack, Value = "Button-7" },
                                new CardAction() { Title = "Button-8", Type = ActionTypes.ImBack, Value = "Button-8" },
                                new CardAction() { Title = "Button-9", Type = ActionTypes.ImBack, Value = "Button-9" },
                                new CardAction() { Title = "Button-10", Type = ActionTypes.ImBack, Value = "Button-10" },
                                new CardAction() { Title = "Button-11", Type = ActionTypes.ImBack, Value = "Button-11" },
                                new CardAction() { Title = "Button-12", Type = ActionTypes.ImBack, Value = "Button-12" },
                                new CardAction() { Title = "Button-13", Type = ActionTypes.ImBack, Value = "Button-13" },
                                new CardAction() { Title = "Button-14", Type = ActionTypes.ImBack, Value = "Button-14" },
                                new CardAction() { Title = "Button-15", Type = ActionTypes.ImBack, Value = "Button-15" },
                                new CardAction() { Title = "Button-16", Type = ActionTypes.ImBack, Value = "Button-16" },
                                new CardAction() { Title = "Button-17", Type = ActionTypes.ImBack, Value = "Button-17" },
                                new CardAction() { Title = "Button-18", Type = ActionTypes.ImBack, Value = "Button-18" },
                                new CardAction() { Title = "Button-19", Type = ActionTypes.ImBack, Value = "Button-19" },
                                new CardAction() { Title = "Button-20", Type = ActionTypes.ImBack, Value = "Button-20" },
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
                                //test messagebacks
                                new CardAction 
                                { 
                                    Title = "Maternity Leave",
                                    Type = ActionTypes.MessageBack,
                                    DisplayText = "Maternity Leave",
                                    Text = "Maternity",
                                    Value = new JObject { { "LeaveType", "Maternity" } }
                                },

                                new CardAction() { Title = "Paternity Leave",
                                    Type = ActionTypes.MessageBack,
                                    DisplayText = "Paternity Leave",
                                    Text = "Paternity",
                                    Value = new JObject { { "LeaveType", "Paternity" } } 
                                },

                                new CardAction() { Title = "Sick Leave",
                                    Type = ActionTypes.MessageBack,
                                    DisplayText = "Sick Leave",
                                    Text = "Sick",
                                    Value = new JObject {{ "LeaveType", "Sick" } } 
                                },

                                new CardAction() { Title = "Casual Leave",
                                    Type = ActionTypes.MessageBack,
                                    DisplayText = "Casual",
                                    Text = "Casual",
                                    Value = "Casual" 
                                }
                            },
                        };
                    }
                    break;

                case "Maternity":
                case "Paternity":
                case "Sick":
                case "Casual":
                    {
                        reply = MessageFactory.Text($"Value: {turnContext.Activity.Value}", "End");
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
            var reply = MessageFactory.Text("Hello and welcome, choose one of the actions below");

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
