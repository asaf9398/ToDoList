using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using ToDoListClient.Helpers;
using ToDoListClient.Models;

namespace ToDoListClient.Services
{
    public class SignalRService
    {
        private HubConnection? _connection;

        public event Action<TaskDto>? TaskAdded;
        public event Action<TaskDto>? TaskUpdated;
        public event Action<Guid>? TaskDeleted;
        public event Action<Guid, string>? TaskLocked;
        public event Action<Guid>? TaskUnlocked;
        public bool IsConnected => _connection?.State == HubConnectionState.Connected;

        public async Task StartAsync()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(Config.GetSignalRHubUrl())
                .WithAutomaticReconnect()
                .Build();

            RegisterHandlers();

            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SignalR connection failed: " + ex.Message);
            }
        }

        private void RegisterHandlers()
        {
            if (_connection == null)
                return;

            _connection.On<TaskDto>("TaskAdded", task => TaskAdded?.Invoke(task));
            _connection.On<TaskDto>("TaskUpdated", task => TaskUpdated?.Invoke(task));
            _connection.On<Guid>("TaskDeleted", id => TaskDeleted?.Invoke(id));
            _connection.On<Guid, string>("TaskLocked", (id, user) => TaskLocked?.Invoke(id, user));
            _connection.On<Guid>("TaskUnlocked", id => TaskUnlocked?.Invoke(id));
        }

        public async Task StopAsync()
        {
            if (_connection != null)
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
            }
        }

    }
}
