using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Cognitive.CustomVision;
using Microsoft.Cognitive.CustomVision.Models;
using System.Diagnostics;

namespace LogoDetector
{


    public class MSAI
    {

        public string _predictionKey { get; private set; } = "22e67acf75dd46849265203ae667bc7f";
        public string  projectId { get; private set; } = "3a7e5947-1606-4ccf-833c-ca1659068657";
        private  Guid _projectId { get {

                return new Guid(projectId);
            } }

        PredictionEndpoint endpoint = null;
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

       public   MSAI(string predictionKey ,string ProjectId)
        {
            // Create a prediction endpoint, passing in a prediction credentials object that contains the obtained prediction key
            PredictionEndpointCredentials predictionEndpointCredentials = new PredictionEndpointCredentials(_predictionKey);
             endpoint = new PredictionEndpoint(predictionEndpointCredentials);

            
        }

        public async Task<ImagePredictionResultModel> PredictImageAsync(string ImagePath)
        {
           // byte[] byteData = GetImageAsByteArray(ImagePath);
           var byteData = new MemoryStream(File.ReadAllBytes(ImagePath));

            return await endpoint.PredictImageAsync(_projectId, byteData);
        }

        public async Task<ImagePredictionResultModel> PredictImageDataAsync(Stream  ImageData)
        {         
            return await endpoint.PredictImageAsync(_projectId, ImageData);
        }
        public ImageLogoInfo PredictImageData(Stream ImageData, string ImagePath)
        {
            var sw = Stopwatch.StartNew();
            ImageLogoInfo info = new ImageLogoInfo();
            info.ImagePath = ImagePath;
            info.AlgorithmUsed = Algorithm.MicrosoftVision;
            try
            {
                var task = endpoint.PredictImage(_projectId, ImageData);

                ImageTagPrediction tag1 = null, tag2 = null;
                ImageTagPrediction tag = new ImageTagPrediction();
                if (task.Predictions == null) return null;
                if (task.Predictions.Count > 0)
                    tag1 = task.Predictions[0];
                if (task.Predictions.Count > 1)
                    tag2 = task.Predictions[1];
                if (tag2 == null)
                    tag = tag1;
                else if (tag1.Probability > tag2.Probability)
                    tag = tag1;
                else
                    tag = tag2;


                info.Confidence = tag.Tag.Contains("Has Logo") ? (tag.Probability + "").ToPercentagFloat() : ((1 - tag.Probability) + "").ToPercentagFloat();
                  
            }
            catch (Exception ex) { info.Error = ex.FullErrorMessage(); }
            sw.Stop();
            info.ProcessingTime = sw.ElapsedMilliseconds;
            return info;
        }

        public  static async Task<string> MakePredictionRequest(string imageFilePath)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid subscription key.
            client.DefaultRequestHeaders.Add("Prediction-Key", "22e67acf75dd46849265203ae667bc7f");

            // Prediction URL - replace this example URL with your valid prediction URL.
            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/3a7e5947-1606-4ccf-833c-ca1659068657/image?iterationId=17687f02-b1f6-4c77-9d88-98ba5ef16190";

            HttpResponseMessage response;

            // Request body. Try this sample with a locally stored image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                var xx = (await response.Content.ReadAsStringAsync());
                return xx;
            }
        }
    }

}
