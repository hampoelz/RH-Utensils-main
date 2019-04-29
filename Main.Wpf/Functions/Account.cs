using Auth0.OidcClient;
using RestSharp;
using System;

namespace Main.Wpf.Functions
{
    public static class Account
    {
        public static class Auth0
        {
            private static string domain = "hampoelz.eu.auth0.com";

            public static string Domain
            {
                get => domain;
                set
                {
                    if (domain == value || value?.Length == 0) return;

                    domain = value;
                }
            }

            private static string clientId = "_9ZvrbGJUX4MfWdzt6F7pW2e0Z0Zc0OA";

            public static string ClientId
            {
                get => clientId;
                set
                {
                    if (clientId == value || value?.Length == 0) return;

                    clientId = value;
                }
            }

            private static string apiClientId = "GTgQvzJvhsSPT0w8sirtIj69cTwfS9AW";

            public static string ApiClientId
            {
                get => apiClientId;
                set
                {
                    if (apiClientId == value || value?.Length == 0) return;

                    apiClientId = value;
                }
            }

            private static string apiClientSecret = "J4db362UcFbgrQBaXb0doKt4MNEjyPh4W2kueckfCpEppl2zHzB8xyLu3N7REknh";

            public static string ApiClientSecret
            {
                get => apiClientSecret;
                set
                {
                    if (apiClientSecret == value || value?.Length == 0) return;

                    apiClientSecret = value;
                }
            }
        }

        public static Auth0Client Client = new Auth0Client(new Auth0ClientOptions
        {
            Domain = Auth0.Domain,
            ClientId = Auth0.ClientId
        });

        public static string UserId;

        public static string GetToken()
        {
            LogFile.WriteLog("Receive account token from " + Auth0.Domain + " ...");

            try
            {
                var client = new RestClient("https://" + Auth0.Domain + "/oauth/token");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json",
                    "{\"grant_type\":\"client_credentials\",\"client_id\": \"" + Auth0.ApiClientId +
                    "\",\"client_secret\": \"" + Auth0.ApiClientSecret + "\",\"audience\": \"https://" +
                    Auth0.Domain + "/api/v2/\"}", ParameterType.RequestBody);
                var response = client.Execute(request);

                return Json.ReadString(response.Content, "access_token");
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            return "";
        }

        public static string GetTokenType()
        {
            LogFile.WriteLog("Receive account token type from " + Auth0.Domain + " ...");

            try
            {
                var client = new RestClient("https://" + Auth0.Domain + "/oauth/token");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json",
                    "{\"grant_type\":\"client_credentials\",\"client_id\": \"" + Auth0.ApiClientId +
                    "\",\"client_secret\": \"" + Auth0.ApiClientSecret + "\",\"audience\": \"https://" +
                    Auth0.Domain + "/api/v2/\"}", ParameterType.RequestBody);
                var response = client.Execute(request);

                return Json.ReadString(response.Content, "token_type");
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            return "";
        }

        public static string ReadMetadata()
        {
            LogFile.WriteLog("Read app metadata from " + Auth0.Domain + " ...");

            try
            {
                var appName = Informations.Extension.Name.Replace(" ", "_");

                var client = new RestClient("https://" + Auth0.Domain + "/api/v2/users/" + UserId);
                var request = new RestRequest(Method.GET);
                request.AddHeader("authorization", GetTokenType() + " " + GetToken());
                var response = client.Execute(request);

                try
                {
                    return Json.ReadString(Json.ReadString(response.Content, "app_metadata"), appName);
                }
                catch
                {
                    return Json.ReadString(response.Content, "app_metadata");
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
            LogFile.WriteLog("Set app metadata on " + Auth0.Domain + " ...");

            if (!InternetChecker.Check()) return;

            try
            {
                var appName = Informations.Extension.Name.Replace(" ", "_");

                var client = new RestClient("https://" + Auth0.Domain + "/api/v2/users/" + UserId);
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