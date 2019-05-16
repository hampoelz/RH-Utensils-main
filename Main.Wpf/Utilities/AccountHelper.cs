using Auth0.OidcClient;
using RestSharp;
using System;

namespace Main.Wpf.Utilities
{
    public static class AccountHelper
    {
        public static Auth0Client Client = new Auth0Client(new Auth0ClientOptions
        {
            Domain = Config.Auth0.Domain,
            ClientId = Config.Auth0.ClientId
        });

        public static string UserId;

        public static string GetToken()
        {
            LogFile.WriteLog("Receive account token from " + Config.Auth0.Domain + " ...");

            try
            {
                var client = new RestClient("https://" + Config.Auth0.Domain + "/oauth/token");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json",
                    "{\"grant_type\":\"client_credentials\",\"client_id\": \"" + Config.Auth0.ApiClientId +
                    "\",\"client_secret\": \"" + Config.Auth0.ApiClientSecret + "\",\"audience\": \"https://" +
                    Config.Auth0.Domain + "/api/v2/\"}", ParameterType.RequestBody);
                var response = client.Execute(request);

                return JsonHelper.ReadString(response.Content, "access_token");
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            return "";
        }

        public static string GetTokenType()
        {
            LogFile.WriteLog("Receive account token type from " + Config.Auth0.Domain + " ...");

            try
            {
                var client = new RestClient("https://" + Config.Auth0.Domain + "/oauth/token");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json",
                    "{\"grant_type\":\"client_credentials\",\"client_id\": \"" + Config.Auth0.ApiClientId +
                    "\",\"client_secret\": \"" + Config.Auth0.ApiClientSecret + "\",\"audience\": \"https://" +
                    Config.Auth0.Domain + "/api/v2/\"}", ParameterType.RequestBody);
                var response = client.Execute(request);

                return JsonHelper.ReadString(response.Content, "token_type");
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            return "";
        }

        public static string ReadMetadata()
        {
            LogFile.WriteLog("Read app metadata from " + Config.Auth0.Domain + " ...");

            try
            {
                var appName = Config.Informations.Extension.Name.Replace(" ", "_");

                var client = new RestClient("https://" + Config.Auth0.Domain + "/api/v2/users/" + UserId);
                var request = new RestRequest(Method.GET);
                request.AddHeader("authorization", GetTokenType() + " " + GetToken());
                var response = client.Execute(request);

                try
                {
                    return JsonHelper.ReadString(JsonHelper.ReadString(response.Content, "app_metadata"), appName);
                }
                catch
                {
                    return JsonHelper.ReadString(response.Content, "app_metadata");
                }
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            return "";
        }

        public static void SetMetadata(string jsonData)
        {
            LogFile.WriteLog("Set app metadata on " + Config.Auth0.Domain + " ...");

            if (!InternetHelper.CheckConnection()) return;

            try
            {
                var appName = Config.Informations.Extension.Name.Replace(" ", "_");

                var client = new RestClient("https://" + Config.Auth0.Domain + "/api/v2/users/" + UserId);
                var request = new RestRequest(Method.PATCH);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", GetTokenType() + " " + GetToken());
                request.AddParameter("application/json", "{\"app_metadata\": {\"" + appName + "\": " + jsonData + "}}",
                    ParameterType.RequestBody);
                client.Execute(request);
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }
    }
}