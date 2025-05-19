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
            if (DataContext is not MainViewModel vm || vm.SelectedTask == null)
                return;

            var task = vm.SelectedTask;

            // ננעל את המשימה
            bool locked = await vm.LockTaskAsync(task.Id);
            if (!locked)
            {
                MessageBox.Show($"Task is currently being edited by: {task.LockedBy}", "Locked", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var editWindow = new EditTaskWindow(task);
            var result = editWindow.ShowDialog();

            if (result == true)
            {
                await vm.UpdateTaskAsync(editWindow.EditedTask);
            }

            await vm.UnlockTaskAsync(task.Id);
        }

    }
}
