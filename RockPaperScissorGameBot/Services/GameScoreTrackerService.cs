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
        private CardsFactory _cardsFactory;

        public GameScoreTrackerService(IConfiguration config,
            GameFactory gameFactory,
            CardsFactory cardsFactory)
        {
            _appId = config["MicrosoftAppId"];
            _appPassword = config["MicrosoftAppPassword"];
            _gameFactory = gameFactory;
            _cardsFactory = cardsFactory;
        }

        public async Task UpdatePlayerScore(ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var obj = (JObject)turnContext.Activity.Value;
            string gameId = obj["GameId"].ToString();

            var game = _gameFactory.GetGame(gameId);
            //record the players choice
            game.RecordPlayersChoice(
                playerName: obj["user"].ToString(), 
                playerChoice: obj["choice"].ToString()
                );

            //Update the Game card with Thank You Card
            await SendThankyouForPlayingCardToPlayer(turnContext, 
                game.GetPlayer(obj["user"].ToString()), cancellationToken).ConfigureAwait(false);

            //Game over, all players done playing, send the score card to all of them
            if(game.IsGameOver()) {
                await SendScoreCardToAllPlayers(gameId, turnContext, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task SendThankyouForPlayingCardToPlayer(ITurnContext<IMessageActivity> turnContext, 
            Player player, 
            CancellationToken cancellationToken)
        {
            var card = _cardsFactory.CreateThankYouCardAttachment(player.PlayerName);
            var thankYouForPlayingCard = MessageFactory.Attachment(card);
            await UpdateMessageMemberAsync(turnContext, player.userConversationState, thankYouForPlayingCard, cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task SendScoreCardToAllPlayers(string gameId, 
            ITurnContext<IMessageActivity> turnContext, 
            CancellationToken cancellationToken)
        {
            var game = _gameFactory.GetGame(gameId);
            var card = _cardsFactory.CreateScoreCardAttachment(game.GetAllPlayerScores());
            var scoreCard = MessageFactory.Attachment(card);
            foreach(var player in game)
            {
                await UpdateMessageMemberAsync(turnContext, player.userConversationState, scoreCard, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private async Task UpdateMessageMemberAsync(ITurnContext turnContext,
            UserConversationState userConversationState,
            IMessageActivity messageActivity,
            CancellationToken cancellationToken)
        {
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
