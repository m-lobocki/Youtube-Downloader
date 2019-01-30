using System;
using System.Threading;
using System.Threading.Tasks;

namespace YoutubePlaylist.Wpf.Extensions
{
    public static class TaskExtensions
    {
        public static async Task WithCancellation(this Task task, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (token.Register(source => ((TaskCompletionSource<bool>)source).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task)) throw new OperationCanceledException(token);
            }
            await task;
        }
    }
}
