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

        string systemMsg = "You are chat-bot assistant for online creating car insurance policy. \r\nYou get instructions what to do on every request. Your purpose is to guide me though pipeline friendly. Each step you will have instuctions how to do that. Next request should be responded as you are responding to me as customer. So it gives instructions, how to communicate. Remember : You can't ask client questions like \"How can i help you?\" and other, just giving instructions and requests as its final version customer will get";
        public async Task Init()
        {


            var completionResult = await openAiService.ChatCompletion.CreateCompletion(
                new ChatCompletionCreateRequest()
                {
                    Model = Gpt_4o,
                    Messages = new List<ChatMessage>() { new ChatMessage("system", systemMsg) }
                },
                modelId: "gpt-4o");
        }

        public async Task<string> Greet()
        {
            var msg = "Greet the client and descripe purpose of chat-bot.Remember : You can't ask client questions like \"How can i help you?\" and other, just giving instructions and requests";

            var completionResult = await openAiService.ChatCompletion.CreateCompletion(
                new ChatCompletionCreateRequest()
                {
                    Model = Gpt_4o,
                    Messages = new List<ChatMessage>() { new ChatMessage("user", msg) }
                },
                modelId: "gpt-4o");

            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Message.Content;
        }
        public async Task<string> AskForPassport()
        {
            var msg = "Request for passport photo with good illumination and all fields visible";

            var completionResult = await openAiService.ChatCompletion.CreateCompletion(
               new ChatCompletionCreateRequest()
               {
                   Model = Gpt_4o,
                   Messages = new List<ChatMessage>() { new ChatMessage("system", systemMsg), new ChatMessage("user", msg) }
               },
               modelId: "gpt-4o");

            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Message.Content;
        }
        public async Task<string> AskForWaitWhileProcessing()
        {
            var msg = "He uploaded the photo. Ask to wait while photo is processing and data is extracting.";


            var completionResult = await openAiService.ChatCompletion.CreateCompletion(
               new ChatCompletionCreateRequest()
               {
                   Model = Gpt_4o,
                   Messages = new List<ChatMessage>() { new ChatMessage("system", systemMsg), new ChatMessage("user", msg) }
               },
               modelId: "gpt-4o");


            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Message.Content;
        }
        public async Task<string> AskForPassportApprove(PassportData data)
        {
            var msg = $"Ask if his data is right? \\r\\n\"Name\" : \"${data.Name}\"\r\n\"Id\" : \"{data.Id}\"\r\n";


            var completionResult = await openAiService.ChatCompletion.CreateCompletion(
                new ChatCompletionCreateRequest()
                {
                    Model = Gpt_4o,
                    Messages = new List<ChatMessage>() { new ChatMessage("system", systemMsg), new ChatMessage("user", msg) }
                },
                modelId: "gpt-4o");



            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Message.Content;
        }
        public async Task<string> AskForPassportAgain()
        {

            var msg = "Data extracted from photo wasn't correct. Ask to re-upload another photo of passport. Accent on visibility and light";


            var completionResult = await openAiService.ChatCompletion.CreateCompletion(
                new ChatCompletionCreateRequest()
                {
                    Model = Gpt_4o,
                    Messages = new List<ChatMessage>() { new ChatMessage("system", systemMsg), new ChatMessage("user", msg) }
                },
                modelId: "gpt-4o");

            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Message.Content;
        }



        public async Task<string> AskForVenichleId()
        {

            var msg = "Ask client for license plate photo with good illumination and all fields visible";

            var completionResult = await openAiService.ChatCompletion.CreateCompletion(
new ChatCompletionCreateRequest()
{
    Model = Gpt_4o,
    Messages = new List<ChatMessage>() { new ChatMessage("system", systemMsg), new ChatMessage("user", msg) }
},
modelId: "gpt-4o");

            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Message.Content;
        }

        public async Task<string> AskForVenichleIdAgain()
        {

            var msg = "Data extracted from photo wasn't correct. Ask to re-upload another photo of lisence plate. Accent on its visibility and light";
            var completionResult = await openAiService.ChatCompletion.CreateCompletion(
                new ChatCompletionCreateRequest()
                {
                    Model = Gpt_4o,
                    Messages = new List<ChatMessage>() { new ChatMessage("system", systemMsg), new ChatMessage("user", msg) }
                },
                modelId: "gpt-4o");


            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Message.Content;
        }

        public async Task<string> AskForVenichleIdApprove(VeniclePlateData data)
        {
            var msg = $"Ask if his data is right? \r\n\"Id\" : \"{data.Id}\"\r\n";

            var completionResult = await openAiService.ChatCompletion.CreateCompletion(
               new ChatCompletionCreateRequest()
               {
                   Model = Gpt_4o,
                   Messages = new List<ChatMessage>() { new ChatMessage("system", systemMsg), new ChatMessage("user", msg) }
               },
               modelId: "gpt-4o");


            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Message.Content;
        }


        public async Task<string> IncorrectInputHandle()
        {

            var msg = "User made incorrect input. friendly repeat your request.";

            var completionResult = await openAiService.ChatCompletion.CreateCompletion(
              new ChatCompletionCreateRequest()
              {
                  Model = Gpt_4o,
                  Messages = new List<ChatMessage>() { new ChatMessage("system", systemMsg), new ChatMessage("user", msg) }
              },
              modelId: "gpt-4o");



            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Message.Content;
        }

        public async Task<string> PriceAnnouncement(int price)
        {
            var msg = $"Announce, that creation of car insurance policy costs {price} usd.Ask if client agreee to that, if yes, ask to proceed payment with \"yes\" button, if no, ask to reject with \"no\"";


            var completionResult = await openAiService.ChatCompletion.CreateCompletion(
              new ChatCompletionCreateRequest()
              {
                  Model = Gpt_4o,
                  Messages = new List<ChatMessage>() { new ChatMessage("system", systemMsg), new ChatMessage("user", msg) }
              },
              modelId: "gpt-4o");



            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Message.Content;

        }

        public async Task<string> RejectingPriceReaction(int price)
        {
            var msg = $"User rejected the price. Say sorry and explain, that ${price} is the only price we have. Ask him to come back if he will change his mind";

            var completionResult = await openAiService.ChatCompletion.CreateCompletion(
              new ChatCompletionCreateRequest()
              {
                  Model = Gpt_4o,
                  Messages = new List<ChatMessage>() { new ChatMessage("system", systemMsg), new ChatMessage("user", msg) }
              },
              modelId: "gpt-4o");


            if (!completionResult.Successful)
                return $"Seems we have some problem with our conversational API. Try to contact developers or make attepmt later. Error : {completionResult.Error.Message}";

            return completionResult.Choices.First().Message.Content;
        }
    }
}
