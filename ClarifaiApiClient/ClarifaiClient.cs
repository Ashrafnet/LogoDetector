using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClarifaiApiClient
{
    using Util;
    using Models;
    using System.Drawing;

    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class ClarifaiClient
    {
        #region Variables
        private string _clientId = String.Empty;
        public string ClientId {
            get
            {
                return _clientId;
            }
        }
        private string _clientSecret = String.Empty;
        public string ClientSecret
        {
            get
            {
                return _clientSecret;
            }
        }
        private string _accessToken = String.Empty;
        public string AccessToken
        {
            get
            {
                return _accessToken;
            }
        }
        public string ModelPath { get; set; }

        #endregion
        #region Instantiation
        public ClarifaiClient(string clientId, string clientSecret,string model_path)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            ModelPath = model_path;
            ClarifaiHttpClient.SetupModelPath(ModelPath);
        }

        public ClarifaiClient(string accessToken, string model_path)
        {
            _accessToken = accessToken;
            ModelPath = model_path;
            ClarifaiHttpClient.SetupModelPath(ModelPath);
        }
        #endregion
        #region Access Token Methods
        public async Task GenerateToken()
        {
            var response = await ClarifaiHttpClient.GetToken(ClientId, ClientSecret);
            if (response is TokenError)
            {
                TokenError er = (TokenError)response;
                _accessToken = "";
                throw new Exception(er.Status_Msg);
            }
            else
            {
                Token token = (Token)response;
                _accessToken = token.Access_Token;
            }
        }
        public async Task<object> GetToken()
        {
            var response = await ClarifaiHttpClient.GetToken(ClientId, ClientSecret);
            return response;
        }
        #endregion
        #region Prediction Methods
        public async Task<Predict> PredictByImgURL(List<string> urls)
        {
            Predict response = await ClarifaiHttpClient.GetImgUrlPrediction(AccessToken, urls);
            return response;
        }
        public async Task<Predict> PredictByFolderPath(string folder_path)
        {
            Predict response = await ClarifaiHttpClient.GetFolderImgsPrediction(AccessToken, folder_path);
            return response;
        }

        public async Task<Predict> GetImgsPrediction(Dictionary<string, Bitmap> Images)
        {
            Predict response = await ClarifaiHttpClient.GetImgsPrediction(AccessToken, Images);
            return response;
        }

        public async Task<PredictOutput> TrainWithImage(string  ConceptID, Bitmap Image,bool hasLogo)
        {
            var response = await ClarifaiHttpClient.TrainWithImage(AccessToken, ConceptID, Image, hasLogo);
            return response;
        }

        public async Task<PredictOutput> TrainWithImages(string ConceptID,List< Bitmap> Images, bool hasLogo)
        {
            var response = await ClarifaiHttpClient.TrainWithImages(AccessToken, ConceptID, Images, hasLogo);
            return response;
        }

        public async Task<Predict> TrainModel()
        {
            var response = await ClarifaiHttpClient.TrainModel(AccessToken);
            return response;
        }

        #endregion
    }
}
