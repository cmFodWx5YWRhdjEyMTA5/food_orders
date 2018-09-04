using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HWOrderFood.Helpers
{
    public class SingleExecutionCommand : ICommand
    {
        private Func<object, Task> _func;
        private bool _isExecuting;
        private int _delayMillisec;
        private Func<object, bool> _funcCanExucute;

        private const int DelayMillisec = 400;

        public SingleExecutionCommand()
        {
        }

        public static SingleExecutionCommand FromFunc(Func<Task> func, int delayMillisec = DelayMillisec, Func<object, bool> funcCanExecute = null)
        {
            var ret = new SingleExecutionCommand();
            ret._func = (obj) =>
            {
                return func();
            };
            ret._delayMillisec = delayMillisec;
            ret._funcCanExucute = funcCanExecute;
            return ret;
        }

        public static SingleExecutionCommand FromFunc(Func<object, Task> func, int delayMillisec = DelayMillisec, Func<object, bool> funcCanExecute = null)
        {
            var ret = new SingleExecutionCommand();

            ret._func = func;
            ret._delayMillisec = delayMillisec;
            ret._funcCanExucute = funcCanExecute;

            return ret;
        }

        public static SingleExecutionCommand FromFunc<T>(Func<T, Task> func, int delayMillisec = DelayMillisec, Func<object, bool> funcCanExecute = null)
        {
            var ret = new SingleExecutionCommand();

            ret._func = (object obj) =>
            {
                var objT = default(T);
                objT = (T)obj;
                return func(objT);
            };

            ret._delayMillisec = delayMillisec;
            ret._funcCanExucute = funcCanExecute;

            return ret;
        }

        #region -- ICommand implementation --

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_funcCanExucute == null)
                return true;
            else
                return _funcCanExucute(parameter);
        }

        public async void Execute(object parameter)
        {
            if (_isExecuting)
                return;
            
            _isExecuting = true;

            await _func(parameter);

            if (_delayMillisec > 0)
                await Task.Delay(_delayMillisec);
            
            _isExecuting = false;
        }

        #endregion
    }
}
