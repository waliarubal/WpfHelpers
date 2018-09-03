using NullVoidCreations.WpfHelpers.Base;
using System;
using System.ComponentModel;
using System.Windows;

namespace NullVoidCreations.WpfHelpers.Commands
{
    public class RelayCommand : CommandBase
    {
        readonly Action _action;
        BackgroundWorker _worker;

        public RelayCommand(Action action, bool isEnabled = true)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            _action = action;
            IsEnabled = isEnabled;
        }

        public override void Execute(object parameter)
        {
            IsExecuting = true;

            if (IsSynchronous)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(_action);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    IsExecuting = false;
                }
            }
            else
            {
                if (_worker == null)
                {
                    _worker = new BackgroundWorker();
                    _worker.DoWork += Worker_DoWork;
                    _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                }
                _worker.RunWorkerAsync(_action);
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsExecuting = false;
            if (e.Error != null)
                throw e.Error;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var action = e.Argument as Action;
            action.Invoke();
        }
    }
}
