using Microsoft.Bot.Schema;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuggestedActionsToCardActions.Extensions
{
    public static class SuggestedActionsExtensions
    {
        public static List<CardAction> ToMessageBackActions(this SuggestedActions suggestedActions)
        {
            var actionsList = new List<CardAction>();
            foreach (var cardAction in suggestedActions.Actions)
            {
                cardAction.Type = ActionTypes.MessageBack;
                cardAction.DisplayText = cardAction.Value.ToString();
                cardAction.Value = new JObject {
                            {"Value", cardAction.Value.ToString() },
                            { Constants.AddedBy, Constants.SuggestedActionsMiddleware}
                        };
                actionsList.Add(cardAction);
            }
            return actionsList;
        }
    }
}
