using MahApps.Metro.Controls;

namespace YoutubePlaylist.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            TransitioningContentControl.Content = new LoadPlaylistPage(this);
        }

        public TransitioningContentControl TransitioningContentControl => transitioningContentControl;
    }
}
