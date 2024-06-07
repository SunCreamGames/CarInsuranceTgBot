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

        /// <summary>
        /// Initializes html-template with Open ai for policy document
        /// </summary>
        Task InitTemplate();


        /// <summary>
        /// Fills html-template with provided values and convert it to pdf document
        /// </summary>
        /// <returns>pdf  file as byte array </returns>
        Task<byte[]> CreateNewPolicy(PassportData passportData, LicensePlateData venicleIdData, int price);
    }
}
