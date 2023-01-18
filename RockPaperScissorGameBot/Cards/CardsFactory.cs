using AdaptiveCards;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RockPaperScissorGameBot.Cards
{
    public class CardsFactory
    {
        public Attachment CreateThankYouCardAttachment(string memberName)
        {
            var card = new HeroCard
            {
                Title = "Rock, Paper, Scissor Game",
                Text = $"Thank You {memberName} for playing. We will send you the scorecard when the game is over."
            };
            return card.ToAttachment();
        }

        public Attachment CreateGameCardAttachment(string memberName, string gameId)
        {
            var rockValue = new JObject {
                { "choice", "rock" },
                { "user", memberName },
                { "GameId", gameId }
            };

            var paperValue = new JObject {
                { "choice", "paper" },
                { "user", memberName },
                { "GameId", gameId }
            };

            var scissorValue = new JObject {
                { "choice", "scissor" },
                { "user", memberName },
                { "GameId", gameId }
            };

            var card = new HeroCard
            {
                Title = "Welcome to Rock, Paper, Scissor Game",
                Text = "Choose one of the following:",

                Buttons = new List<CardAction>
                        {
                            new CardAction
                            {
                                Type= ActionTypes.MessageBack,
                                Title = "Rock",
                                Text = "Rock",
                                Value = rockValue
                            },
                            new CardAction
                            {
                                Type= ActionTypes.MessageBack,
                                Title = "Paper",
                                Text = "Paper",
                                Value = paperValue
                            },
                            new CardAction
                            {
                                Type= ActionTypes.MessageBack,
                                Title = "Scissor",
                                Text = "Scissor",
                                Value = scissorValue
                            }
                        }
            };
            
            return card.ToAttachment();
        }

        public Attachment CreateScoreCardAttachment(Dictionary<string, int> playerScores)
        {
            List<AdaptiveFact> facts = new List<AdaptiveFact>();
            foreach (var key in playerScores.Keys)
            {
                facts.Add(new AdaptiveFact(key, playerScores[key].ToString(CultureInfo.InvariantCulture)));
            }

            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 2))
            {
                Body = new List<AdaptiveElement>()
                {
                    new AdaptiveTextBlock( "Players Scoreborad:"),
                    new AdaptiveFactSet()
                    {
                        Facts = facts
                    }
                }
            };
            return CreateApativeCardAttachment(card);
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
