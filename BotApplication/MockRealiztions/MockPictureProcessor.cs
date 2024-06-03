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
        public PassportData ProcessPassportPicture(object someData)
        {
            return new PassportData
            {
                Name = "Name",
                Id = "ID"
            };
        }

        public VenicleIdData ProcessVenichleIdPicture(object someData)
        {
            return new VenicleIdData
            {
                Name = "Name",
                Id = "ID"
            };
        }
    }
}
