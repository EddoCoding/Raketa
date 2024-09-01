using System.Windows.Input;

namespace Raketa
{
    public class RaketaTCommand<T> : ICommand
    {
        readonly Action<T>? _executeSync;
        readonly Func<T, Task>? _executeAsync;
        readonly Func<T, bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RaketaTCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _executeSync = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RaketaTCommand(Func<T, Task> execute, Func<T, bool> canExecute = null)
        {
            _executeAsync = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke((T)parameter) ?? true;

        public async void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                if (_executeAsync != null)
                {
                    await _executeAsync((T)parameter);
                }
                else
                {
                    _executeSync?.Invoke((T)parameter);
                }
            }
        }

        public static RaketaTCommand<T> Launch(Action<T> execute, Func<T, bool> canExecute = null) =>
            new RaketaTCommand<T>(execute, canExecute);

        public static RaketaTCommand<T> Launch(Func<T, Task> execute, Func<T, bool> canExecute = null) =>
            new RaketaTCommand<T>(execute, canExecute);
    }
}