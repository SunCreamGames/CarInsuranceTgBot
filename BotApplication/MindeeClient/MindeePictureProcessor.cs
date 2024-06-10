
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
        string apiKey; // Actually we can use only one api-key.
                       // but free mindee account give us only 1-type of document,
                       // so I used 2 mindee free accounts there
        public async Task<PassportData> ProcessPassportPicture(byte[] fileData)
        {

            apiKey = Environment.GetEnvironmentVariable("MindeeApiKey1");


            MindeeClient mindeeClient = new MindeeClient(apiKey);

            var input = new LocalInputSource(fileData, "passport_photo.jpg");

            try
            {
                var response = await mindeeClient
             .EnqueueAndParseAsync<InternationalIdV1>(input);

                if (response.ApiRequest.StatusCode > 400)
                    throw new PictureProcessException("Seems like we have some issues with our picture processing API. Try again later or contact our developers, please");

                if (response.Document.Inference.Prediction.GivenNames.Count == 0 || response.Document.Inference.Prediction.GivenNames.All(x => string.IsNullOrEmpty(x.Value)))
                    throw new PictureProcessException("Couldn't recognize any name field");

                var name = string.Join(' ', response.Document.Inference.Prediction.GivenNames.Select(x => x.Value));

                if (string.IsNullOrEmpty(response.Document.Inference.Prediction.DocumentNumber.Value))
                    throw new PictureProcessException("Couldn't recognize any Id field");

                var id = response.Document.Inference.Prediction.DocumentNumber.Value;
                return new PassportData { Name = name, Id = id };
            }
            catch
            {
                throw new PictureProcessException("Seems like we have some issues with our picture processing API. Try again later or contact our developers, please");
            }
        }

        public async Task<LicensePlateData> ProcessLicensePlatePicture(byte[] fileData)
        {
            apiKey = Environment.GetEnvironmentVariable("MindeeApiKey2");

            MindeeClient mindeeClient = new MindeeClient(apiKey);

            var input = new LocalInputSource(fileData, "plate_photo.jpg");

            try
            {
                var response = await mindeeClient
                    .ParseAsync<LicensePlateV1>(input);

                if (response.ApiRequest.StatusCode > 400)
                    throw new PictureProcessException("Seems like we have some issues with our picture processing API. Try again later or contact our developers, please");

                if (response.Document.Inference.Prediction.LicensePlates.Count == 0 || string.IsNullOrEmpty(response.Document.Inference.Prediction.LicensePlates.FirstOrDefault().Value))
                    throw new PictureProcessException("Couldn't recognize any Id field");

                var id = response.Document.Inference.Prediction.LicensePlates.FirstOrDefault().Value;

                return new LicensePlateData { Id = id };
            }
            catch
            {
                throw new PictureProcessException("Seems like we have some issues with our picture processing API. Try again later or contact our developers, please");
            }
        }
    }
}