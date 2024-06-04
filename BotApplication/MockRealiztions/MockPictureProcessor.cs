using Domain.Contracts;
using Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockRealiztions
{
    public class MockPictureProcessor : IPictureProcessor
    {
        public Task<PassportData> ProcessPassportPicture(byte[] someData)
        {
            return Task.Run(() => new PassportData
            {
                Name = "Name",
                Id = "ID"
            });
        }

        public Task<VenicleIdData> ProcessVenichleIdPicture(byte[] someData)
        {
            return Task.Run(() => new VenicleIdData
            {
                Name = "Name",
                Id = "ID"
            });
        }
    }
}
