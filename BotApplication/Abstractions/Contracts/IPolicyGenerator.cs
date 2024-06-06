using Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IPolicyGenerator
    {
        Task InitTemplate();
        Task<byte[]> CreateNewPolicy(PassportData passportData, VeniclePlateData venicleIdData, int price);
    }
}
