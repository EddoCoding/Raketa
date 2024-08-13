using System.Windows;
using System.Windows.Controls;

namespace newRaketa
{
    public class ServiceView(IContainerDi containerDi) : IServiceView
    {
        IContainerDi _containerDi = containerDi;
        Dictionary<object, (Type, Type)> containerView = new();
        Dictionary<Type, IView> registryView = new();
        Window window;

        public void RegisterTypeView<ViewModel, TWindow, TUserControl>()
            where TWindow : IView
            where TUserControl : IView
        {
            checkKey<ViewModel>();
            containerView.Add(typeof(ViewModel), (typeof(TWindow), typeof(TUserControl)));
        }
        public void RegisterTypeView<ViewModel, View>() where View : IView
        {
            checkKey<ViewModel>();
            
            if (typeof(Window).IsAssignableFrom(typeof(View)))
                containerView.Add(typeof(ViewModel), (typeof(View), null));
            else if(typeof(UserControl).IsAssignableFrom(typeof(View))) 
                containerView.Add(typeof(ViewModel), (null, typeof(View)));
        }

        public IServiceView Window<ViewModel>(string[] identifier = null, params object[] args) where ViewModel : class
        {
            if (containerView.TryGetValue(typeof(ViewModel), out var view) && view.Item1 != null)
            {
                if (view.Item1 == null) throw new Exception($"{view.Item1} = null");

                var window = _containerDi.Resolve((Type)view.Item1, identifier, args) as Window;
                var viewModel = _containerDi.Resolve(typeof(ViewModel), identifier, args);
                window.DataContext = viewModel;
                this.window = window;
                registryView.Add(viewModel.GetType(), (IView)window);
            }
            else throw new Exception($"Модель представления: -- {typeof(ViewModel).Name} -- не зарегистрирована!");

            return this;
        }
        public UserControl UserControl<ViewModel>(string[] identifier = null, params object[] args)
        {
            if (containerView.TryGetValue(typeof(ViewModel), out var view) && view.Item2 != null)
            {
                if (view.Item2 == null) throw new Exception($"{view.Item2} = null");

                var userControl = _containerDi.Resolve((Type)view.Item2, identifier, args) as UserControl;
                var viewModel = _containerDi.Resolve(typeof(ViewModel), identifier, args);
                userControl.DataContext = viewModel;
                if (!registryView.ContainsKey(viewModel.GetType()))
                {
                    registryView.Add(viewModel.GetType(), (IView)userControl);
                    return userControl;
                }
                else
                {
                    MessageBox.Show("Вкладка уже открыта!");
                    return null;
                }
                
            }
            else throw new Exception($"Модель представления: -- {typeof(ViewModel).Name} -- не зарегистрирована!");
        }

        public void Modal()
        {
            if (window != null) window.ShowDialog();
        }
        public void NonModal()
        {
            if (window != null) window.Show();
        }
        public void Close<ViewModel>()
        {
            if(registryView.TryGetValue(typeof(ViewModel), out var view))
            {
                view.Exit();
                registryView.Remove(typeof(ViewModel));
            }
        }

        void checkKey<ViewModel>()
        {
            if (containerView.ContainsKey(typeof(ViewModel)))
                throw new Exception($"Тип модели представления: {typeof(ViewModel).Name} уже зарегистрирован!");
        }
    }
}