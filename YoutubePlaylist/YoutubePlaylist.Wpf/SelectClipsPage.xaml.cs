using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace YoutubePlaylist.Wpf
{
    /// <summary>
    /// Interaction logic for DownloadPlaylistPage.xaml
    /// </summary>
    public partial class SelectClipsPage
    {
        private readonly MainWindow parentWindow;
        private readonly YoutubeEngine youtube;

        public SelectClipsPage(MainWindow parentWindow, IEnumerable<Clip> playlist, YoutubeEngine youtube)
        {
            InitializeComponent();
            this.parentWindow = parentWindow;
            this.youtube = youtube;
            videoListBox.ItemsSource = playlist;
            headerText.Text = string.Format(headerText.Text, youtube.Format.ToString().ToLower());
        }

        private void videoListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e?.AddedItems?.Count > 0)
            {
                Clip item = e.AddedItems[0] as Clip;
                if (item != null) item.IsChecked = !item.IsChecked;
            }
            
            videoListBox.SelectedIndex = -1;
            videoListBox.Items.Refresh();
        }

        private void checkAllButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleIsChecked(true);
        }

        private void uncheckAllButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleIsChecked(false);
        }

        private void ToggleIsChecked(bool state)
        {
            foreach (var item in videoListBox.Items)
            {
                ((Clip)item).IsChecked = state;
            }

            videoListBox.Items.Refresh();
        }

        private async void nextButton_Click(object sender, RoutedEventArgs e)
        {
            var clips = videoListBox.Items.Cast<Clip>().Where(clip => clip.IsChecked);

            if (clips.Any()) parentWindow.TransitioningContentControl.Content = new DownloadPlaylistPage(parentWindow, youtube, clips);
            else await parentWindow.ShowMessageAsync("Error", $"Minimum 1 {youtube.Format.ToString().ToLower()} must be selected");
        }
    }
}
