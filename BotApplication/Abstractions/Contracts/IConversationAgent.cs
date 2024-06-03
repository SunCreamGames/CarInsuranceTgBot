using Domain.Data;
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
        string AskForPassportApprove(PassportData data);
        string AskForPassportAgain();
        string AskForVenichleId();
        string AskForVenichleIdApprove(VenicleIdData data);
        string AskForVenichleIdAgain();
        string PriceAnnouncement();
        string RejectingPriceReaction();
        string DocumentCoverText();
        string FarewellIncomplete();
        string FarewellFinish();
        string ErrorMessage();
    }
}
