using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TextAppApi.Core
{
    public class EventStream<T>
    {
        private static IList<EventStream<T>> LongPollingQueue { get; set; } = new List<EventStream<T>>();
        //
        private T _data;
        private Predicate<T> _predicate;
        private int _delaytimeout;
        private TaskCompletionSource<bool> _taskCompletion;

        public EventStream(Predicate<T> predicate, int delay = 15000)
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
                for(int i = LongPollingQueue.Count - 1; i >= 0; i--) // start from the end, otherwise only one event will be called all the time.
                {
                    if (LongPollingQueue[i] is EventStream<T>)
                    {
                        LongPollingQueue[i].CallEventResolve(long_event);
                        //Trace.WriteLine("Resolving event ok...");
                    }
                }
            }
        }
    }
}
