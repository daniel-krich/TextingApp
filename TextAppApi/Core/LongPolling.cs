using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TextAppApi.Core
{
    public class LongPolling<T>
    {
        private static IList<LongPolling<T>> LongPollingQueue { get; set; } = new List<LongPolling<T>>();
        //
        private T _data;
        private Predicate<T> _predicate;
        private int _delaytimeout;
        private TaskCompletionSource<bool> _taskCompletion;

        public LongPolling(Predicate<T> predicate, int delay)
        {
            _predicate = predicate;
            _delaytimeout = delay;
            _taskCompletion = new TaskCompletionSource<bool>();
            lock (LongPollingQueue)
            {
                LongPollingQueue.Add(this);
            }
        }

        private void CallEventResolve(T data)
        {
            if (_predicate(data))
            {
                this._data = data;
                this._taskCompletion.SetResult(true);
            }
        }

        public async Task<T> ResolveEvent()
        {
            await Task.WhenAny(this._taskCompletion.Task, Task.Delay(this._delaytimeout));
            lock (LongPollingQueue)
            {
                LongPollingQueue.Remove(this);
            }
            return this._data;
        }

        //

        public static void CallEvent(T long_event)
        {
            lock(LongPollingQueue)
            {
                for(int i = 0; i < LongPollingQueue.Count; i++)
                {
                    if (LongPollingQueue[i] is LongPolling<T>)
                    {
                        LongPollingQueue[i].CallEventResolve(long_event);
                        Trace.WriteLine("Resolving event ok...");
                    }
                }
            }
        }
    }
}
