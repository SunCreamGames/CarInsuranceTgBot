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
        Task Init();
        Task<string> Greet();
        Task<string> AskForPassport();
        Task<string> AskForPassportApprove(PassportData data);
        Task<string> AskForWaitWhileProcessing();
        Task<string> AskForPassportAgain();
        Task<string> AskForVenichleId();
        Task<string> AskForVenichleIdApprove(VeniclePlateData data);
        Task<string> AskForVenichleIdAgain();
        Task<string> PriceAnnouncement(int price);
        Task<string> RejectingPriceReaction(int price);
        Task<string> DocumentCoverText();
        Task<string> IncorrectInputHandle();
    }
}
