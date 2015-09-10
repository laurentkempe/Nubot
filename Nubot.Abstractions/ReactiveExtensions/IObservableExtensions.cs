namespace Nubot.Abstractions.ReactiveExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    // ReSharper disable once InconsistentNaming
    public static class IObservableExtensions
    {
        public static IObservable<IList<T>> BufferUntilInactive<T>(this IObservable<T> stream, TimeSpan delay, IScheduler scheduler, int? max = null)
        {
            var closes = stream.Throttle(delay, scheduler);
            if (max != null)
            {
                var overflows = stream.Where((x, index) => index + 1 >= max);
                closes = closes.Merge(overflows);
            }
            return stream.Window(() => closes).SelectMany(window => window.ToList());
        }
    }
}