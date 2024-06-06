
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
     .EnqueueAndParseAsync<InternationalIdV1>(input);

            if (response.Document.Inference.Prediction.GivenNames.Count == 0 || response.Document.Inference.Prediction.GivenNames.All(x => string.IsNullOrEmpty(x.Value)))
                throw new PictureProcessException("Couldn't recognize any name field");

            var name = string.Join(' ', response.Document.Inference.Prediction.GivenNames.Select(x => x.Value));

            if (string.IsNullOrEmpty(response.Document.Inference.Prediction.DocumentNumber.Value))
                throw new PictureProcessException("Couldn't recognize any Id field");

            var id = response.Document.Inference.Prediction.DocumentNumber.Value;
            return new PassportData { Name = name, Id = id };
        }

        public async Task<VeniclePlateData> ProcessVenichleIdPicture(byte[] fileData)
        {
            string apiKey = Environment.GetEnvironmentVariable("MindeeApiKey2");

            MindeeClient mindeeClient = new MindeeClient(apiKey);

            var input = new LocalInputSource(fileData, "plate_photo.jpg");

            var response = await mindeeClient
                .ParseAsync<LicensePlateV1>(input);

            if (response.Document.Inference.Prediction.LicensePlates.Count == 0 || string.IsNullOrEmpty(response.Document.Inference.Prediction.LicensePlates.FirstOrDefault().Value))
                throw new PictureProcessException("Couldn't recognize any Id field");

            var id = response.Document.Inference.Prediction.LicensePlates.FirstOrDefault().Value;

            return new VeniclePlateData { Id = id };
        }
    }
}