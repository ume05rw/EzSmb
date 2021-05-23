using System;

namespace EzSmb.Shareds
{
    internal class Locker
    {
        public bool IsLocked { get; set; }

        public void LockedInvoke(Action action)
        {
            Exception exception = null;

            lock (this)
            {
                this.IsLocked = true;

                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                finally
                {
                    this.IsLocked = false;
                }
            }

            if (exception != null)
                throw exception;
        }

        public TResult LockedInvoke<TResult>(Func<TResult> func)
        {
            TResult result = default;
            Exception exception = null;

            lock (this)
            {
                this.IsLocked = true;

                try
                {
                    result = func.Invoke();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                finally
                {
                    this.IsLocked = false;
                }
            }

            if (exception != null)
                throw exception;

            return result;
        }
    }
}
