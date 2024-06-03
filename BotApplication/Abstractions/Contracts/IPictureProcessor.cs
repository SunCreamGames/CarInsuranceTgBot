using Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IPictureProcessor
    {
        PassportData ProcessPassportPicture(object someData);
        VenicleIdData ProcessVenichleIdPicture(object someData);
    }
}
