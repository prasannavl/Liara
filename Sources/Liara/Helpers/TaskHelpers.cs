// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System.Collections.Generic;
using System.Diagnostics.Contracts;

// ReSharper disable once CheckNamespace

namespace System.Threading.Tasks
{
    public static class TaskHelpers
    {
        private static readonly Task DefaultCompleted = FromResult(default(AsyncVoid));

        private static readonly Task<object> CompletedTaskReturningNull = FromResult<object>(null);

        public static Task Canceled()
        {
            return CancelCache<AsyncVoid>.Canceled;
        }

        public static Task<TResult> Canceled<TResult>()
        {
            return CancelCache<TResult>.Canceled;
        }

        public static Task Completed()
        {
            return DefaultCompleted;
        }

        public static Task FromError(Exception exception)
        {
            return FromError<AsyncVoid>(exception);
        }

        public static Task<TResult> FromError<TResult>(Exception exception)
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetException(exception);
            return tcs.Task;
        }

        public static Task FromErrors(IEnumerable<Exception> exceptions)
        {
            return FromErrors<AsyncVoid>(exceptions);
        }

        public static Task<TResult> FromErrors<TResult>(IEnumerable<Exception> exceptions)
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetException(exceptions);
            return tcs.Task;
        }

        public static Task<TResult> FromResult<TResult>(TResult result)
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetResult(result);
            return tcs.Task;
        }

        public static Task<object> NullResult()
        {
            return CompletedTaskReturningNull;
        }

        public static Task Iterate(IEnumerable<Task> asyncIterator,
            CancellationToken cancellationToken = default(CancellationToken), bool disposeEnumerator = true)
        {
            Contract.Assert(asyncIterator != null);

            try
            {
                var enumerator = asyncIterator.GetEnumerator();
                var task = IterateImpl(enumerator, cancellationToken);
                return (disposeEnumerator) ? task.Finally(enumerator.Dispose, true) : task;
            }
            catch (Exception ex)
            {
                return TaskHelpers.FromError(ex);
            }
        }

        public static Task IterateImpl(IEnumerator<Task> enumerator, CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return TaskHelpers.Canceled();
                    }

                    if (!enumerator.MoveNext())
                    {
                        return TaskHelpers.Completed();
                    }

                    var currentTask = enumerator.Current;
                    if (currentTask.Status == TaskStatus.RanToCompletion)
                    {
                        continue;
                    }

                    if (currentTask.IsCanceled || currentTask.IsFaulted)
                    {
                        return currentTask;
                    }

                    return IterateImplIncompleteTask(enumerator, currentTask, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                return TaskHelpers.FromError(ex);
            }
        }


        public static Task IterateImplIncompleteTask(IEnumerator<Task> enumerator, Task currentTask,
            CancellationToken cancellationToken)
        {
            return currentTask.Then(() => IterateImpl(enumerator, cancellationToken));
        }

        public static Task RunSynchronously(Action action, CancellationToken token = default(CancellationToken))
        {
            if (token.IsCancellationRequested)
            {
                return Canceled();
            }

            try
            {
                action();
                return Completed();
            }
            catch (Exception e)
            {
                return FromError(e);
            }
        }

        public static Task<TResult> RunSynchronously<TResult>(Func<TResult> func,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Canceled<TResult>();
            }

            try
            {
                return FromResult(func());
            }
            catch (Exception e)
            {
                return FromError<TResult>(e);
            }
        }

        public static Task<TResult> RunSynchronously<TResult>(Func<Task<TResult>> func,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Canceled<TResult>();
            }

            try
            {
                return func();
            }
            catch (Exception e)
            {
                return FromError<TResult>(e);
            }
        }

        public static bool SetIfTaskFailed<TResult>(this TaskCompletionSource<TResult> tcs, Task source)
        {
            switch (source.Status)
            {
                case TaskStatus.Canceled:
                case TaskStatus.Faulted:
                    return tcs.TrySetFromTask(source);
            }

            return false;
        }

        public static bool TrySetFromTask<TResult>(this TaskCompletionSource<TResult> tcs, Task source)
        {
            if (source.Status == TaskStatus.Canceled)
            {
                return tcs.TrySetCanceled();
            }

            if (source.Status == TaskStatus.Faulted)
            {
                return tcs.TrySetException(source.Exception.InnerExceptions);
            }

            if (source.Status == TaskStatus.RanToCompletion)
            {
                var taskOfResult = source as Task<TResult>;
                return tcs.TrySetResult(taskOfResult == null ? default(TResult) : taskOfResult.Result);
            }

            return false;
        }

        public static bool TrySetFromTask<TResult>(this TaskCompletionSource<Task<TResult>> tcs, Task source)
        {
            if (source.Status == TaskStatus.Canceled)
            {
                return tcs.TrySetCanceled();
            }

            if (source.Status == TaskStatus.Faulted)
            {
                return tcs.TrySetException(source.Exception.InnerExceptions);
            }

            if (source.Status == TaskStatus.RanToCompletion)
            {
                var taskOfTaskOfResult = source as Task<Task<TResult>>;
                if (taskOfTaskOfResult != null)
                {
                    return tcs.TrySetResult(taskOfTaskOfResult.Result);
                }

                var taskOfResult = source as Task<TResult>;
                if (taskOfResult != null)
                {
                    return tcs.TrySetResult(taskOfResult);
                }

                return tcs.TrySetResult(TaskHelpers.FromResult(default(TResult)));
            }

            return false;
        }

        private struct AsyncVoid
        {
        }

        private static class CancelCache<TResult>
        {
            public static readonly Task<TResult> Canceled = GetCancelledTask();

            private static Task<TResult> GetCancelledTask()
            {
                var tcs = new TaskCompletionSource<TResult>();
                tcs.SetCanceled();
                return tcs.Task;
            }
        }
    }
}