using Common.Enums;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ToDoListClient.Helpers;
using ToDoListClient.Models;

namespace ToDoListClient.ViewModels
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        private readonly TaskDto _task;
        private readonly string _currentUsername = "guest";

        public TaskViewModel(TaskDto task)
        {
            _task = task;
        }

        public Guid Id => _task.Id;
        public string Title
        {
            get => _task.Title;
            set { _task.Title = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _task.Description;
            set { _task.Description = value; OnPropertyChanged(); }
        }

        public TaskPriority Priority
        {
            get => _task.Priority;
            set { _task.Priority = value; OnPropertyChanged(); }
        }

        public bool IsCompleted
        {
            get => _task.IsCompleted;
            set { _task.IsCompleted = value; OnPropertyChanged(); }
        }

        public string? LockedBy
        {
            get => _task.LockedBy;
            set { _task.LockedBy = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsLocked)); OnPropertyChanged(nameof(IsLockedByMe)); }
        }

        public bool IsLocked => !string.IsNullOrEmpty(LockedBy);
        public bool IsLockedByMe => LockedBy == _currentUsername;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
