namespace RockPaperScissorGameBot.Models
{
    public class PlayerChoice
    {
        public string PlayerName { get; set; }
        /// <summary>
        /// One of the following strings: rock, paper, scissor
        /// </summary>
        public string Choice { get; set; }
    }
}
