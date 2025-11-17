using BlueCloudK.WpfMusicTilesAI.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlueCloudK.WpfMusicTilesAI.Views
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        public GameView()
        {
            InitializeComponent();
            Loaded += (s, e) => Focus(); // Ensure the control can receive key events
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is GameViewModel viewModel)
            {
                int lane = e.Key switch
                {
                    Key.D => 1,
                    Key.F => 2,
                    Key.J => 3,
                    Key.K => 4,
                    _ => 0
                };

                if (lane > 0)
                {
                    viewModel.HandleKeyPress(lane);
                }
            }
        }
    }
}
