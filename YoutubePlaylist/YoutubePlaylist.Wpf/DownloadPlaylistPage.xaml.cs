using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using YoutubePlaylist.Wpf.Extensions;
using YoutubePlaylist.Wpf.MetadataProviders;

namespace YoutubePlaylist.Wpf
{
    /// <summary>
    /// Interaction logic for DownloadPlaylistPage.xaml
    /// </summary>
    public partial class DownloadPlaylistPage
    {
        private readonly MainWindow parentWindow;
        private readonly YoutubeEngine youtube;
        private readonly IMetadataProvider metadataProvider = new LastFmAudioMetadataProvider();
        private readonly IEnumerable<Clip> clips;
        private CancellationTokenSource cancellationTokenSource;
        private readonly int clipsCount;
        private int downloadCount;
        private int skippedCount;

        public DownloadPlaylistPage(MainWindow parentWindow, YoutubeEngine youtube, IEnumerable<Clip> clips)
        {
            InitializeComponent();
            this.parentWindow = parentWindow;
            this.youtube = youtube;
            this.clips = clips;
            clipsCount = clips.Count();

            youtube.GeneratingUrl += (s, e) => UpdateDownloadStatus(0, "Generating url...");
            youtube.PlaylistDownloadProgressChanged += (s, e) =>
                UpdateDownloadStatus(e?.DownloadProgressEventArgs?.ProgressPercentage ?? 0, $"Downloading \"{e.FileName}\"");
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Clip clip in clips)
            {
                bool success = await TryDownload(clip);
                if (!success) break;
            }

            ResetStatus();
            parentWindow.WindowState = WindowState.Normal;
            parentWindow.Activate();
            string message = "Finished downloading";
            if (skippedCount > 0) message += $". Skipped {skippedCount}/{clips.Count()} files, because found them in the output directory.";
            await parentWindow.ShowMessageAsync("Download", message);
            parentWindow.Close();
        }

        /// <summary>
        /// Try to download a clip
        /// </summary>
        /// <param name="clip">The clip</param>
        /// <returns>Whether success</returns>
        private async Task<bool> TryDownload(Clip clip)
        {
            bool success = true;

            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                await youtube.Download(clip).WithCancellation(cancellationTokenSource.Token);
                await metadataProvider.DownloadDataTo(clip.FilePath);
                downloadCount++;
            }
            catch (DownloadSkippedException)
            {
                skippedCount++;
                downloadCount++;
            }
            catch (OperationCanceledException)
            {
                success = false;
            }
            catch (Exception exception)
            {
                if (await AskWhetherUserWantToCancel(exception, clip.Title)) success = false;
            }

            return success;
        }

        private async Task<bool> AskWhetherUserWantToCancel(Exception exception, string clipTitle)
        {
            MessageDialogResult result = await parentWindow.ShowMessageAsync(
                       "Error", $"Cannot download '{clipTitle}': " + exception.Message + "\nWould you like to cancel?",
                       MessageDialogStyle.AffirmativeAndNegative,
                       new MetroDialogSettings { AffirmativeButtonText = "Cancel", NegativeButtonText = "Continue downloading", DefaultButtonFocus = MessageDialogResult.Affirmative });
            return result == MessageDialogResult.Affirmative;
        }

        // ReSharper disable once ParameterHidesMember
        private void UpdateDownloadStatus(double currentProgress, string currentlyDownloading, double? overallProgress = null, string statusText = null)
        {
            currentDownloadProgress.Value = currentProgress;
            currentlyDownloadingText.Text = currentlyDownloading;

            if (overallProgress == null) overallDownloadProgress.Value = downloadCount * 100.0 / clipsCount;
            else overallDownloadProgress.Value = overallProgress.Value;

            this.statusText.Text = statusText ?? $"Downloaded {downloadCount} / {clipsCount}";
        }

        private void ResetStatus()
        {
            UpdateDownloadStatus(0, "", 0, "");
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }
    }
}
