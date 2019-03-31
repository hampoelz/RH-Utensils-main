using Auth0.OidcClient;
using RestSharp;
using System;

namespace Main.Wpf.Functions
{
    public static class Account
    {
        public static Auth0Client Client = new Auth0Client(new Auth0ClientOptions
        {
            Domain = App.Auth0Domain,
            ClientId = App.Auth0ClientId
        });

        public static string UserId;

        public static string GetToken()
        {
            LogFile.WriteLog("Received account token from " + App.Auth0Domain + " ...");

            try
            {
                var client = new RestClient("https://" + App.Auth0Domain + "/oauth/token");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json",
                    "{\"grant_type\":\"client_credentials\",\"client_id\": \"" + App.Auth0ApiClientId +
                    "\",\"client_secret\": \"" + App.Auth0ApiClientSecret + "\",\"audience\": \"https://" +
                    App.Auth0Domain + "/api/v2/\"}", ParameterType.RequestBody);
                var response = client.Execute(request);

                return Json.ConvertToString(response.Content, "access_token");
            }
            catch (Exception e)
            {
                LogFile.WriteLog(e);
            }

            return "";
        }

        public static string GetTokenType()
        {
            LogFile.WriteLog("Received account token type from " + App.Auth0Domain + " ...");

            try
            {
                var client = new RestClient("https://" + App.Auth0Domain + "/oauth/token");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json",
                    "{\"grant_type\":\"client_credentials\",\"client_id\": \"" + App.Auth0ApiClientId +
                    "\",\"client_secret\": \"" + App.Auth0ApiClientSecret + "\",\"audience\": \"https://" +
                    App.Auth0Domain + "/api/v2/\"}", ParameterType.RequestBody);
                var response = client.Execute(request);

                return Json.ConvertToString(response.Content, "token_type");
            }
            catch (Exception e)
            {
                LogFile.WriteLog(e);
            }

            return "";
        }

        public static string ReadMetadata()
        {
            LogFile.WriteLog("Read app metadata from " + App.Auth0Domain + " ...");

            try
            {
                var appName = App.Name.Replace(" ", "_");

                var client = new RestClient("https://" + App.Auth0Domain + "/api/v2/users/" + UserId);
                var request = new RestRequest(Method.GET);
                request.AddHeader("authorization", GetTokenType() + " " + GetToken());
                var response = client.Execute(request);

                try
                {
                    return Json.ConvertToString(Json.ConvertToString(response.Content, "app_metadata"), appName);
                }
                catch
                {
                    return Json.ConvertToString(response.Content, "app_metadata");
                }
            }
            catch (Exception e)
            {
                LogFile.WriteLog(e);
            }

            return "";
        }

        public static void SetMetadata(string jsonData)
        {
            LogFile.WriteLog("Set app metadata on " + App.Auth0Domain + " ...");

            if (!InternetChecker.Check()) return;

            try
            {
                var appName = App.Name.Replace(" ", "_");

                var client = new RestClient("https://" + App.Auth0Domain + "/api/v2/users/" + UserId);
                var request = new RestRequest(Method.PATCH);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", GetTokenType() + " " + GetToken());
                request.AddParameter("application/json", "{\"app_metadata\": {\"" + appName + "\": " + jsonData + "}}",
                    ParameterType.RequestBody);
                client.Execute(request);
            }
            catch (Exception e)
            {
                LogFile.WriteLog(e);
            }
        }
    }
}