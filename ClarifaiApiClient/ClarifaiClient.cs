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
        #endregion
        #region Instantiation
        public ClarifaiClient(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public ClarifaiClient(string accessToken)
        {
            _accessToken = accessToken;
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

        #endregion
    }
}
