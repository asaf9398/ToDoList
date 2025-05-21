using Common.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ToDoListClient.Helpers;

namespace ToDoListClient.Services
{
    public class TaskApiService
    {
        private readonly HttpClient _client;

        public TaskApiService()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(Config.GetApiBaseUrl())
            };
        }

        public async Task<List<TaskDto>> GetAllAsync()
        {
            try
            {
                var response = await _client.GetFromJsonAsync<List<TaskDto>>("");
                return response ?? new List<TaskDto>();
            }
            catch
            {
                return new List<TaskDto>();
            }
        }

        public async Task<TaskDto?> AddAsync(TaskDto task)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("", task);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TaskDto>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<TaskDto?> UpdateAsync(TaskDto task)
        {
            try
            {
                var response = await _client.PutAsJsonAsync("", task);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TaskDto>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _client.DeleteAsync($"{_client.BaseAddress}/?id={id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LockTaskAsync(Guid id)
        {
            try
            {
                var response = await _client.PostAsync($"{_client.BaseAddress}/lock/{id}", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UnlockTaskAsync(Guid id)
        {
            try
            {
                var response = await _client.PostAsync($"{_client.BaseAddress}/unlock/{id}", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
