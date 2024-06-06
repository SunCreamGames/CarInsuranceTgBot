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
                Name = "Vegabréf",
                Id = "1234567"
            });
        }

        public Task<VeniclePlateData> ProcessVenichleIdPicture(byte[] someData)
        {
            return Task.Run(() => new VeniclePlateData
            {
                Id = "88448844"
            });
        }
    }
}
