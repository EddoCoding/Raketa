using System.Windows.Input;

namespace Raketa
{
    public class RaketaTCommand<T> : ICommand
    {
        readonly Action<T> _execute;
        readonly Func<T, bool> _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RaketaTCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke((T)parameter) ?? true;

        public void Execute(object? parameter) => _execute?.Invoke((T)parameter);

        public static RaketaTCommand<T> Launch(Action<T> execute, Func<T, bool> canExecute = null)
        => new RaketaTCommand<T>(execute, canExecute);
    }
}