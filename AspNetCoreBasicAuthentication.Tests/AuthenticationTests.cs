using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreBasicAuthentication.Models.AccountViewModels;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Xunit;

namespace AspNetCoreBasicAuthentication.Tests
{
    public class AuthenticationTests
    {
        private readonly HttpClient _client = TestServerUtilities.Client;
        private HttpResponseMessage _response;
        private List<string> _deserializedResponse;
        
        private async Task MakeRequest()
        {
            var url = "api/values/values";
            _response = await _client.GetAsync(url);
            if (_response.IsSuccessStatusCode)
            {
                var responseString = await _response.Content.ReadAsStringAsync();
                _deserializedResponse = JsonConvert.DeserializeObject<List<string>>(responseString);
            }
        }
        
        [Fact]
        public async Task UnauthorizedForUnauthenticatedUser()
        {
            await MakeRequest();
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, _response.StatusCode);
        }

        [Fact]
        public async Task OkResponseForCookieAuthentication()
        {
            await AppendCookieAuthenticationHeaderToClient();
            await MakeRequest();
            Assert.Equal(System.Net.HttpStatusCode.OK, _response.StatusCode);
        }

        [Fact]
        public async Task OkResponseForBasicAuthentication()
        {
            AppendBasicAuthenticationHeaderToClient();
            await MakeRequest();
            Assert.Equal(System.Net.HttpStatusCode.OK, _response.StatusCode);
        }

        [Fact]
        public async Task CorrectResponseValuesForCookieAuthentication()
        {
            await AppendCookieAuthenticationHeaderToClient();
            await MakeRequest();
            Assert.Equal(3, _deserializedResponse.Count);
        }

        [Fact]
        public async Task CorrectResponseValuesForBasicAuthentication()
        {
            AppendBasicAuthenticationHeaderToClient();
            await MakeRequest();
            Assert.Equal(3, _deserializedResponse.Count);
        }

        private void AppendBasicAuthenticationHeaderToClient()
        {
            var plainCredentials = $"{AppConstants.USERNAME}:{AppConstants.PASSWORD}";
            var asciiCredentialsBytes = Encoding.ASCII.GetBytes(plainCredentials); // ASCII due to it being sent in the http header.
            var base64Credentials = Convert.ToBase64String(asciiCredentialsBytes);
            _client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64Credentials}");
        }

        private async Task AppendCookieAuthenticationHeaderToClient()
        {
            var loginResponse = await PerformLoginRequest();
            var loginSucceeded = loginResponse.StatusCode == System.Net.HttpStatusCode.Found;
            Assert.True(loginSucceeded, "Login failed");
            SetAuthenticationCookieHeaderForClient(loginResponse);
        }

        private Task<HttpResponseMessage> PerformLoginRequest()
        {
            var newClient = TestServerUtilities.Client;
            var formContent = new MultipartFormDataContent();
            formContent.Add(new StringContent(AppConstants.USERNAME), nameof(LoginViewModel.Identifier));
            formContent.Add(new StringContent(AppConstants.PASSWORD), nameof(LoginViewModel.Password));
            var url = "/Account/Login";
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = formContent
            };
            return newClient.SendAsync(requestMessage);
        }

        private void SetAuthenticationCookieHeaderForClient(HttpResponseMessage loginResponse)
        {
            var cookieHeaders = loginResponse.Headers
                .Where(h => h.Key == "Set-Cookie")
                .SelectMany(h => h.Value)
                .ToList();
            var authCookie = SetCookieHeaderValue.ParseList(cookieHeaders)
                .Single(c=>c.Name == ".AspNetCore.Identity.Application");
            _client.DefaultRequestHeaders.Add("cookie", authCookie.ToString());
        }
    }
}
