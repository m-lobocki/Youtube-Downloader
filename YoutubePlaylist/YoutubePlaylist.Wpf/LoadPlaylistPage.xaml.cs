using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using MahApps.Metro.Controls.Dialogs;

// ReSharper disable PossibleMultipleEnumeration

namespace YoutubePlaylist.Wpf
{
    /// <summary>
    /// Interaction logic for LoadPlaylistPage.xaml
    /// </summary>
    public partial class LoadPlaylistPage
    {
        private readonly MainWindow parentWindow;

        public LoadPlaylistPage(MainWindow parent)
        {
            InitializeComponent();
            parentWindow = parent;
            destinationPathBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            formatBox.ItemsSource = new[]
            {
                Wpf.Clip.Format.Audio,
                Wpf.Clip.Format.Video
            };
        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog { SelectedPath = destinationPathBox.Text };
            if (dialog.ShowDialog() != DialogResult.OK) return;

            destinationPathBox.Text = dialog.SelectedPath;
        }

        private async void nextButton_Click(object sender, RoutedEventArgs e)
        {
            var progressDialog = await parentWindow.ShowProgressAsync("Please wait", "Processing the playlist...");
            progressDialog.SetIndeterminate();

            try
            {
                ToggleControls(false);
                Clip.Format format = (Clip.Format)formatBox.SelectedItem;
                YoutubeEngine youtube = new YoutubeEngine(destinationPathBox.Text, format);
                var playlist = await youtube.ParsePlaylist(playlistUrlBox.Text);
                string destinationDirectory = youtube.DownloadDestinationDirectory;

                if (!Directory.Exists(destinationDirectory))
                {
                    var result = await parentWindow.ShowMessageAsync("Question", "The specified directory does not exist, would you like to create it?",
                        MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative}) ;
                    if (result == MessageDialogResult.Negative) return;
                    else Directory.CreateDirectory(destinationDirectory);
                }
                if (string.IsNullOrEmpty(destinationDirectory)) await parentWindow.ShowMessageAsync("Error", "Specify the directory");
                if (playlist.Any()) parentWindow.TransitioningContentControl.Content = new SelectClipsPage(parentWindow, playlist, youtube);
                else await parentWindow.ShowMessageAsync("Error", "The playlisty is empty");
            }
            catch (Exception exception) when (exception is ArgumentNullException || exception is UriFormatException)
            {
                await parentWindow.ShowMessageAsync("Error", "The url is not valid");
            }
            catch (Exception exception)
            {
                await parentWindow.ShowMessageAsync("Error", exception.Message);
            }
            finally
            {
                ToggleControls(true);
                await progressDialog.CloseAsync();
            }
        }

        private void ToggleControls(bool state)
        {
            destinationPathBox.IsEnabled = state;
            playlistUrlBox.IsEnabled = state;
            browseButton.IsEnabled = state;
            nextButton.IsEnabled = state;
        }
    }
}
