using OpenAI.Managers;
using OpenAI;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using Domain.Contracts;
using Domain.Data;
using static OpenAI.ObjectModels.Models;
using static OpenAI.ObjectModels.SharedModels.IOpenAiModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using System.Threading.Tasks;
using System;

namespace OpenAiClient
{
    public class OpenAiConversationalAgent : IConversationAgent
    {
        OpenAIService openAiService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = Environment.GetEnvironmentVariable("OpenAiApiKey")

        });

        public async Task Init()
        {
            var completionResult = await openAiService.ChatCompletion.CreateCompletion(
                new ChatCompletionCreateRequest()
                {
                    Model = Gpt_4o,
                    Messages = new List<ChatMessage>() { new ChatMessage("user", "\"Play the role of chat-bot assistant for online creating car insurance policy.\\nJust route client though pipeline friendly. Each step you will have instuctions how to do that. Do not ask client addiotional questions like \\\"How can i help you\\\" because he has only buttons-interface.\", ") }
                },
                modelId: "gpt-4o");
        }

        public async Task<string> Greet()
        {
            var msg = "Greet the client and descripe purpose of chat-bot";

            var completionResult = await openAiService.ChatCompletion.CreateCompletion(
                new ChatCompletionCreateRequest()
                {
                    Model = Gpt_4o,
                    Messages = new List<ChatMessage>() { new ChatMessage("user", msg) }
                },
                modelId: "gpt-4o");

            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Text;
        }
        public async Task<string> AskForPassport()
        {
            var completionResult = await openAiService.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = "Ask client for passport photo with good illumination and all fields visible",
                Model = Gpt_3_5_Turbo
            });


            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Text;
        }
        public async Task<string> AskForWaitWhileProcessing()
        {
            var completionResult = await openAiService.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = "He uploaded the photo. Ask to wait while photo is processing and data is extracting.",
                Model = Gpt_3_5_Turbo
            });


            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Text;
        }
        public async Task<string> AskForPassportApprove(PassportData data)
        {
            var completionResult = await openAiService.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = $"Ask if his data is right? \\r\\n\"Name\" : \"${data.Name}\"\r\n\"Id\" : \"{data.Id}\"\r\n",
                Model = Gpt_3_5_Turbo
            });



            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Text;
        }
        public async Task<string> AskForPassportAgain()
        {
            var completionResult = await openAiService.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = "He disapproved data. Ask to re-upload another photo of passport. Accent on visibility and light",
                Model = Gpt_3_5_Turbo
            });



            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Text;
        }



        public async Task<string> AskForVenichleId()
        {

            var completionResult = await openAiService.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = "Ask client for license plate photo with good illumination and all fields visible",
                Model = Gpt_3_5_Turbo
            });


            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Text;
        }

        public async Task<string> AskForVenichleIdAgain()
        {
            var completionResult = await openAiService.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = "He disapproved data. Ask to re-upload another photo of lisence plate. Accent on its visibility and light",
                Model = Gpt_3_5_Turbo
            });


            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Text;
        }

        public async Task<string> AskForVenichleIdApprove(VeniclePlateData data)
        {
            var completionResult = await openAiService.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = $"Ask if his data is right? \r\n\"Id\" : \"{data.Id}\"\r\n",
                Model = Gpt_3_5_Turbo
            });


            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Text;
        }

        public async Task<string> DocumentCoverText()
        {
            var completionResult = await openAiService.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = $"We created and sended pdf of his car insurance policy, write some cover text like \"here is your insurance\", \"happy to help you\", \"come back again\", etc.",
                Model = Gpt_3_5_Turbo
            });


            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Text;
        }


        public async Task<string> IncorrectInputHandle()
        {
            var completionResult = await openAiService.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = "User made incorrect input. friendly repeat your request.",
                Model = Models.Gpt_3_5_Turbo
            });


            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Text;
        }

        public async Task<string> PriceAnnouncement(int price)
        {
            var completionResult = await openAiService.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = $"Announce, that creation of car insurance policy costs {price} usd.Ask if client agreee to that, if yes, ask to proceed payment with \"yes\" button, if no, ask to reject with \"no\"",
                Model = Models.Gpt_3_5_Turbo
            });

            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Text;

        }

        public async Task<string> RejectingPriceReaction(int price)
        {
            var completionResult = await openAiService.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = $"User rejected the price. Say sorry and explain, that ${price} is the only price we have. Ask him to come back if he will change his mind",
                Model = Models.Gpt_3_5_Turbo
            });


            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Text;
        }
    }
}
