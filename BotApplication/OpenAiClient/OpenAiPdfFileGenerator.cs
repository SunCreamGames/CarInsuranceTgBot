using Domain.Contracts;
using Domain.Data;
using OpenAI.Managers;
using OpenAI;
using OpenAI.ObjectModels.RequestModels;
using IronPdf;

namespace OpenAiClient
{
    public class OpenAiPdfFileGenerator : IPolicyGenerator
    {
        public string HtmlTemplate { get; set; }
        OpenAIService openAiService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = Environment.GetEnvironmentVariable("OpenAiApiKey")
        });
        public async Task InitTemplate()
        {
            License.LicenseKey = Environment.GetEnvironmentVariable("IronPdfApiKey");

            var systemMsg = $"Return just html code. Replace all placeholders with random but realistic data";


            var msg = $"Generate a structure a fake car insurance policy document. Make it html, so i can easily replace fields like Name, PasspoerId, etc. The only data MUST be present necessarily are \"Name\" field, \"PassportId\" field, \"LicensePlateId\" field and \"Price\". Set all listed fields with placeHolders \"[FakeName]\", \"[FakePassportId]\", etc. with FakeFieldName pattern. Set all other fields with some generated realistic data.Send answer as html-code only, without cover text.";



            var completionResult = await openAiService.ChatCompletion.CreateCompletion(
                new ChatCompletionCreateRequest()
                {
                    Messages = new List<ChatMessage>() { new ChatMessage("system", systemMsg), new ChatMessage("user", msg) },
                    ResponseFormat = new ResponseFormat { Type = "text" }
                },
                modelId: "gpt-4o");

            if (!completionResult.Successful)
                throw new Exception("Can't produce file structure");

            HtmlTemplate = completionResult.Choices[0].Message.Content;
            HtmlTemplate = HtmlTemplate.Trim('`');
            if (HtmlTemplate.Substring(0, 4).ToLower() == "html")
                HtmlTemplate = HtmlTemplate.Substring(4);
        }
        public async Task<byte[]> CreateNewPolicy(PassportData passportData, VeniclePlateData venicleIdData, int price)
        {

            var htmlRes = HtmlTemplate.Replace("[FakeName]", passportData.Name);
            htmlRes = htmlRes.Replace("[FakePassportId]", passportData.Id);
            htmlRes = htmlRes.Replace("[FakeLicensePlateId]", venicleIdData.Id);
            htmlRes = htmlRes.Replace("[FakePrice]", price.ToString());


            var renderer = new ChromePdfRenderer();

            var pdf = renderer.RenderHtmlAsPdf(htmlRes);

            return pdf.BinaryData;

            //using (MemoryStream ms = new MemoryStream())
            //{

            //    var pdf = PdfGenerator.GeneratePdf(htmlRes, PdfSharp.PageSize.A4);
            //    pdf.Save(ms);
            //    documentBytes = ms.ToArray();
            //}
        }
    }
}
