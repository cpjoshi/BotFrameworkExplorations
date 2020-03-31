using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SuggestedActionsToCardActions.Utils
{
    public static class TurnContextExtensions
    {
        private static string PreviousSuggestedActionActivityId = string.Empty;

        public async static Task<ResourceResponse> SendActivityAsync(
            this ITurnContext turnContext,
            IMessageActivity activity, 
            CancellationToken cancellationToken = default,
            bool bReplaceSuggestedActions = false)
        {
            //webchat does not support updateactivity
            if(turnContext.Activity.ChannelId == "webchat" || 
                activity.SuggestedActions == null || 
                !bReplaceSuggestedActions)
            {
                PreviousSuggestedActionActivityId = string.Empty;
                return await turnContext.SendActivityAsync(activity, cancellationToken);
            }

            //This is to clear the UI for the previous suggested actions
            await turnContext.ClearLastActivity(cancellationToken);

            //Create a new card having suggested action buttons
            var card = new HeroCard() { 
                Buttons = activity.SuggestedActions.Actions 
            };

            //Remove suggested actions from origianl activity
            activity.SuggestedActions = null;

            //send origianl activity
            await turnContext.SendActivityAsync(activity, cancellationToken);

            //send suggested actions card
            var response = await turnContext.SendActivityAsync(
                MessageFactory.Attachment(card.ToAttachment()), 
                cancellationToken);

            PreviousSuggestedActionActivityId = response.Id;
            return response;
        }

        public async static Task<ResourceResponse> ClearLastActivity (
            this ITurnContext turnContext,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(PreviousSuggestedActionActivityId))
                return null;

            //var blankActivity = MessageFactory.Text("Clear");
            //blankActivity.Id = PreviousSuggestedActionActivityId;
            //return await turnContext.UpdateActivityAsync(blankActivity, cancellationToken);

            await turnContext.DeleteActivityAsync(PreviousSuggestedActionActivityId, cancellationToken);
            return null;
        }
    }
}
