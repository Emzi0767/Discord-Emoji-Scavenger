using System;
using System.Threading;
using System.Threading.Tasks;

namespace EmoteScavenger
{
    public class AsyncExecutor
    {
        private SemaphoreSlim Semaphore { get; }

        public AsyncExecutor()
        {
            this.Semaphore = new SemaphoreSlim(1, 1);
        }

        public void Execute<TArg>(Func<TArg, Task> func, TArg arg)
        {
            this.Semaphore.Wait();

            Exception taskex = null;

            var are = new AutoResetEvent(false);
            _ = Task.Run(Executor);
            are.WaitOne();

            this.Semaphore.Release();

            if (taskex != null)
                throw taskex;

            async Task Executor()
            {
                try
                {
                    await func(arg);
                }
                catch (Exception ex)
                {
                    taskex = ex;
                }

                are.Set();
            }
        }

        public TResult Execute<TArg, TResult>(Func<TArg, Task<TResult>> func, TArg arg)
        {
            this.Semaphore.Wait();

            Exception taskex = null;
            TResult result = default;

            var are = new AutoResetEvent(false);
            _ = Task.Run(Executor);
            are.WaitOne();

            this.Semaphore.Release();

            if (taskex != null)
                throw taskex;

            return result;

            async Task Executor()
            {
                try
                {
                    result = await func(arg);
                }
                catch (Exception ex)
                {
                    taskex = ex;
                }

                are.Set();
            }
        }
    }
}
