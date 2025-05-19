using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDoListClient.Models;
using ToDoListClient.Services;
using ToDoListClient.Helpers;
using Common.Enums;
using ToDoListClient.Views;

namespace ToDoListClient.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly TaskApiService _apiService;
        private readonly SignalRService _signalRService;

        public ObservableCollection<TaskDto> Tasks { get; set; } = new();

        private TaskDto? _selectedTask;
        public TaskDto? SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        public MainViewModel()
        {
            _apiService = new TaskApiService();
            _signalRService = new SignalRService();

            AddCommand = new RelayCommand(async _ => await AddTaskAsync());
            DeleteCommand = new RelayCommand(async _ => await DeleteTaskAsync(), _ => SelectedTask != null);
           
            _ = InitializeAsync();
        }

        public async Task<bool> LockTaskAsync(Guid id)
        {
            return await _apiService.LockTaskAsync(id);
        }

        public async Task UnlockTaskAsync(Guid id)
        {
            await _apiService.UnlockTaskAsync(id);
        }

        public async Task UpdateTaskAsync(TaskDto task)
        {
            await _apiService.UpdateAsync(task);
        }

        private async Task InitializeAsync()
        {
            await LoadTasksAsync();
            await _signalRService.StartAsync();
            RegisterSignalREvents();
        }

        private async Task LoadTasksAsync()
        {
            var tasks = await _apiService.GetAllAsync();
            Tasks.Clear();
            foreach (var task in tasks)
                Tasks.Add(task);
        }

        private void RegisterSignalREvents()
        {
            _signalRService.TaskAdded += task =>
            {
                App.Current.Dispatcher.Invoke(() => Tasks.Add(task));
            };

            _signalRService.TaskUpdated += task =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var existing = Tasks.FirstOrDefault(t => t.Id == task.Id);
                    if (existing != null)
                    {
                        var index = Tasks.IndexOf(existing);
                        Tasks[index] = task;
                    }
                });
            };

            _signalRService.TaskDeleted += id =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var task = Tasks.FirstOrDefault(t => t.Id == id);
                    if (task != null)
                        Tasks.Remove(task);
                });
            };

            _signalRService.TaskLocked += (id, user) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var task = Tasks.FirstOrDefault(t => t.Id == id);
                    if (task != null)
                        task.LockedBy = user;
                });
            };

            _signalRService.TaskUnlocked += id =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var task = Tasks.FirstOrDefault(t => t.Id == id);
                    if (task != null)
                        task.LockedBy = null;
                });
            };
        }

        private async Task AddTaskAsync()
        {
            var newTask = new TaskDto
            {
                Id = Guid.NewGuid()
            };

            var editWindow = new EditTaskWindow(newTask);
            var result = editWindow.ShowDialog();

            if (result == true)
            {
                var added = await _apiService.AddAsync(editWindow.EditedTask);
                if (added != null)
                {
                    Tasks.Add(added);
                }
            }
        }

        private async Task DeleteTaskAsync()
        {
            if (SelectedTask == null) return;

            await _apiService.DeleteAsync(SelectedTask.Id);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
