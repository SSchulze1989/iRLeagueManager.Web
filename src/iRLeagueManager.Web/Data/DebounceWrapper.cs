using System.Collections.Concurrent;

namespace iRLeagueManager.Web.Data;

public static class DebounceWrapper
{
    public static Action Debounce(this Action action, int milliseconds)
    {
        CancellationTokenSource? cancelTokenSource = null;

        return () =>
        {
            cancelTokenSource?.Cancel();
            cancelTokenSource = new CancellationTokenSource();

            Task.Delay(milliseconds, cancelTokenSource.Token)
                .ContinueWith(t =>
                {
                    if (t.IsCompletedSuccessfully)
                    {
                        action.Invoke();
                    }
                }, TaskScheduler.Default);
        };
    }

    public static Func<Task> Debounce(this Func<Task> handler, int milliseconds)
    {
        CancellationTokenSource? cancelTokenSource = null;

        return () =>
        {
            cancelTokenSource?.Cancel();
            cancelTokenSource = new CancellationTokenSource();

            return Task.Delay(milliseconds, cancelTokenSource.Token)
                .ContinueWith(async t =>
                {
                    if (t.IsCompletedSuccessfully)
                    {
                        await handler.Invoke();
                    }
                }, TaskScheduler.Default);
        };
    }
}
