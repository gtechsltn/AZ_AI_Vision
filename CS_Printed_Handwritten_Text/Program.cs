// See https://aka.ms/new-console-template for more information
using Azure.AI.Vision.ImageAnalysis;
using Azure;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Net;
using System.Text;

Console.WriteLine("Azure AI Vision v3.2 GA Read");

try
{

    string key = "";
    string endpoint = "";

   //string READ_TEXT_URL_IMAGE = "https://raw.githubusercontent.com/Azure-Samples/cognitive-services-sample-data-files/master/ComputerVision/Images/printed_text.jpg";

    string READ_TEXT_URL_IMAGE = Path.GetRelativePath(AppContext.BaseDirectory, 
            "D:\\MyDemos\\ai\\AZ_AI_Vision\\CS_Printed_Handwritten_Text\\printed_Texts\\jb.jpg");

    Console.WriteLine($"Get Relative path : {READ_TEXT_URL_IMAGE}");

    ComputerVisionClient computerVisionClient = Authenticate(endpoint, key);


    //await ReadFileUrl(computerVisionClient, READ_TEXT_URL_IMAGE);
    Console.WriteLine("Analysis Image");

   // await ImageTextAnalysis();

    await AnalyzeImageUrl();
}
catch (Exception ex)
{
    Console.WriteLine($"Error Message : {ex.Message} ");
}

Console.ReadLine();



ComputerVisionClient Authenticate(string endpoint, string key)
{
    ComputerVisionClient client =
      new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
      { Endpoint = endpoint };
    return client;
}


async Task ReadFileUrl(ComputerVisionClient client, string urlFile)
{
    Console.WriteLine("----------------------------------------------------------");
    Console.WriteLine("READ FILE FROM URL");
    Console.WriteLine();

 


    using (Stream imageStream = File.OpenRead(urlFile))
    {
        var features = new List<VisualFeatureTypes?> { VisualFeatureTypes.Description, VisualFeatureTypes.Objects, VisualFeatureTypes.Categories };
        var result = await client.AnalyzeImageInStreamAsync(imageStream, features);

        Console.WriteLine("Description: ");
        foreach (var caption in result.Description.Captions)
        {
            Console.WriteLine($"{caption.Text} with confidence {caption.Confidence}");
        }
    }



    //var textHeaders = await client.ReadInStreamAsync(File.OpenRead(urlFile), language: "en");
    var textHeaders = await client.ReadInStreamAsync(File.OpenRead(urlFile), language: "en");
    
    string operationLocation = textHeaders.OperationLocation;
    Thread.Sleep(2000);
    const int numberOfCharsInOperationId = 36;
    string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

    ReadOperationResult results;
    Console.WriteLine($"Extracting text from URL file {Path.GetFileName(urlFile)}...");
    Console.WriteLine();

    do
    {
        results = await client.GetReadResultAsync(Guid.Parse(operationId));
    }
    while ((results.Status == OperationStatusCodes.Running ||
        results.Status == OperationStatusCodes.NotStarted));


    Console.WriteLine();
    var textUrlFileResults = results.AnalyzeResult.ReadResults;
    foreach (Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models.ReadResult page in textUrlFileResults)
    {
        foreach (Line line in page.Lines)
        {
            Console.WriteLine(line.Text);
        }
    }
    Console.WriteLine();


    Console.WriteLine("The Text Reading");
    var res = await client.RecognizePrintedTextInStreamAsync(true, File.OpenRead(urlFile));
    res.Regions.ToList().ForEach(region =>
    {
        region.Lines.ToList().ForEach(line =>
        {
            line.Words.ToList().ForEach(word =>
            {
                Console.WriteLine(word.Text);
            });
        });
    });
}


async Task ImageTextAnalysis()
{
    var client = new ImageAnalysisClient(new Uri("https://msitvision.cognitiveservices.azure.com/"),
            new AzureKeyCredential("7KtvmqupSJlv7gNwcdaJQD42Qc8PRrya4J6msWkhpUeriGIFBz9lJQQJ99AKAC4f1cMXJ3w3AAAFACOGt7FO"));

    byte[] imageData = ImageToBinary("D:\\MyDemos\\ai\\AZ_AI_Vision\\CS_Printed_Handwritten_Text\\printed_Texts\\patrika" +
        ".jpg");

    var options = new ImageAnalysisOptions
    {
        Language = "mr"  // Marathi language code
    };

    try
    {
        var result = await client.AnalyzeAsync(BinaryData.FromBytes(imageData), VisualFeatures.Read, options);

        var blocks = result.Value.Read.Blocks;

        Console.OutputEncoding = Encoding.UTF8; // Ensure console can display Marathi characters

     

        foreach (DetectedTextBlock block in result.Value.Read.Blocks)
        {
            foreach (DetectedTextLine line in block.Lines)
            {
                Console.WriteLine($"Line: '{line.Text}'");
            }
        }


    }
    catch (RequestFailedException e)
    {
        Console.WriteLine($"Error: {e.Message}");
    }
}

 byte[] ImageToBinary(string imagePath)
{
    using (FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
    {
        byte[] buffer = new byte[fileStream.Length];
        fileStream.Read(buffer, 0, (int)fileStream.Length);
        return buffer;
    }
}




 
 
   
 

 async Task AnalyzeImageUrl()
        {
    try
    {
        string READ_TEXT_URL_IMAGE = Path.GetRelativePath(AppContext.BaseDirectory,
       "D:\\MyDemos\\ai\\AZ_AI_Vision\\CS_Printed_Handwritten_Text\\printed_Texts\\sampleimage.jpg");



        ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials("7KtvmqupSJlv7gNwcdaJQD42Qc8PRrya4J6msWkhpUeriGIFBz9lJQQJ99AKAC4f1cMXJ3w3AAAFACOGt7FO"))
              { Endpoint = "https://msitvision.cognitiveservices.azure.com/" };
        Console.WriteLine("----------------------------------------------------------");
        Console.WriteLine("ANALYZE IMAGE - URL");
        Console.WriteLine($"URL:  {READ_TEXT_URL_IMAGE}");

        // Creating a list that defines the features to be extracted from the image. 

        List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Tags
            };

        Console.WriteLine($"Analyzing the image {Path.GetFileName(READ_TEXT_URL_IMAGE)}...");
        Console.WriteLine();
        // Analyze the URL image 
        ImageAnalysis results = await client.AnalyzeImageInStreamAsync(
            File.OpenRead("D:\\MyDemos\\ai\\AZ_AI_Vision\\CS_Printed_Handwritten_Text\\printed_Texts\\buildings.jpg"), 
            visualFeatures: features);

        // Image tags and their confidence score
        Console.WriteLine("Tags:");
        foreach (var tag in results.Tags)
        {
            Console.WriteLine($"{tag.Name} {tag.Confidence}");
        }
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error In Method : {ex.Message}");
    }
    Console.ReadLine();
}
 