using System.Windows.Input;

namespace Raketa
{
    public class RaketaCommand : ICommand
    {
        readonly Func<Task>? _executeAsync;
        readonly Action? _executeSync;
        readonly Func<object, bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RaketaCommand(Func<Task> executeAsync, Func<object, bool> canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        public RaketaCommand(Action executeSync, Func<object, bool> canExecute = null)
        {
            _executeSync = executeSync ?? throw new ArgumentNullException(nameof(executeSync));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public async void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                if (_executeAsync != null)
                {
                    await _executeAsync();
                }
                else
                {
                    _executeSync?.Invoke();
                }
            }
        }
    }
}