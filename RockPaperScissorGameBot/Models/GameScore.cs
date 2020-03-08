using Microsoft.Recognizers.Text.DateTime.Japanese;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RockPaperScissorGameBot.Models
{
    public class PlayerChoice
    {
        public string PlayerName { get; set; }
        public string Choice { get; set; }
    }
    public class GameScore
    {
        private ConcurrentDictionary<string, Dictionary<string, PlayerChoice>> _games = 
            new ConcurrentDictionary<string, Dictionary<string, PlayerChoice>>();
        public void AddNewGame(string gameId)
        {
            //Initilize a new game
            _games.AddOrUpdate(gameId, 
                new Dictionary<string, PlayerChoice>(), 
                (key, oldValue) => new Dictionary<string, PlayerChoice>());
        }

        public void AddNewPlayer(string gameId, string playerName)
        {
            _games[gameId].Add(playerName, null);
        }
        public void RecordPlayersChoice(string gameId, PlayerChoice playerChoice)
        {
            if (!_games[gameId].ContainsKey(playerChoice.PlayerName))
                return;

            //this player has already recorded his choice, don't record it again
            if (_games[gameId][playerChoice.PlayerName] != null)
                return;

            _games[gameId][playerChoice.PlayerName] = playerChoice;
        }

        public bool isGameOver(string gameId)
        {
            foreach(var player in _games[gameId].Keys)
            {
                //Some player has not recorded his choice yet, game not over yet
                if (_games[gameId][player] == null)
                    return false;
            }
            return true;
        }
        public Dictionary<string, int> GetAllPlayerScores(string gameId)
        {
            var gameScoreForAllPlayers = new Dictionary<string, int>();
            PlayerChoice[] arr = _games[gameId].Values.ToArray();

            //initialize all player scores with 0
            for (int i = 0; i < arr.Length; i++)
            {
                gameScoreForAllPlayers.Add(arr[i].PlayerName, 0);
            }

            for (int i=0; i<arr.Length; i++)
            {
                for(int j=i+1; j<arr.Length; j++)
                {
                    PlayerChoice tmp = Whowon(arr[i], arr[j]);
                    if(tmp == null)
                    {
                        continue;
                    }
                    gameScoreForAllPlayers[tmp.PlayerName]++;
                }
            }
            return gameScoreForAllPlayers;
        }

        private PlayerChoice Whowon(PlayerChoice one, PlayerChoice two)
        {
            if (one.Choice == two.Choice)
            {
                //we have a draw
                return null;
            }

            switch (one.Choice)
            {
                case "rock":
                    {
                        return two.Choice == "scissor" ? one : two;
                    }
                case "paper":
                    {
                        return two.Choice == "rock" ? one : two;
                    }
                case "scissor":
                    {
                        return two.Choice == "paper" ? one : two;
                    }
                default:
                    return null;
            }
        }

    }
}
