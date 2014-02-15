// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System.Diagnostics.Contracts;
using System.Linq.Expressions;

// ReSharper disable once CheckNamespace

namespace System.Threading.Tasks
{
    public static class TaskHelpersExtensions
    {
        private static readonly Task<AsyncVoid> DefaultCompleted = TaskHelpers.FromResult(default(AsyncVoid));
        private static readonly Action<Task> RethrowWithNoStackLossDelegate = GetRethrowWithNoStackLossDelegate();

        public static Task Catch(this Task task, Func<CatchInfo, CatchInfo.CatchResult> continuation,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return task.Status == TaskStatus.RanToCompletion
                ? task
                : task.CatchImpl(() => continuation(new CatchInfo(task, cancellationToken)).Task.ToTask<AsyncVoid>(),
                    cancellationToken);
        }

        public static Task<TResult> Catch<TResult>(this Task<TResult> task,
            Func<CatchInfo<TResult>, CatchInfo<TResult>.CatchResult> continuation,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (task.Status == TaskStatus.RanToCompletion)
            {
                return task;
            }
            return task.CatchImpl(() => continuation(new CatchInfo<TResult>(task, cancellationToken)).Task,
                cancellationToken);
        }

        private static Task<TResult> CatchImpl<TResult>(this Task task, Func<Task<TResult>> continuation,
            CancellationToken cancellationToken)
        {
            if (task.IsCompleted)
            {
                if (task.IsFaulted || task.IsCanceled || cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var resultTask = continuation();
                        if (resultTask == null)
                        {
                            throw new InvalidOperationException(
                                "You must set the Task property of the CatchInfo returned from the TaskHelpersExtensions.Catch continuation.");
                        }

                        return resultTask;
                    }
                    catch (Exception ex)
                    {
                        return TaskHelpers.FromError<TResult>(ex);
                    }
                }

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    var tcs = new TaskCompletionSource<TResult>();
                    tcs.TrySetFromTask(task);
                    return tcs.Task;
                }
            }

            return CatchImplContinuation(task, continuation);
        }

        private static Task<TResult> CatchImplContinuation<TResult>(Task task, Func<Task<TResult>> continuation)
        {
            var syncContext = SynchronizationContext.Current;

            var tcs = new TaskCompletionSource<Task<TResult>>();

            task.ContinueWith(innerTask => tcs.TrySetFromTask(innerTask),
                TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);

            task.ContinueWith(innerTask =>
            {
                if (syncContext != null)
                {
                    syncContext.Post(state =>
                    {
                        try
                        {
                            var resultTask = continuation();
                            if (resultTask == null)
                            {
                                throw new InvalidOperationException(
                                    "You cannot return null from the TaskHelpersExtensions.Catch continuation. You must return a valid task or throw an exception.");
                            }

                            tcs.TrySetResult(resultTask);
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                        }
                    }, null);
                }
                else
                {
                    try
                    {
                        var resultTask = continuation();
                        if (resultTask == null)
                        {
                            throw new InvalidOperationException(
                                "You cannot return null from the TaskHelpersExtensions.Catch continuation. You must return a valid task or throw an exception.");
                        }

                        tcs.TrySetResult(resultTask);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }
            }, TaskContinuationOptions.NotOnRanToCompletion);

            return tcs.Task.FastUnwrap();
        }

        public static Task CopyResultToCompletionSource<TResult>(this Task task, TaskCompletionSource<TResult> tcs,
            TResult completionResult)
        {
            return task.CopyResultToCompletionSourceImpl(tcs, innerTask => completionResult);
        }

        public static Task CopyResultToCompletionSource<TResult>(this Task<TResult> task,
            TaskCompletionSource<TResult> tcs)
        {
            return task.CopyResultToCompletionSourceImpl(tcs, innerTask => innerTask.Result);
        }

        private static Task CopyResultToCompletionSourceImpl<TTask, TResult>(this TTask task,
            TaskCompletionSource<TResult> tcs, Func<TTask, TResult> resultThunk)
            where TTask : Task
        {
            if (task.IsCompleted)
            {
                switch (task.Status)
                {
                    case TaskStatus.Canceled:
                    case TaskStatus.Faulted:
                        tcs.TrySetFromTask(task);
                        break;

                    case TaskStatus.RanToCompletion:
                        tcs.TrySetResult(resultThunk(task));
                        break;
                }

                return TaskHelpers.Completed();
            }

            return CopyResultToCompletionSourceImplContinuation(task, tcs, resultThunk);
        }

        private static Task CopyResultToCompletionSourceImplContinuation<TTask, TResult>(TTask task,
            TaskCompletionSource<TResult> tcs, Func<TTask, TResult> resultThunk)
            where TTask : Task
        {
            return task.ContinueWith(innerTask =>
            {
                switch (innerTask.Status)
                {
                    case TaskStatus.Canceled:
                    case TaskStatus.Faulted:
                        tcs.TrySetFromTask(innerTask);
                        break;

                    case TaskStatus.RanToCompletion:
                        tcs.TrySetResult(resultThunk(task));
                        break;
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
        }


        public static Task<object> CastToObject(this Task task)
        {
            if (task.IsCompleted)
            {
                if (task.IsFaulted)
                {
                    return TaskHelpers.FromErrors<object>(task.Exception.InnerExceptions);
                }
                if (task.IsCanceled)
                {
                    return TaskHelpers.Canceled<object>();
                }
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    return TaskHelpers.FromResult((object) null);
                }
            }

            var tcs = new TaskCompletionSource<object>();

            task.ContinueWith(innerTask =>
            {
                if (innerTask.IsFaulted)
                {
                    tcs.SetException(innerTask.Exception.InnerExceptions);
                }
                else if (innerTask.IsCanceled)
                {
                    tcs.SetCanceled();
                }
                else
                {
                    tcs.SetResult(null);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        public static Task<object> CastToObject<T>(this Task<T> task)
        {
            if (task.IsCompleted)
            {
                if (task.IsFaulted)
                {
                    return TaskHelpers.FromErrors<object>(task.Exception.InnerExceptions);
                }
                if (task.IsCanceled)
                {
                    return TaskHelpers.Canceled<object>();
                }
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    return TaskHelpers.FromResult((object) task.Result);
                }
            }

            var tcs = new TaskCompletionSource<object>();

            task.ContinueWith(innerTask =>
            {
                if (innerTask.IsFaulted)
                {
                    tcs.SetException(innerTask.Exception.InnerExceptions);
                }
                else if (innerTask.IsCanceled)
                {
                    tcs.SetCanceled();
                }
                else
                {
                    tcs.SetResult(innerTask.Result);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        public static Task<TOuterResult> CastFromObject<TOuterResult>(this Task<object> task)
        {
            if (task.IsCompleted)
            {
                if (task.IsFaulted)
                {
                    return TaskHelpers.FromErrors<TOuterResult>(task.Exception.InnerExceptions);
                }
                if (task.IsCanceled)
                {
                    return TaskHelpers.Canceled<TOuterResult>();
                }
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    try
                    {
                        return TaskHelpers.FromResult((TOuterResult) task.Result);
                    }
                    catch (Exception exception)
                    {
                        return TaskHelpers.FromError<TOuterResult>(exception);
                    }
                }
            }

            var tcs = new TaskCompletionSource<TOuterResult>();

            task.ContinueWith(innerTask =>
            {
                if (innerTask.IsFaulted)
                {
                    tcs.SetException(innerTask.Exception.InnerExceptions);
                }
                else if (innerTask.IsCanceled)
                {
                    tcs.SetCanceled();
                }
                else
                {
                    try
                    {
                        tcs.SetResult((TOuterResult) innerTask.Result);
                    }
                    catch (Exception exception)
                    {
                        tcs.SetException(exception);
                    }
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        public static Task FastUnwrap(this Task<Task> task)
        {
            var innerTask = task.Status == TaskStatus.RanToCompletion ? task.Result : null;
            return innerTask ?? task.Unwrap();
        }

        public static Task<TResult> FastUnwrap<TResult>(this Task<Task<TResult>> task)
        {
            var innerTask = task.Status == TaskStatus.RanToCompletion ? task.Result : null;
            return innerTask ?? task.Unwrap();
        }

        public static Task Finally(this Task task, Action continuation, bool runSynchronously = false)
        {
            if (task.IsCompleted)
            {
                try
                {
                    continuation();
                    return task;
                }
                catch (Exception ex)
                {
                    MarkExceptionsObserved(task);
                    return TaskHelpers.FromError(ex);
                }
            }

            return FinallyImplContinuation<AsyncVoid>(task, continuation, runSynchronously);
        }

        public static Task<TResult> Finally<TResult>(this Task<TResult> task, Action continuation,
            bool runSynchronously = false)
        {
            if (task.IsCompleted)
            {
                try
                {
                    continuation();
                    return task;
                }
                catch (Exception ex)
                {
                    MarkExceptionsObserved(task);
                    return TaskHelpers.FromError<TResult>(ex);
                }
            }

            return FinallyImplContinuation<TResult>(task, continuation, runSynchronously);
        }

        private static Task<TResult> FinallyImplContinuation<TResult>(Task task, Action continuation,
            bool runSynchronously = false)
        {
            var syncContext = SynchronizationContext.Current;

            var tcs = new TaskCompletionSource<TResult>();

            task.ContinueWith(innerTask =>
            {
                try
                {
                    if (syncContext != null)
                    {
                        syncContext.Post(state =>
                        {
                            try
                            {
                                continuation();
                                tcs.TrySetFromTask(innerTask);
                            }
                            catch (Exception ex)
                            {
                                MarkExceptionsObserved(innerTask);
                                tcs.SetException(ex);
                            }
                        }, null);
                    }
                    else
                    {
                        continuation();
                        tcs.TrySetFromTask(innerTask);
                    }
                }
                catch (Exception ex)
                {
                    MarkExceptionsObserved(innerTask);
                    tcs.TrySetException(ex);
                }
            }, runSynchronously ? TaskContinuationOptions.ExecuteSynchronously : TaskContinuationOptions.None);

            return tcs.Task;
        }


        private static Action<Task> GetRethrowWithNoStackLossDelegate()
        {
#if NETFX_CORE
            return task => task.GetAwaiter().GetResult();
#else
            var getAwaiterMethod = typeof (Task).GetMethod("GetAwaiter", Type.EmptyTypes);
            if (getAwaiterMethod != null)
            {
                var taskParameter = Expression.Parameter(typeof (Task));
                var getAwaiterCall = Expression.Call(taskParameter, getAwaiterMethod);
                var getResultCall = Expression.Call(getAwaiterCall, "GetResult", Type.EmptyTypes);
                var lambda = Expression.Lambda<Action<Task>>(getResultCall, taskParameter);
                return lambda.Compile();
            }
            Func<Exception, Exception> prepForRemoting = null;

            try
            {
                if (AppDomain.CurrentDomain.IsFullyTrusted)
                {
                    var exceptionParameter = Expression.Parameter(typeof (Exception));
                    var prepForRemotingCall = Expression.Call(exceptionParameter, "PrepForRemoting", Type.EmptyTypes);
                    var lambda = Expression.Lambda<Func<Exception, Exception>>(prepForRemotingCall, exceptionParameter);
                    var func = lambda.Compile();
                    func(new Exception());
                    prepForRemoting = func;
                }
            }
            catch
            {
            }
            return task =>
            {
                try
                {
                    task.Wait();
                }
                catch (AggregateException ex)
                {
                    var baseException = ex.GetBaseException();
                    if (prepForRemoting != null)
                    {
                        baseException = prepForRemoting(baseException);
                    }
                    throw baseException;
                }
            };
#endif
        }

        private static void MarkExceptionsObserved(this Task task)
        {
            Contract.Assert(task.IsCompleted);

            Exception unused = task.Exception;
        }

        public static Task Then(this Task task, Action continuation,
            CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false)
        {
            return task.ThenImpl(t => ToAsyncVoidTask(continuation), cancellationToken, runSynchronously);
        }

        public static Task<TOuterResult> Then<TOuterResult>(this Task task, Func<TOuterResult> continuation,
            CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false)
        {
            return task.ThenImpl(t => TaskHelpers.FromResult(continuation()), cancellationToken, runSynchronously);
        }

        public static Task Then(this Task task, Func<Task> continuation,
            CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false)
        {
            return task.Then(() => continuation().Then(() => default(AsyncVoid)),
                cancellationToken, runSynchronously);
        }

        public static Task<TOuterResult> Then<TOuterResult>(this Task task, Func<Task<TOuterResult>> continuation,
            CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false)
        {
            return task.ThenImpl(t => continuation(), cancellationToken, runSynchronously);
        }

        public static Task Then<TInnerResult>(this Task<TInnerResult> task, Action<TInnerResult> continuation,
            CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false)
        {
            return task.ThenImpl(t => ToAsyncVoidTask(() => continuation(t.Result)), cancellationToken, runSynchronously);
        }

        public static Task<TOuterResult> Then<TInnerResult, TOuterResult>(this Task<TInnerResult> task,
            Func<TInnerResult, TOuterResult> continuation,
            CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false)
        {
            return task.ThenImpl(t => TaskHelpers.FromResult(continuation(t.Result)), cancellationToken,
                runSynchronously);
        }


        public static Task Then<TInnerResult>(this Task<TInnerResult> task, Func<TInnerResult, Task> continuation,
            CancellationToken token = default(CancellationToken), bool runSynchronously = false)
        {
            return task.ThenImpl(t => continuation(t.Result).ToTask<AsyncVoid>(), token, runSynchronously);
        }

        public static Task<TOuterResult> Then<TInnerResult, TOuterResult>(this Task<TInnerResult> task,
            Func<TInnerResult, Task<TOuterResult>> continuation,
            CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false)
        {
            return task.ThenImpl(t => continuation(t.Result), cancellationToken, runSynchronously);
        }

        private static Task<TOuterResult> ThenImpl<TTask, TOuterResult>(this TTask task,
            Func<TTask, Task<TOuterResult>> continuation, CancellationToken cancellationToken, bool runSynchronously)
            where TTask : Task
        {
            if (task.IsCompleted)
            {
                if (task.IsFaulted)
                {
                    return TaskHelpers.FromErrors<TOuterResult>(task.Exception.InnerExceptions);
                }
                if (task.IsCanceled || cancellationToken.IsCancellationRequested)
                {
                    return TaskHelpers.Canceled<TOuterResult>();
                }
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    try
                    {
                        return continuation(task);
                    }
                    catch (Exception ex)
                    {
                        return TaskHelpers.FromError<TOuterResult>(ex);
                    }
                }
            }

            return ThenImplContinuation(task, continuation, cancellationToken, runSynchronously);
        }

        private static Task<TOuterResult> ThenImplContinuation<TOuterResult, TTask>(TTask task,
            Func<TTask, Task<TOuterResult>> continuation, CancellationToken cancellationToken,
            bool runSynchronously = false)
            where TTask : Task
        {
            var syncContext = SynchronizationContext.Current;

            var tcs = new TaskCompletionSource<Task<TOuterResult>>();

            task.ContinueWith(innerTask =>
            {
                if (innerTask.IsFaulted)
                {
                    tcs.TrySetException(innerTask.Exception.InnerExceptions);
                }
                else if (innerTask.IsCanceled || cancellationToken.IsCancellationRequested)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    if (syncContext != null)
                    {
                        syncContext.Post(state =>
                        {
                            try
                            {
                                tcs.TrySetResult(continuation(task));
                            }
                            catch (Exception ex)
                            {
                                tcs.TrySetException(ex);
                            }
                        }, null);
                    }
                    else
                    {
                        tcs.TrySetResult(continuation(task));
                    }
                }
            }, runSynchronously ? TaskContinuationOptions.ExecuteSynchronously : TaskContinuationOptions.None);

            return tcs.Task.FastUnwrap();
        }

        public static void ThrowIfFaulted(this Task task)
        {
            RethrowWithNoStackLossDelegate(task);
        }

        private static Task<AsyncVoid> ToAsyncVoidTask(Action action)
        {
            return TaskHelpers.RunSynchronously(() =>
            {
                action();
                return DefaultCompleted;
            });
        }

        public static Task<TResult> ToTask<TResult>(this Task task,
            CancellationToken cancellationToken = default(CancellationToken), TResult result = default(TResult))
        {
            if (task == null)
            {
                return null;
            }

            if (task.IsCompleted)
            {
                if (task.IsFaulted)
                {
                    return TaskHelpers.FromErrors<TResult>(task.Exception.InnerExceptions);
                }
                if (task.IsCanceled || cancellationToken.IsCancellationRequested)
                {
                    return TaskHelpers.Canceled<TResult>();
                }
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    return TaskHelpers.FromResult(result);
                }
            }

            return ToTaskContinuation(task, result);
        }

        private static Task<TResult> ToTaskContinuation<TResult>(Task task, TResult result)
        {
            var tcs = new TaskCompletionSource<TResult>();

            task.ContinueWith(innerTask =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    tcs.TrySetResult(result);
                }
                else
                {
                    tcs.TrySetFromTask(innerTask);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        public static bool TryGetResult<TResult>(this Task<TResult> task, out TResult result)
        {
            if (task.Status == TaskStatus.RanToCompletion)
            {
                result = task.Result;
                return true;
            }

            result = default(TResult);
            return false;
        }

        private struct AsyncVoid
        {
        }
    }

    public abstract class CatchInfoBase<TTask>
        where TTask : Task
    {
        private readonly Exception exception;
        private readonly TTask task;

        protected CatchInfoBase(TTask task, CancellationToken cancellationToken)
        {
            Contract.Assert(task != null);
            this.task = task;
            if (task.IsFaulted)
            {
                exception = this.task.Exception.GetBaseException();
            }
            else if (task.IsCanceled)
            {
                exception = new TaskCanceledException(task);
            }
            else
            {
                System.Diagnostics.Debug.Assert(cancellationToken.IsCancellationRequested);
                exception = new OperationCanceledException(cancellationToken);
            }
        }

        protected TTask Task
        {
            get { return task; }
        }

        public Exception Exception
        {
            get { return exception; }
        }

        public struct CatchResult
        {
            internal TTask Task { get; set; }
        }
    }

    public class CatchInfo : CatchInfoBase<Task>
    {
        private static readonly CatchResult Completed = new CatchResult {Task = TaskHelpers.Completed()};

        public CatchInfo(Task task, CancellationToken cancellationToken)
            : base(task, cancellationToken)
        {
        }


        public CatchResult Handled()
        {
            return Completed;
        }

        public CatchResult Task(Task task)
        {
            return new CatchResult {Task = task};
        }

        public CatchResult Throw()
        {
            if (base.Task.IsFaulted || base.Task.IsCanceled)
            {
                return new CatchResult {Task = base.Task};
            }
            // Canceled via CancelationToken
            return new CatchResult {Task = TaskHelpers.Canceled()};
        }

        public CatchResult Throw(Exception ex)
        {
            return new CatchResult {Task = TaskHelpers.FromError<object>(ex)};
        }
    }


    public class CatchInfo<T> : CatchInfoBase<Task<T>>
    {
        public CatchInfo(Task<T> task, CancellationToken cancellationToken)
            : base(task, cancellationToken)
        {
        }

        public CatchResult Handled(T returnValue)
        {
            return new CatchResult {Task = TaskHelpers.FromResult(returnValue)};
        }


        public CatchResult Task(Task<T> task)
        {
            return new CatchResult {Task = task};
        }

        public CatchResult Throw()
        {
            if (base.Task.IsFaulted || base.Task.IsCanceled)
            {
                return new CatchResult {Task = base.Task};
            }
            return new CatchResult {Task = TaskHelpers.Canceled<T>()};
        }

        public CatchResult Throw(Exception ex)
        {
            return new CatchResult {Task = TaskHelpers.FromError<T>(ex)};
        }
    }
}