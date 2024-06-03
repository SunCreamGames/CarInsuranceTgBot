using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;

namespace MockRealiztions
{
    public class MockConversationalAgent : IConversationAgent
    {
        public string AskForPassport()
        {
            return "Provide your passport photo, please";
        }

        public string AskForPassportAgain()
        {
            return "Provide your passport photo one more time, please";
        }

        public string AskForPassportApprove(object someData)
        {
            return "Check if your passport data is ok";
        }

        public string AskForVenichleId()
        {
            return "Provide your venichle id photo, please";
        }

        public string AskForVenichleIdAgain()
        {
            return "Provide your venichle id photo one more time, please";
        }

        public string AskForVenichleIdApprove(object someData)
        {
            return "Check if your venichle id data is ok";
        }

        public string DocumentCoverText()
        {
            return "Here is your insurance policy docunent.";
        }

        public string FarewellFinish()
        {
            return "We were happy to help you. Hope you are satisfied with our service. Have a nice day!";
        }
        public string FarewellIncomplete()
        {
            return "Ok then. You can always come back to create an insurance policy dociument in case you will change your ming. Have a nice day!";
        }

        public string Greet()
        {
            return "Hello, this is the bot to create a car insurance policy document, using your passport and venichle id document.";
        }

        public string PriceAnnouncement()
        {
            return "Creating car insurance policy document costs $100. If you agreed with this price, that click \'continue button\' to procced a payment and get your document. If not, click \'no\' button.";
        }

        public string RejectingPriceReaction()
        {
            return "We are really sorry to hear that. But $100 is the only available price to create an insurance document. In case you changed your ming click 'I've changed my mind' to procced a payment and get your document. If not, click 'no' button.";
        }
    }
}
