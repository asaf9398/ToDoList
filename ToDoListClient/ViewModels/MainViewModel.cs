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
using System.Windows;

namespace ToDoListClient.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly TaskApiService _apiService;
        private readonly SignalRService _signalRService;

        public ObservableCollection<TaskViewModel> Tasks { get; set; } = new();

        private TaskViewModel? _selectedTask;
        public TaskViewModel? SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged();
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
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
            SortTasks();
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

            foreach (var task in tasks
                .OrderBy(t => t.IsCompleted)
                .ThenByDescending(t => t.Priority))
            {
                Tasks.Add(new TaskViewModel(task));
            }
        }


        private void RegisterSignalREvents()
        {
            _signalRService.TaskAdded += task =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (!Tasks.Any(t => t.Id == task.Id))
                        Tasks.Add(new TaskViewModel(task));
                    SortTasks();
                });
            };

            _signalRService.TaskUpdated += task =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var existing = Tasks.FirstOrDefault(t => t.Id == task.Id);

                    if (existing != null)
                    {
                        Tasks.Remove(existing);
                    }

                    Tasks.Add(new TaskViewModel(task));
                    SortTasks();
                });
            };

            _signalRService.TaskLocked += (id, user) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var task = Tasks.FirstOrDefault(t => t.Id == id);
                    if (task != null)
                        task.LockedBy = user;
                    SortTasks();
                });
            };

            _signalRService.TaskUnlocked += id =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var task = Tasks.FirstOrDefault(t => t.Id == id);
                    if (task != null)
                        task.LockedBy = null;
                    SortTasks();
                });
            };

            _signalRService.TaskDeleted += id =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var task = Tasks.FirstOrDefault(t => t.Id == id);
                    if (task != null)
                        Tasks.Remove(task);
                    SortTasks();
                });
            };


        }

        private async Task AddTaskAsync()
        {
            var newTask = new TaskDto
            {
                Id = Guid.NewGuid(),
                Title = "",
                Description = "",
                Priority = TaskPriority.Low,
                IsCompleted = false
            };

            var editWindow = new EditTaskWindow(newTask);
            var result = editWindow.ShowDialog();

            if (result == true)
            {
                var added = await _apiService.AddAsync(editWindow.EditedTask);

                if (added == null)
                {
                    MessageBox.Show($"The task did not be add due to server error!", "Adding Task Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task DeleteTaskAsync()
        {
            if (SelectedTask == null) return;

            await _apiService.DeleteAsync(SelectedTask.Id);
        }

        private void SortTasks()
        {
            var sorted = Tasks.OrderBy(t => t.IsCompleted)
                              .ThenByDescending(t => t.Priority)
                              .ToList();

            Tasks.Clear();
            foreach (var task in sorted)
                Tasks.Add(task);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
