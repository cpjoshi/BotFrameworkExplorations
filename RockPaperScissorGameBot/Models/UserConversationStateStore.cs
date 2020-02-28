using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockPaperScissorGameBot.Models
{
    public class UserConversationState
    {
        public TeamsChannelAccount TeamMember { get; set; }
        public ConversationReference Conversation { get; set; }
        public string ActivityId { get; set; }
    }

    public class UserConversationStateStore
    {
        Dictionary<string, UserConversationState> dict = new Dictionary<string, UserConversationState>();
        public void SaveConversationReference(TeamsChannelAccount teamMember, 
            ConversationReference conversationReference, 
            string activityId)
        {
            dict.Add(teamMember.Id, new UserConversationState()
            {
                TeamMember = teamMember,
                Conversation = conversationReference,
                ActivityId = activityId
            });
        }

        public UserConversationState GetConversationReference(string userId)
        {
            return dict[userId];
        }
    }
}
