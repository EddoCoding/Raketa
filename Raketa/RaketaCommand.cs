using System.Windows.Input;

namespace Raketa.Commands
{
    public class RaketaCommand : ICommand
    {
        readonly Action _execeute;
        readonly Func<object, bool> _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RaketaCommand(Action execute, Func<object, bool> canExecute = null)
        {
            _execeute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execeute?.Invoke();

        public static RaketaCommand Launch(Action execute, Func<object, bool> canExecute = null)
            => new RaketaCommand(execute, canExecute);
    }
}