using Azure.AI.Vision;
using Azure.AI.Vision.ImageAnalysis;
namespace Blazor_AI_ComputerViosn_ImageTableReader.AIProcessor
{
    /// <summary>
    /// Class for Processing the Uploaded Image
    /// </summary>
    public class AIImageProcessor(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;
        public async Task<string> UploadAndProcessImageAsync(Stream stream)
        {
            string strText = string.Empty;
            var key = _configuration["AzureComputerVisionAIServiceKey"];
            var endpoint = _configuration["AzureComputerVisionAIServiceEndpoint"];

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(endpoint))
            {
                throw new ArgumentNullException("Azure Computer Vision AI Service key or endpoint is not configured properly.");
            }

            var client = new ImageAnalysisClient(new Uri(endpoint), new Azure.AzureKeyCredential(key));

            var result = await client.AnalyzeAsync(BinaryData.FromStream(stream), VisualFeatures.Read);

            foreach (DetectedTextBlock block in result.Value.Read.Blocks)
            {
                foreach (DetectedTextLine line in block.Lines)
                {
                    strText += line.Text + Environment.NewLine;
                }
            }
            return strText;
        }

    }
}
