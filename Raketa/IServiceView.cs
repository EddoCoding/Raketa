using System.Windows;
using System.Windows.Controls;

namespace CheckRaketa
{
    public interface IServiceView
    {
        void RegisterView<ViewModel, ViewWindow, ViewUserControl>()
            where ViewWindow : Window
            where ViewUserControl : UserControl;
        void RegisterView<ViewModel, View>();
        ServiceView Window<View>(string[] identifiers = null, params object[] arguments) where View : Window;
        UserControl UserControl<View>(string[] identifiers = null, params object[] arguments) where View : UserControl;

        void Modal();
        void NonModal();

        void Close(object viewModel);
    }
}