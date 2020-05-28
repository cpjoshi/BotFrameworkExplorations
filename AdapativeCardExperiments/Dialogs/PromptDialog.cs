using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace AdapativeCardExperiments.Dialogs
{
    public class PromptDialog: ComponentDialog
    {
        public PromptDialog(): base(nameof(PromptDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                CollectInput,
                FinalStep,
            }));

            AddDialog(new ChoicePrompt("input"));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> CollectInput(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var prompt = MessageFactory.Text("Did you mean following?");
            var newChoices = new List<Choice>();
            newChoices.Add(new Choice() { Action = new CardAction() { Title = "Alternative 1", Type = "imBack", Value = "Alternative 1" }, Value = "value1" });
            newChoices.Add(new Choice() { Action = new CardAction() { Title = "Alternative 2", Type = "imBack", Value = "Alternative 2" }, Value = "value2" });
            return await stepContext.PromptAsync("input", new PromptOptions { Prompt = prompt, Choices = newChoices, Style = ListStyle.HeroCard });
        }
        private async Task<DialogTurnResult> FinalStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is FoundChoice choice)
            {
                await stepContext.Context.SendActivityAsync(choice.Value);
            }
            return await stepContext.EndDialogAsync();
        }

    }
}
