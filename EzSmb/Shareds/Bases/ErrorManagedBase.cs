using EzSmb.Shareds.Interfaces;
using System;
using System.Collections.Generic;

namespace EzSmb.Shareds.Bases
{
    /// <summary>
    /// Abstract class with error string
    /// </summary>
    public abstract class ErrorManagedBase : IErrorManaged, IDisposable
    {
        private List<string> _errors;
        private bool disposedValue;

        /// <summary>
        /// Error string array
        /// </summary>
        public string[] Errors => this._errors.ToArray();

        /// <summary>
        /// Error flag
        /// </summary>
        public bool HasError => (0 < this._errors.Count);

        /// <summary>
        /// Constructor
        /// </summary>
        public ErrorManagedBase()
        {
            this._errors = new List<string>();
        }

        /// <summary>
        /// Add error string
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="message"></param>
        protected void AddError(string methodName, string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            this._errors.Add($"{DateTime.Now:HH:mm:ss.fff}: [{this.GetType()}.{methodName}] {message}");
        }

        /// <summary>
        /// Add error string
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        protected void AddError(string methodName, string message, Exception ex)
        {
            if (string.IsNullOrEmpty(message) && ex == null)
                return;

            this.AddError(methodName, $"{message}, Exception.Message: {ex.Message}, Exception.StackTrace: {ex.StackTrace}");
        }

        /// <summary>
        /// Add error string from IErrorManaged
        /// </summary>
        /// <param name="errorManaged"></param>
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

        /// <summary>
        /// Clear error strings
        /// </summary>
        public void ClearErrors()
        {
            this._errors.Clear();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
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

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
