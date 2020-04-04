﻿using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using SuggestedActionsToCardActions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SuggestedActionsToCardActions.Middleware
{
    public class SuggestedActionsWorkAroundMiddleware : IMiddleware
    {
        private const string TeamsChannelId = "msteams";

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            turnContext.OnSendActivities(async (newContext, activities, nextSend) =>
            {
                var suggestionCardActivities = new List<Activity>();
                foreach(var activity in activities.Where(act => (act.ChannelId == TeamsChannelId && act.SuggestedActions != null)))
                {
                    var newActivity = newContext.Activity.CreateReply();
                    newActivity.Attachments = new List<Attachment>();
                    //Create a new card having suggested action buttons
                    var suggestedActionCard = new HeroCard()
                    {
                        Buttons = activity.SuggestedActions.ToMessageBackActions()
                    };
                    newActivity.Attachments.Add(suggestedActionCard.ToAttachment());
                    suggestionCardActivities.Add(newActivity);
                    activity.SuggestedActions = null;
                }

                activities.AddRange(suggestionCardActivities);
                return await nextSend();
            });

            if (turnContext.Activity.ChannelId == TeamsChannelId && 
                turnContext.Activity.Value != null)
            {
                var obj = (JObject)turnContext.Activity.Value;
                if (obj != null
                    && obj.ContainsKey(Constants.AddedBy)
                    && obj[Constants.AddedBy].ToString() == Constants.SuggestedActionsMiddleware)
                {
                    turnContext.Activity.Text = obj["Value"].ToString();
                    //delete the original suggested actions card in which this button was clicked.
                    _ = turnContext.DeleteActivityAsync(turnContext.Activity.ReplyToId);
                }
            }

            await next(cancellationToken).ConfigureAwait(false);
        }
    }
}
