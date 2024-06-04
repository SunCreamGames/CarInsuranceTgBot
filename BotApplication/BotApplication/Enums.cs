using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotApplication
{
    public enum ProcessStage
    {
        Start,
        WaitingForPassportPhoto,
        WaitingForPassportDataApprove,
        WaitingForVenichleIdPhoto,
        WaitingForVenichleIdDataApprove,
        WaitingForPriceConfirmation,
    }
    public enum InlineReplies
    {
        Confirm,
        Unconfirm,
    }

}
