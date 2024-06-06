using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Data;

namespace MockRealiztions
{
    public class MockConversationalAgent : IConversationAgent
    {
        public Task<string> AskForPassport()
        {
            return Task.Run(() => "Provide your passport photo, please");
        }

        public Task<string> AskForPassportAgain()
        {
            return Task.Run(() => "It is hard to extract data from provided photo. Send another passport photo, please. And make sure it is good.");
        }

        public Task<string> AskForPassportApprove(PassportData data)
        {
            return Task.Run(() => $"Check if your passport data is ok. Name is {data.Name}. Id is {data.Id}");
        }

        public Task<string> AskForVenichleId()
        {
            return Task.Run(() => "Provide your venichle id photo, please");
        }

        public Task<string> AskForVenichleIdAgain()
        {
            return Task.Run(() => "Provide your venichle id photo one more time, please");
        }

        public Task<string> AskForVenichleIdApprove(VeniclePlateData data)
        {
            return Task.Run(() => $"Check if your venichle id data is ok. Id is {data.Id}");
        }

      
        public Task<string> Greet()
        {
            return Task.Run(() => "Hello, this is the bot to create a car insurance policy document, using your passport and venichle id document.");
        }

        public Task<string> PriceAnnouncement(int price)
        {
            return Task.Run(() => $"Creating car insurance policy document costs ${price}. If you agreed with this price, that click \'continue button\' to procced a payment and get your document. If not, click \'no\' button.");
        }

        public Task<string> RejectingPriceReaction(int price)
        {
            return Task.Run(() => $"We are really sorry to hear that. But ${price} is the only available price to create an insurance document. In case you changed your ming click 'I've changed my mind' to procced a payment and get your document. If not, click 'no' button.");
        }


        public Task<string> AskForWaitWhileProcessing()
        {
            return Task.Run(() => "Wait a minute, we are processing your photo");

        }


        public Task<string> IncorrectInputHandle()
        {
            return Task.Run(() => "Oops, something unexpected happened. Try again");
        }

        public Task Init()
        {
            return Task.CompletedTask;
        }
    }
}
