using WebUI.Components.Models;

namespace WebUI.Components
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "http://host.docker.internal:5003";

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

            var response = await _httpClient.PostAsJsonAsync(BASE_URL + "/gateway/auth/login", loginPayload);
            if (response.IsSuccessStatusCode)
            {
                var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return auth?.Token;
            }

            return null;
        }

        //===============================STUDENT=================================
        public async Task<List<StudentModel>> GetStudentsAsync()
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return new List<StudentModel>();

            var request = new HttpRequestMessage(HttpMethod.Get, BASE_URL +  "/gateway/students");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<StudentModel>>();
            }

            return new List<StudentModel>();
        }

        public async Task<bool> SubmitStudentAsync(StudentModel student)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            var request = new HttpRequestMessage(HttpMethod.Post, BASE_URL + "/gateway/students");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Content = JsonContent.Create(student);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<StudentModel> GetStudentsAsyncById(int Id)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return new StudentModel();

            var request = new HttpRequestMessage(HttpMethod.Get, BASE_URL + $"/gateway/students/{Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<StudentModel>();
            }

            return new StudentModel();
        }

        public async Task<bool> UpdateStudentAsync(StudentModel updatedStudent, int id)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            var request = new HttpRequestMessage(HttpMethod.Put, BASE_URL + $"/gateway/students/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Content = JsonContent.Create(updatedStudent);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            var request = new HttpRequestMessage(HttpMethod.Delete, BASE_URL + $"/gateway/students/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }



        //===============================TEACHER=================================

        public async Task<List<TeacherModel>> GetTeachersAsync()
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return new List<TeacherModel>();

            var request = new HttpRequestMessage(HttpMethod.Get, BASE_URL + "/gateway/teachers");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<TeacherModel>>();
            }

            return new List<TeacherModel>();
        }

        public async Task<bool> SubmitTeacherAsync(TeacherModel student)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            var request = new HttpRequestMessage(HttpMethod.Post, BASE_URL + "/gateway/teachers");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Content = JsonContent.Create(student);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<TeacherModel> GetTeachersAsyncById(int Id)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return new TeacherModel();

            var request = new HttpRequestMessage(HttpMethod.Get, BASE_URL + $"/gateway/teachers/{Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TeacherModel>();
            }

            return new TeacherModel();
        }

        public async Task<bool> UpdateTeacherAsync(TeacherModel updatedStudent, int id)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            var request = new HttpRequestMessage(HttpMethod.Put, BASE_URL + $"/gateway/teachers/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Content = JsonContent.Create(updatedStudent);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTeacherAsync(int id)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            var request = new HttpRequestMessage(HttpMethod.Delete, BASE_URL + $"/gateway/teachers/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }


        //===============================CLAZZ=================================

        public async Task<List<ClazzModel>> GetClazzesAsync()
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return new List<ClazzModel>();

            var request = new HttpRequestMessage(HttpMethod.Get, BASE_URL + "/gateway/clazzes");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {

                List<ClazzModel> clazzModels = await response.Content.ReadFromJsonAsync<List<ClazzModel>>();

                foreach (var clazz in clazzModels)
                {
                    if (clazz.StudentIds != null && clazz.StudentIds.Any())
                    {
                        var studentTasks = clazz.StudentIds.Select(id => GetStudentsAsyncById(id));
                        var students = await Task.WhenAll(studentTasks);
                        clazz.Students = students.ToList();
                    }
                    else
                    {
                        clazz.Students = new List<StudentModel>();
                    }


                    if (clazz.TeacherId.HasValue)
                    {
                        var teacher = await GetTeachersAsyncById(clazz.TeacherId.Value);
                        clazz.TeacherName = teacher.Name;
                    }
                    else
                    {
                        clazz.TeacherName = "No Teacher Assigned";
                    }

                }

                return clazzModels;

            }

            return new List<ClazzModel>();
        }

        public async Task<bool> SubmitClazzAsync(ClazzModel student)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            var request = new HttpRequestMessage(HttpMethod.Post, BASE_URL + "/gateway/clazzes");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Content = JsonContent.Create(student);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<ClazzModel> GetClazzesAsyncById(int Id)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return new ClazzModel();

            var request = new HttpRequestMessage(HttpMethod.Get, BASE_URL + $"/gateway/clazzes/{Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                ClazzModel clazzModel = await response.Content.ReadFromJsonAsync<ClazzModel>();
                    if (clazzModel.StudentIds != null && clazzModel.StudentIds.Any())
                    {
                        var studentTasks = clazzModel.StudentIds.Select(id => GetStudentsAsyncById(id));
                        var students = await Task.WhenAll(studentTasks);
                        clazzModel.Students = students.ToList();
                    }
                    else
                    {
                        clazzModel.Students = new List<StudentModel>();
                    }


                    if (clazzModel.TeacherId.HasValue)
                    {
                        var teacher = await GetTeachersAsyncById(clazzModel.TeacherId.Value);
                        clazzModel.TeacherName = teacher.Name;
                    }
                    else
                    {
                        clazzModel.TeacherName = "No Teacher Assigned";
                    }

               

                return clazzModel;
            }

            return new ClazzModel();
        }

        public async Task<bool> UpdateClazzAsync(ClazzModel updatedStudent, int id)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            var request = new HttpRequestMessage(HttpMethod.Put, BASE_URL + $"/gateway/clazzes/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Content = JsonContent.Create(updatedStudent);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteClazzAsync(int id)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            var request = new HttpRequestMessage(HttpMethod.Delete, BASE_URL + $"/gateway/clazzes/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AssignClazzAsync(AssignModel assignModel, int id)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            var request = new HttpRequestMessage(HttpMethod.Post, BASE_URL + $"/gateway/clazzes/{id}/assign");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Content = JsonContent.Create(assignModel);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }


    }

}
