using System.Windows.Controls;

namespace newRaketa
{
    public interface IServiceView
    {
        void RegisterTypeView<ViewModel, TWindow, TUserControl>()
            where TWindow : IView
            where TUserControl : IView;
        void RegisterTypeView<ViewModel, View>() where View : IView;

        IServiceView Window<ViewModel>(string[] identifier = null, params object[] args) where ViewModel : class;
        UserControl UserControl<ViewModel>(string[] identifier = null, params object[] args);

        void Modal();
        void NonModal();
        void Close<ViewModel>();
    }
}