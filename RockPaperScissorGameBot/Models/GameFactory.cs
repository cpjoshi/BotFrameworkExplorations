using Microsoft.Recognizers.Text.DateTime.Japanese;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RockPaperScissorGameBot.Models
{
    public class GameFactory
    {
        private ConcurrentDictionary<string, Game> _games = 
            new ConcurrentDictionary<string, Game>();
        public Game CreateNewGame()
        {
            //Initilize a new game
            var gameId = Guid.NewGuid().ToString();
            var gameInstance = new Game(gameId);
            
            _games.AddOrUpdate(gameId, gameInstance, 
                (key, oldValue) => gameInstance);

            return gameInstance;
        }

        public Game GetGame(string gameId)
        {
            return _games[gameId];
        }

    }
}
