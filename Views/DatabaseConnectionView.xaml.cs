using System.Windows;
using System.Windows.Controls;
using SyncCore.ViewModels;

namespace SyncCore.Views
{
    /// <summary>
    /// Interaction logic for DatabaseConnectionView.xaml
    /// </summary>
    /// <remarks>
    /// منطق تعامل برای نمایش اتصال به پایگاه داده
    /// </remarks>
    public partial class DatabaseConnectionView : UserControl
    {
        public DatabaseConnectionView()
        {
            InitializeComponent();
            DataContext = new DatabaseConnectionViewModel();
        }
    }
} 