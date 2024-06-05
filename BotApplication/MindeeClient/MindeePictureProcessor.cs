
using Domain.Contracts;
using Domain.Data;
using Mindee;
using Mindee.Input;
using Mindee.Product.Eu.LicensePlate;
using Mindee.Product.InternationalId;
using Mindee.Product.Us.DriverLicense;

namespace MindeePictureProcessing
{
    public class MindeePictureProcessor : IPictureProcessor
    {

        public async Task<PassportData> ProcessPassportPicture(byte[] fileData)
        {

            string apiKey = Environment.GetEnvironmentVariable("MindeeApiKey1");
            
            
            MindeeClient mindeeClient = new MindeeClient(apiKey);

            var input = new LocalInputSource(fileData, "plate_photo.jpg");

            var response = await mindeeClient
     .EnqueueAndParseAsync<InternationalIdV2>(input);


            var name = response.Document.Inference.Prediction.GivenNames.FirstOrDefault().Value;
            var id = response.Document.Inference.Prediction.DocumentNumber.Value;
            return new PassportData { Name = name, Id = id };
        }

        public async Task<VenicleIdData> ProcessVenichleIdPicture(byte[] fileData)
        {
            string apiKey = Environment.GetEnvironmentVariable("MindeeApiKey2");

            MindeeClient mindeeClient = new MindeeClient(apiKey);

            var input = new LocalInputSource(fileData, "plate_photo.jpg");

            var response = await mindeeClient
                .ParseAsync<LicensePlateV1>(input);

            var id = response.Document.Inference.Prediction.LicensePlates.FirstOrDefault().Value;

            return new VenicleIdData { Id = id };
        }
    }
