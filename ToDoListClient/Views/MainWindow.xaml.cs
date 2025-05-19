using System.Windows;
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

        private async void TaskList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not MainViewModel mainViewModel || mainViewModel.SelectedTask == null)
                return;

            var taskVM = mainViewModel.SelectedTask;
            var task = taskVM.ToDto();

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
