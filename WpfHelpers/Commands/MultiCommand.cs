using NullVoidCreations.WpfHelpers.Base;
using NullVoidCreations.WpfHelpers.DataStructures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace NullVoidCreations.WpfHelpers.Commands
{
    public class MultiCommand : CommandBase
    {
        BackgroundWorker _worker;
        int _waitInterval;

        public MultiCommand(bool isEnabled = true, int waitInterval = 10)
        {
            IsSynchronous = false;
            IsEnabled = isEnabled;
            WaitInterval = waitInterval;
        }

        #region properties

        public int WaitInterval
        {
            get => _waitInterval;
            set => Set(nameof(WaitInterval), ref _waitInterval, value);
        }

        #endregion

        public override void Execute(object parameter)
        {
            var entries = parameter as List<object>;
            if (entries == null)
                throw new ArgumentException("Commands and their parameters not supplied.", nameof(parameter));

            IsExecuting = true;

            var commands = new List<Doublet<CommandBase, object>>();
            foreach(var entry in entries)
            {
                var command = entry as CommandBase;
                if (command == null)
                {
                    if (commands.Count == 0)
                        continue;

                    commands[commands.Count - 1].Second = entry;
                }
                else
                    commands.Add(new Doublet<CommandBase, object>(command, null));
            }

            if (_worker == null)
            {
                _worker = new BackgroundWorker();
                _worker.DoWork += Worker_DoWork;
                _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            }
            _worker.RunWorkerAsync(commands);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsExecuting = false;
            if (e.Error != null)
                throw e.Error;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var commandStack = e.Argument as List<KeyValuePair<CommandBase, object>>;
            foreach(var commandEntry in commandStack)
            {
                var command = commandEntry.Key;
                var parameter = commandEntry.Value;
                if (command.IsSynchronous)
                    command.Execute(parameter);
                else
                {
                    command.Execute(parameter);
                    while (command.IsExecuting)
                        Thread.Sleep(WaitInterval);
                }
            }
        }
    }
}
