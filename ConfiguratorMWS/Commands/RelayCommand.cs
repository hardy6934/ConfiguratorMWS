 using System.Windows.Input;

namespace ConfiguratorMWS.Commands
{
    public class RelayCommand : ICommand
    {
        //private Action<object> execute;
        //private Func<object, bool> canExecute;

        //public event EventHandler CanExecuteChanged
        //{
        //    add { CommandManager.RequerySuggested += value; }
        //    remove { CommandManager.RequerySuggested -= value; }
        //}

        //public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        //{
        //    this.execute = execute;
        //    this.canExecute = canExecute;
        //}

        //public bool CanExecute(object parameter)
        //{
        //    return this.canExecute == null || this.canExecute(parameter);
        //}

        //public void Execute(object parameter)
        //{
        //    this.execute(parameter);
        //}



        private readonly Func<object, Task> _executeAsync;
        private readonly Action<object> _executeSync;
        private readonly Func<object, bool> _canExecute;
        private bool _isExecuting;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Конструктор для асинхронного метода
        public RelayCommand(Func<object, Task> executeAsync, Func<object, bool> canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        // Конструктор для синхронного метода
        public RelayCommand(Action<object> executeSync, Func<object, bool> canExecute = null)
        {
            _executeSync = executeSync ?? throw new ArgumentNullException(nameof(executeSync));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && (_canExecute == null || _canExecute(parameter));
        }

        public async void Execute(object parameter)
        {
            if (_executeAsync != null)
            {
                await ExecuteAsync(parameter);
            }
            else if (_executeSync != null)
            {
                ExecuteSync(parameter);
            }
        }

        private async Task ExecuteAsync(object parameter)
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();

            try
            {
                await _executeAsync(parameter);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        private void ExecuteSync(object parameter)
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();

            try
            {
                _executeSync(parameter);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }



    }
}