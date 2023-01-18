using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using RockPaperScissorGameBot.Services;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Bot.Connector.Authentication;

namespace RockPaperScissorGameBot.Bots
{
    public class GameBot: TeamsActivityHandler
    {
        private GameStarterService _gameStarter;
        private GameScoreTrackerService _gameScoreTracker;

        public GameBot(GameStarterService gameStarter, GameScoreTrackerService gameScoreTracker)
        {
            _gameStarter = gameStarter;
            _gameScoreTracker = gameScoreTracker;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, 
            CancellationToken cancellationToken)
        {
            turnContext.Activity.RemoveRecipientMention();
            var commandName = turnContext.Activity.Text?.Trim();
            
            //User wants to start a new game
            switch (commandName)
            {
                case "Start":
                    {
                        await _gameStarter.StartNewGame(turnContext, cancellationToken).ConfigureAwait(false);
                        return;
                    }

                case "SendScore":
                    {
                        await _gameStarter.StartNewThread(turnContext, cancellationToken).ConfigureAwait(false);
                        return;
                    }
            }

            //User clicked on rock/paper/scissor
            if (PlayerSubmittedChoice(commandName))
            {
                //async call
                _ = _gameScoreTracker.UpdatePlayerScore(turnContext, cancellationToken);
                return;
            }


            //Unknown Command
            await turnContext.SendActivityAsync($"Well {turnContext.Activity.From.Name}, I don't understand {commandName}. " +
                $"I understand: StartGame").ConfigureAwait(false);
        }

        private bool PlayerSubmittedChoice(string str)
        {
            str = str?.ToUpperInvariant();
            return (str == "ROCK" || str == "PAPER" || str == "SCISSOR");
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionFetchTaskAsync(
            ITurnContext<IInvokeActivity> turnContext, 
            MessagingExtensionAction action, 
            CancellationToken cancellationToken)
        {
            
            var type = turnContext.Activity.Conversation.ConversationType;
            System.Diagnostics.Debug.WriteLine(type);

            var gameCard = new HeroCard(title: "MyGame").ToAttachment();

            return new MessagingExtensionActionResponse
            {
                Task = this.TaskModuleReportCardTask(turnContext, gameCard),
            };
        }

        /// <summary>
        /// Called asynchronous when request is a messaging extension action for submit action.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <param name="action">action</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(
            ITurnContext<IInvokeActivity> turnContext, 
            MessagingExtensionAction action, 
            CancellationToken cancellationToken)
        {
            var type = turnContext.Activity.Conversation.ConversationType;
            System.Diagnostics.Debug.WriteLine(type);

            var gameCard = new HeroCard(title: "MyGame").ToAttachment();

            return new MessagingExtensionActionResponse
            {
                Task = this.TaskModuleReportCardTask(turnContext, gameCard),
            };
        }

        private TaskModuleResponseBase TaskModuleReportCardTask(ITurnContext<IInvokeActivity> turnContext, Attachment card)
        {

            return new TaskModuleContinueResponse()
            {
                Type = "continue",
                Value = new TaskModuleTaskInfo()
                {
                    Title = "Starting the game",
                    Card = card,
                },
            };
        }


    }
}
