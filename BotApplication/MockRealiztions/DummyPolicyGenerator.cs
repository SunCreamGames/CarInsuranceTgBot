﻿using Domain.Contracts;
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
        Task<byte[]> IPolicyGenerator.CreateNewPolicy(PassportData passportData, VenicleIdData venicleIdData)
        {
            return Task.Run(() => File.ReadAllBytes("dummy.pdf"));
        }
    }
}