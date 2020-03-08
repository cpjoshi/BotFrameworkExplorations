using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using System;
using System.Collections;
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

    public class UserConversationStateCollection: IEnumerable<string>
    {
        Dictionary<string, UserConversationState> dict = new Dictionary<string, UserConversationState>();
        public void AddConversationReference(TeamsChannelAccount teamMember, 
            ConversationReference conversationReference, 
            string activityId)
        {
            var userConversationState = new UserConversationState()
            {
                TeamMember = teamMember,
                Conversation = conversationReference,
                ActivityId = activityId
            };
            dict[teamMember.Id] = userConversationState;
        }

        public UserConversationState GetConversationReference(string userId)
        {
            return dict[userId];
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach(string userId in dict.Keys)
            {
                yield return userId;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
