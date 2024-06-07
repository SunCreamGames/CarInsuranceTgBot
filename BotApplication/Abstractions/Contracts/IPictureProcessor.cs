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
        /// <summary>
        /// Parses given picture file and returns data from it
        /// </summary>
        /// <param name="fileData"> picture in .jpg format as byte array </param>
        /// <returns> PassportData filled with parsed from picture values </returns>
        Task<PassportData> ProcessPassportPicture(byte[] fileData);

        /// <summary>
        /// Parses given picture file and returns data from it
        /// </summary>
        /// <param name="fileData"> picture in .jpg format as byte array </param>
        /// <returns> LicensePlateData filled with parsed from picture values </returns>
        Task<LicensePlateData> ProcessLicensePlatePicture(byte[] fileData);
    }
}
