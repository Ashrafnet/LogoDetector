using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClarifaiApiClient.Util
{
    using Models;
    public class ClarifaiHttpClient
    {
        public static bool ValidateServerCertificate(
                                                    object sender,
                                                    X509Certificate certificate,
                                                    X509Chain chain,
                                                    SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private static string _apiEndPoint = "https://api.clarifai.com";
        private static string _tokenPath = "/v1/token/";
        private static string _predictPath = "/v2/models/Cropped Logo Detector/outputs";

        public static async Task<object> GetToken(string clientId, string clientSecret)
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            //
            //  MESSAGE CONTENT
            string postData = "client_id="+ clientId + "&client_secret="+ clientSecret + "&grant_type=client_credentials";
            //string postData = "client_id=Wads5t98mMepkTuQFLbQvdYBPH0xIBz_7KqdhMIp&client_secret=Igqe-qov2dbcS8EQDHoI8DxRR0PiEN3y3Tj4Vu2m&grant_type=client_credentials";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            
            //
            //  CREATE REQUEST
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(_apiEndPoint + _tokenPath);
            Request.Method = "POST";
            Request.KeepAlive = false;
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Headers.Add("cache-control", "no-cache");

            Stream dataStream = await Request.GetRequestStreamAsync();
            await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //
            //  SEND MESSAGE
            try
            {
                WebResponse Response = await Request.GetResponseAsync();

                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string responseLine = await Reader.ReadToEndAsync();
                Reader.Close();

                HttpStatusCode ResponseCode = ((HttpWebResponse)Response).StatusCode;
                if (!ResponseCode.Equals(HttpStatusCode.OK))
                {
                    TokenError error = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenError>(responseLine);
                    return error;
                }

                Token token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(responseLine);
                return token;
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        TokenError error = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenError>(text);
                        return error;
                    }
                }
            }
            
            return new TokenError
            {
                Status_Code = "Undefined Error",
                Status_Msg = "Undefined Error"
            };
        }

        public static async Task<Predict> GetImgUrlPrediction(string accessToken, List<string> imgURLs)
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            //
            //  MESSAGE CONTENT
            //string postData = "client_id="+ clientId + "&client_secret="+ clientSecret + "&grant_type=client_credentials";
            List <PredictInput> inputs = new List<PredictInput>();
            foreach (string imgUrl in imgURLs) {
                inputs.Add(new PredictInput
                {
                    Data = new PredictImage
                    {
                        Image = new PredictImageData
                        {
                            Url = imgUrl
                        }
                    }
                });
            }
            var ins = new
            {
                Inputs = inputs
            };

            string postData = LowercaseJsonSerializer.SerializeObject(ins);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            //
            //  CREATE REQUEST
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(_apiEndPoint + _predictPath);
            Request.Method = "POST";
            Request.KeepAlive = false;
            Request.ContentType = "application/json";
            Request.Headers.Add("cache-control", "no-cache");
            Request.Headers.Add("authorization", "Bearer "+ accessToken);

            Stream dataStream = await Request.GetRequestStreamAsync();
            await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //
            //  SEND MESSAGE
            try
            {
                WebResponse Response = await Request.GetResponseAsync();

                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string responseLine = await Reader.ReadToEndAsync();
                Reader.Close();

                HttpStatusCode ResponseCode = ((HttpWebResponse)Response).StatusCode;
                if (!ResponseCode.Equals(HttpStatusCode.OK))
                {
                    Predict error = Newtonsoft.Json.JsonConvert.DeserializeObject<Predict>(responseLine);
                    return error;
                }

                Predict predict = Newtonsoft.Json.JsonConvert.DeserializeObject<Predict>(responseLine);
                int ind = 0;
                foreach (string imgUrl in imgURLs)
                {
                    var outputs = predict.Outputs;
                    outputs[ind].Data.Concepts[0].ImageName = imgUrl;
                    ind++;
                }
                return predict;
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        Predict error = Newtonsoft.Json.JsonConvert.DeserializeObject<Predict>(text);
                        return error;
                    }
                }
            }

            return new Predict
            {
                Status = new PredictStatus
                {
                    Code = 0,
                    Description = "Undefined Error"
                }
            };
        }

        public static async Task<Predict> GetFolderImgsPrediction(string accessToken, string folder_path)
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            //
            //  MESSAGE CONTENT
            //string postData = "client_id="+ clientId + "&client_secret="+ clientSecret + "&grant_type=client_credentials";
            List<PredictInput> inputs = new List<PredictInput>();
            var imgPaths = Directory.GetFiles(folder_path, "*.*", SearchOption.AllDirectories).ToList();
            foreach (string imgPath in imgPaths)
            {
                Bitmap source = new Bitmap(imgPath);
                int x = source.Width - 120;
                int y = source.Height - 120;
                Bitmap CroppedImage = source.Clone(new Rectangle(x, y, 120, 120), source.PixelFormat);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CroppedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] imageArray = ms.ToArray();
                //byte[] imageArray = System.IO.File.ReadAllBytes(imgPath);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                inputs.Add(new PredictInput
                {
                    Data = new PredictImage
                    {
                        Image = new PredictImageData
                        {
                            Base64 = base64ImageRepresentation
                        }
                    }
                });
            }
            var ins = new
            {
                Inputs = inputs
            };

            string postData = LowercaseJsonSerializer.SerializeObject(ins);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            //
            //  CREATE REQUEST
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(_apiEndPoint + _predictPath);
            Request.Method = "POST";
            Request.KeepAlive = false;
            Request.ContentType = "application/json";
            Request.Headers.Add("cache-control", "no-cache");
            Request.Headers.Add("authorization", "Bearer " + accessToken);

            Stream dataStream = await Request.GetRequestStreamAsync();
            await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //
            //  SEND MESSAGE
            try
            {
                WebResponse Response = await Request.GetResponseAsync();

                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string responseLine = await Reader.ReadToEndAsync();
                Reader.Close();

                HttpStatusCode ResponseCode = ((HttpWebResponse)Response).StatusCode;
                if (!ResponseCode.Equals(HttpStatusCode.OK))
                {
                    Predict error = Newtonsoft.Json.JsonConvert.DeserializeObject<Predict>(responseLine);
                    return error;
                }

                Predict predict = Newtonsoft.Json.JsonConvert.DeserializeObject<Predict>(responseLine);
                int ind = 0;
                foreach (string imgPath in imgPaths)
                {
                    var outputs = predict.Outputs;
                    outputs[ind].Data.Concepts[0].ImageName = imgPath;
                    ind++;
                }
                    return predict;
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        Predict error = Newtonsoft.Json.JsonConvert.DeserializeObject<Predict>(text);
                        return error;
                    }
                }
            }

            return new Predict
            {
                Status = new PredictStatus
                {
                    Code = 0,
                    Description = "Undefined Error"
                }
            };
        }

    }
}
