using EzSmb.Shareds.Interfaces;
using System;
using System.Collections.Generic;

namespace EzSmb.Shareds.Bases
{
    public abstract class ErrorManagedBase : IErrorManaged, IDisposable
    {
        private List<string> _errors;
        private bool disposedValue;

        public string[] Errors => this._errors.ToArray();
        public bool HasError => (0 < this._errors.Count);

        public ErrorManagedBase()
        {
            this._errors = new List<string>();
        }

        protected void AddError(string methodName, string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            this._errors.Add($"{DateTime.Now:HH:mm:ss.fff}: [{this.GetType()}.{methodName}] {message}");
        }

        protected void AddError(string methodName, string message, Exception ex)
        {
            if (string.IsNullOrEmpty(message) && ex == null)
                return;

            this.AddError(methodName, $"{message}, Exception.Message: {ex.Message}, Exception.StackTrace: {ex.StackTrace}");
        }

        protected void CopyErrors(IErrorManaged errorManaged)
        {
            if (errorManaged == null || !errorManaged.HasError)
                return;

            var errors = errorManaged.Errors;
            foreach (var message in errors)
                this._errors.Add(message);
        }

        //protected void Dump(string methodName, string message)
        //{
        //    Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: [{this.GetType()}.{methodName}] {message}");
        //}

        //protected void Dump(string methodName, string message, Exception ex)
        //{
        //    this.Dump(methodName, $"{message}, Exception.Message: {ex.Message}, Exception.StackTrace: {ex.StackTrace}");
        //}

        public void ClearErrors()
        {
            this._errors.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this._errors?.Clear();
                    this._errors = null;
                }

                this.disposedValue = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
