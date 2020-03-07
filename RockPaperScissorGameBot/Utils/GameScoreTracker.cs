using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RockPaperScissorGameBot.Cards;
using RockPaperScissorGameBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RockPaperScissorGameBot.Utils
{
    public class GameScoreTracker
    {
        private string _appId;
        private string _appPassword;
        private GameScore _gameScore;
        private UserConversationStateStore _userConversationStateStore;
        private CardsFactory _cardsFactory;

        public GameScoreTracker(IConfiguration config,
            GameScore gameScore,
            UserConversationStateStore userConversationStateStore,
            CardsFactory cardsFactory)
        {
            _appId = config["MicrosoftAppId"];
            _appPassword = config["MicrosoftAppPassword"];
            _gameScore = gameScore;
            _userConversationStateStore = userConversationStateStore;
            _cardsFactory = cardsFactory;
        }

        public async Task UpdatePlayerScore(ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var obj = (JObject)turnContext.Activity.Value;

            //record the choice
            _gameScore.AddScore(new PlayerChoice()
            {
                PlayerName = obj["user"].ToString(),
                Choice = obj["choice"].ToString()
            });

            //Update the Game card with Thank You Card
            var card = _cardsFactory.CreateThankYouCardAttachment(obj["user"].ToString());
            var thankYouForPlayingCard = MessageFactory.Attachment(card);
            await UpdateMessageMemberAsync(turnContext, turnContext.Activity.From.Id, thankYouForPlayingCard, cancellationToken)
                .ConfigureAwait(false);

        }

        public async Task PostGameScoreInChannel(ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Attachment(
                _cardsFactory.CreateScoreCardAttachment(_gameScore.GetPlayerScores())), 
                cancellationToken).ConfigureAwait(false);
        }

        private async Task UpdateMessageMemberAsync(ITurnContext turnContext,
            string teamMemberId,
            IMessageActivity messageActivity,
            CancellationToken cancellationToken)
        {
            var userConversationState = _userConversationStateStore.GetConversationReference(teamMemberId);
            await ((BotFrameworkAdapter)turnContext.Adapter).ContinueConversationAsync(
                _appId,
                userConversationState.Conversation,
                async (t2, c2) =>
                {
                    messageActivity.Id = userConversationState.ActivityId;
                    await t2.UpdateActivityAsync(messageActivity, c2).ConfigureAwait(false);
                },
                cancellationToken).ConfigureAwait(false);
        }

    }
}
