using Domain.Contracts;
using Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockRealiztions
{
    public class DummyPolicyGenerator : IPolicyGenerator
    {
        public Task InitTemplate()
        {
            return Task.CompletedTask;
        }

        Task<byte[]> IPolicyGenerator.CreateNewPolicy(PassportData passportData, VeniclePlateData venicleIdData, int price)
        {

            return Task.Run(() => File.ReadAllBytes("dummy.pdf"));
        }
    }
}
