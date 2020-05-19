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
                switch(cardAction.Type)
                {
                    case ActionTypes.ImBack:
                        {
                            var newCardAction = new CardAction()
                            {
                                Type = ActionTypes.MessageBack,
                                Title = cardAction.Title,
                                Text = cardAction.Value.ToString(),
                                DisplayText = cardAction.Value.ToString(),
                                Value = new JObject {
                                    { Constants.AddedBy, Constants.SuggestedActionsMiddleware },
                                    { "type",  ActionTypes.ImBack}
                                }
                            };
                            actionsList.Add(newCardAction);
                        }
                        break;

                    case ActionTypes.MessageBack:
                        {
                            var newValue = new JObject {
                                { "Value", cardAction.Value.ToString() },
                                { Constants.AddedBy, Constants.SuggestedActionsMiddleware },
                                { "type", ActionTypes.MessageBack }
                            };

                            cardAction.Value = newValue;
                            actionsList.Add(cardAction);
                        }
                        break;
                    
                    default:
                        throw new InvalidOperationException($"{cardAction.Type} suggestion action is not supported");
                }
            }
            return actionsList;
        }
    }
}
