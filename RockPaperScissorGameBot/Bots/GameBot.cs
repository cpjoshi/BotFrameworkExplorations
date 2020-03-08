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
using RockPaperScissorGameBot.Utils;
using System.Globalization;

namespace RockPaperScissorGameBot.Bots
{
    public class GameBot: TeamsActivityHandler
    {
        private GameStarter _gameStarter;
        private GameScoreTracker _gameScoreTracker;

        public GameBot(GameStarter gameStarter, GameScoreTracker gameScoreTracker)
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
