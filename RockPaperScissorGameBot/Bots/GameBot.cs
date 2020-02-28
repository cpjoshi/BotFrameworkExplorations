using RockPaperScissorGameBot.Cards;
using RockPaperScissorGameBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RockPaperScissorGameBot.Bots
{
    public class GameBot: TeamsActivityHandler
    {
        private string _appId;
        private string _appPassword;
        private GameScore _gameScore;
        private UserConversationStateStore _userConversationStateStore;
        private CardsFactory _cardsFactory;

        public GameBot(IConfiguration config, 
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

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, 
            CancellationToken cancellationToken)
        {
            turnContext.Activity.RemoveRecipientMention();
            var txt = turnContext.Activity.Text;

            //response received from the adaptive card.
            if (IsButtonClick(txt) && turnContext.Activity.Value != null)
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
                await UpdateMessageMemberAsync(turnContext, turnContext.Activity.From.Id, thankYouForPlayingCard, cancellationToken);
                return;
            }

            var commandName = turnContext.Activity.Text.Trim();
            switch (commandName)
            {
                case "StartGame":
                    {
                        var members = await TeamsInfo.GetMembersAsync(turnContext, cancellationToken);
                        foreach(var member in members)
                        {
                            if (member.Id == turnContext.Activity.Recipient.Id)
                            {
                                continue;
                            }

                            string gameId = Guid.NewGuid().ToString();
                            var gameCard = _cardsFactory.CreateGameCardAttachment(member.Name, gameId);
                            var activity = MessageFactory.Attachment(gameCard);
                            
                            await MessageMembersAsync(turnContext, 
                                member, 
                                activity, 
                                cancellationToken);
                        }
                        await turnContext.SendActivityAsync(MessageFactory.Text("Game invitation is sent to all members."), 
                            cancellationToken);
                    }
                    break;

                case "ShowScore":
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Attachment(
                            _cardsFactory.CreateScoreCardAttachment(_gameScore.GetPlayerScores())));
                    }
                    break;

                default:
                    await turnContext.SendActivityAsync($"Well {turnContext.Activity.From.Name}, I don't understand {commandName}");
                    break;
            }
        }

        private bool IsButtonClick(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            str = str.ToLower();
            return (str == "rock" || str == "paper" || str == "scissor");
        }

        private async Task MessageMembersAsync (ITurnContext turnContext,
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

            await ((BotFrameworkAdapter) turnContext.Adapter).CreateConversationAsync(
                teamsChannelId,
                serviceUrl,
                credentials,
                conversationParameters,
                callback: async (turnContext1, cancellationToken1) =>
                {
                    conversationReference = turnContext1.Activity.GetConversationReference();

                    await ((BotFrameworkAdapter) turnContext.Adapter).ContinueConversationAsync(
                        _appId,
                        conversationReference,
                        async (t2, c2) =>
                        {
                            var response = await t2.SendActivityAsync(messageActivity, c2);
                            activityId = response.Id;
                            _userConversationStateStore.SaveConversationReference(teamMember, conversationReference, activityId);
                        },
                        cancellationToken);

                },
                cancellationToken);
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
                    await t2.UpdateActivityAsync(messageActivity, c2);
                },
                cancellationToken);
        }
    }
}
