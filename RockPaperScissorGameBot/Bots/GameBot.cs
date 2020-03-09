using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using RockPaperScissorGameBot.Services;

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
                case "StartGame":
                    {
                        await _gameStarter.StartNewGame(turnContext, cancellationToken).ConfigureAwait(false);
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
    }
}
