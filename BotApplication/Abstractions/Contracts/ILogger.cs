﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface ILogger
    {
        void LogMessage(string message);
        void LogError(string error);
    }
}
