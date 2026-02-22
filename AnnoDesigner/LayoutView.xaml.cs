using System.Windows.Controls;
using AnnoDesigner.ViewModels;

namespace AnnoDesigner
{
    /// <summary>
    /// Interaction logic for LayoutView.xaml
    /// </summary>
    public partial class LayoutView : UserControl
    {
        public LayoutViewModel Context
        {
            get => DataContext as LayoutViewModel;
            set => DataContext = value;
        }

        public LayoutView()
        {
            InitializeComponent();
        }
    }
}
