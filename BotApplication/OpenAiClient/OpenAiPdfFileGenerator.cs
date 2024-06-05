using Domain.Contracts;
using Domain.Data;
using OpenAI.Managers;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI.ObjectModels.RequestModels;

namespace OpenAiClient
{
    public class OpenAiPdfFileGenerator : IPolicyGenerator
    {
        public async Task<byte[]> CreateNewPolicy(PassportData passportData, VeniclePlateData venicleIdData, int price)
        {
            OpenAIService openAiService = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = Environment.GetEnvironmentVariable("OpenAiApiKey")
            });


            var msg = $"Generate for me a pdf file. Use any font. This should be a file of car insurance policy. You can use fake data for address, vin, etc. The only data you should use are \"Name\":\"{passportData.Name}\", \"PassportId\":\"{passportData.Id}\",\"LicensePlateId\":\"{venicleIdData.Id}\", \"Price\" : ${price}";


            var completionResult = await openAiService.ChatCompletion.CreateCompletion(
                new ChatCompletionCreateRequest()
                {
                    Messages = new List<ChatMessage>() { new ChatMessage("user", msg) }
                },
                modelId: "gpt-4o");

            if (!completionResult.Successful)
                throw new Exception("Can't produce pdf file");

            return null;
            //return completionResult.Choices.FirstOrDefault().Message;
        }
    }
}
