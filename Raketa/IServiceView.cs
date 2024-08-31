using System.Windows.Controls;

namespace Raketa
{
    public interface IServiceView
    {
        void RegisterTypeView<ViewModel, TWindow, TUserControl>()
            where ViewModel : RaketaViewModel
            where TWindow : IView;
        void RegisterTypeView<ViewModel, View>() where ViewModel : RaketaViewModel;

        IServiceView Window<ViewModel>(string[] identifier = null, params object[] args) where ViewModel : class;
        UserControl UserControl<ViewModel>(string[] identifier = null, params object[] args);

        void Modal();
        void NonModal();
        void Close<ViewModel>();
    }
}