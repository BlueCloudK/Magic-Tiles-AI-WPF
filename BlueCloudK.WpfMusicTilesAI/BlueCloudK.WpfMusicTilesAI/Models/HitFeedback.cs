using CommunityToolkit.Mvvm.ComponentModel;

namespace BlueCloudK.WpfMusicTilesAI.Models
{
    /// <summary>
    /// Represents visual feedback when a note is hit or missed
    /// </summary>
    public partial class HitFeedback : ObservableObject
    {
        [ObservableProperty]
        private string _text = string.Empty;

        [ObservableProperty]
        private string _color = "#ffffff";

        [ObservableProperty]
        private int _lane;

        [ObservableProperty]
        private double _opacity = 1.0;

        [ObservableProperty]
        private double _y;

        public HitFeedback(int lane, string text, string color)
        {
            Lane = lane;
            Text = text;
            Color = color;
            Y = 500; // Start at hit zone
            Opacity = 1.0;
        }
    }
}
