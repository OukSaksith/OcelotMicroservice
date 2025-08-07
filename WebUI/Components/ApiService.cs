using WebUI.Components.Models;

namespace WebUI.Components
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<string> GetTokenAsync()
        {
            var loginPayload = new
            {
                username = "webui",
                password = "webui7777"
            };

            var response = await _httpClient.PostAsJsonAsync("http://localhost:5003/gateway/auth/login", loginPayload);
            if (response.IsSuccessStatusCode)
            {
                var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return auth?.Token;
            }

            return null;
        }

        public async Task<List<StudentModel>> GetStudentsAsync()
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return new List<StudentModel>();

            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5003/gateway/students");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<StudentModel>>();
            }

            return new List<StudentModel>();
        }
    }

}
