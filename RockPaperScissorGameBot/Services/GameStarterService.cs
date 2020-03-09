using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using RockPaperScissorGameBot.Cards;
using RockPaperScissorGameBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RockPaperScissorGameBot.Services
{
    public class GameStarterService
    {
        private const string InvitationSent = "Game invitation is sent to all members.";
        private string _appId;
        private string _appPassword;
        private GameFactory _gameFactory;
        private UserConversationStateCollection _userConversationStateStore;
        private CardsFactory _cardsFactory;

        public GameStarterService(IConfiguration config,
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


        public async Task StartNewGame(ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var game = _gameFactory.CreateNewGame();

            var members = await TeamsInfo.GetMembersAsync(turnContext, cancellationToken).ConfigureAwait(false);
            foreach (var member in members)
            {
                if (member.Id == turnContext.Activity.Recipient.Id)
                {
                    continue;
                }

                game.AddNewPlayer(member.Name);
                var gameCard = _cardsFactory.CreateGameCardAttachment(member.Name, game.GameId);
                var activity = MessageFactory.Attachment(gameCard);

                await MessageMembersAsync(turnContext,
                    member,
                    activity,
                    cancellationToken).ConfigureAwait(false);
            }
            await turnContext.SendActivityAsync(MessageFactory.Text(InvitationSent),
                cancellationToken).ConfigureAwait(false);

        }

        private async Task MessageMembersAsync(ITurnContext turnContext,
            TeamsChannelAccount teamMember,
            IMessageActivity messageActivity,
            CancellationToken cancellationToken)
        {
            var teamsChannelId = turnContext.Activity.TeamsGetChannelId();
            var serviceUrl = turnContext.Activity.ServiceUrl;
            var credentials = new MicrosoftAppCredentials(_appId, _appPassword);
            ConversationReference conversationReference = null;
            string activityId = string.Empty;

            var conversationParameters = new ConversationParameters
            {
                IsGroup = false,
                Bot = turnContext.Activity.Recipient,
                Members = new ChannelAccount[] { teamMember },
                TenantId = turnContext.Activity.Conversation.TenantId,
            };

            await ((BotFrameworkAdapter)turnContext.Adapter).CreateConversationAsync(
                teamsChannelId,
                serviceUrl,
                credentials,
                conversationParameters,
                callback: async (turnContext1, cancellationToken1) =>
                {
                    conversationReference = turnContext1.Activity.GetConversationReference();

                    await ((BotFrameworkAdapter)turnContext.Adapter).ContinueConversationAsync(
                        _appId,
                        conversationReference,
                        async (t2, c2) =>
                        {
                            var response = await t2.SendActivityAsync(messageActivity, c2).ConfigureAwait(true);
                            activityId = response.Id;
                            _userConversationStateStore.AddConversationReference(teamMember, conversationReference, activityId);
                        },
                        cancellationToken).ConfigureAwait(true);

                },
                cancellationToken).ConfigureAwait(true);
        }

    }
}
