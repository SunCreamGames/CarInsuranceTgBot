using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IConversationAgent
    {
        string Greet();
        string AskForPassport();
        string AskForPassportApprove(object someData);
        string AskForPassportAgain();
        string AskForVenichleId();
        string AskForVenichleIdApprove(object someData);
        string AskForVenichleIdAgain();

        string PriceAnnouncement();
        string RejectingPriceReaction();
        string DocumentCoverText();
        string FarewellIncomplete();
        string FarewellFinish();
    }
}
