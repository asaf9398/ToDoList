using System.Windows;
using ToDoListClient.Models;

namespace ToDoListClient.Views
{
    public partial class EditTaskWindow : Window
    {
        public TaskDto EditedTask { get; private set; }

        public EditTaskWindow(TaskDto task)
        {
            InitializeComponent();

            EditedTask = new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Priority = task.Priority,
                IsCompleted = task.IsCompleted,
                LockedBy = task.LockedBy
            };

            DataContext = EditedTask;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
