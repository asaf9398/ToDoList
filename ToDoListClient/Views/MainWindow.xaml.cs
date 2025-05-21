using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ToDoListClient.ViewModels;

namespace ToDoListClient.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void CompletedCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.DataContext is TaskViewModel task)
            {
                if (DataContext is MainViewModel mainViewModel)
                {
                    await mainViewModel.UpdateTaskAsync(task.ToDto());
                }
            }
        }

        private async void TaskList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not MainViewModel mainViewModel || mainViewModel.SelectedTask == null)
                return;

            var taskViewModel = mainViewModel.SelectedTask;
            var task = taskViewModel.ToDto();

            bool locked = await mainViewModel.LockTaskAsync(task.Id);

            if (!locked)
            {
                MessageBox.Show("This task is currently locked by another user.", "Locked", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var editWindow = new EditTaskWindow(task);
            var result = editWindow.ShowDialog();

            if (result == true)
            {
                await mainViewModel.UpdateTaskAsync(editWindow.EditedTask);
            }

            await mainViewModel.UnlockTaskAsync(task.Id);
        }



    }
}
