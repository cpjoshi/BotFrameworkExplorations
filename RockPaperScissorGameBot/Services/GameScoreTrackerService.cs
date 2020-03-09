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

namespace RockPaperScissorGameBot.Services
{
    public class GameScoreTrackerService
    {
        private string _appId;
        private string _appPassword;
        private GameFactory _gameFactory;
        private UserConversationStateCollection _userConversationStateStore;
        private CardsFactory _cardsFactory;

        public GameScoreTrackerService(IConfiguration config,
            GameFactory gameFactory,
            UserConversationStateCollection userConversationStateStore,
            CardsFactory cardsFactory)
        {
            _appId = config["MicrosoftAppId"];
            _appPassword = config["MicrosoftAppPassword"];
            _gameFactory = gameFactory;
            _userConversationStateStore = userConversationStateStore;
            _cardsFactory = cardsFactory;
        }

        public async Task UpdatePlayerScore(ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var obj = (JObject)turnContext.Activity.Value;
            string gameId = obj["GameId"].ToString();

            var game = _gameFactory.GetGame(gameId);
            //record the players choice
            game.RecordPlayersChoice(new PlayerChoice()
            {
                PlayerName = obj["user"].ToString(),
                Choice = obj["choice"].ToString()
            });

            //Update the Game card with Thank You Card
            await SendThankyouForPlayingCard(turnContext, obj, cancellationToken).ConfigureAwait(false);

            //Game over, all players done playing, send the score card to all of them
            if(game.IsGameOver()) {
                await SendScoreCardToAllPlayers(gameId, turnContext, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task SendThankyouForPlayingCard(ITurnContext<IMessageActivity> turnContext, 
            JObject obj, 
            CancellationToken cancellationToken)
        {
            var card = _cardsFactory.CreateThankYouCardAttachment(obj["user"].ToString());
            var thankYouForPlayingCard = MessageFactory.Attachment(card);
            await UpdateMessageMemberAsync(turnContext, turnContext.Activity.From.Id, thankYouForPlayingCard, cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task SendScoreCardToAllPlayers(string gameId, 
            ITurnContext<IMessageActivity> turnContext, 
            CancellationToken cancellationToken)
        {
            var game = _gameFactory.GetGame(gameId);
            var card = _cardsFactory.CreateScoreCardAttachment(game.GetAllPlayerScores());
            var scoreCard = MessageFactory.Attachment(card);
            foreach(var user in _userConversationStateStore)
            {
                await UpdateMessageMemberAsync(turnContext, user, scoreCard, cancellationToken)
                    .ConfigureAwait(false);
            }
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
