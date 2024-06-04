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
        Task<PassportData> ProcessPassportPicture(byte[] fileData);
        Task<VenicleIdData> ProcessVenichleIdPicture(byte[] fileData);
    }
}
