using System.Windows;
using System.Windows.Controls;

namespace CheckRaketa
{
    public class ServiceView : IServiceView
    {
        Dictionary<Type, (Type, Type)> containerView = new();   // Контейнер хранит сопоставление VM с View(window и usercontrol)
        Dictionary<object, object> registryView = new();        // Контейнер для отслеживания открытых окон
        Window window;

        IContainer _containerDi;
        public ServiceView(IContainer containerDi) => _containerDi = containerDi;

        public void RegisterView<ViewModel, ViewWindow, ViewUserControl>()
            where ViewWindow : Window
            where ViewUserControl : UserControl
        {
            checkKey<ViewModel>();
            containerView.Add(typeof(ViewModel), (typeof(ViewWindow), typeof(ViewUserControl)));
        }
        public void RegisterView<ViewModel, View>()
        {
            checkKey<ViewModel>();
            if (typeof(Window).IsAssignableFrom(typeof(View))) containerView.Add(typeof(ViewModel), (typeof(View), null));
            if (typeof(UserControl).IsAssignableFrom(typeof(View))) containerView.Add(typeof(ViewModel), (null, typeof(View)));
        }

        public ServiceView Window<View>(string[] identifiers = null, params object[] arguments) where View : Window
        {
            window = Activator.CreateInstance<View>();
            var viewType = typeof(View);
            foreach (var viewModel in containerView)
            {
                if (viewModel.Value.Item1 == viewType)
                {
                    List<object> dependencies = _containerDi.Resolve(viewModel.Key, identifiers, arguments);
                    var vm = Activator.CreateInstance(viewModel.Key, dependencies.ToArray());
                    window.DataContext = vm;
                    registryView.Add(vm, window);
                    break;
                }
            }
            return this;
        }
        public UserControl UserControl<View>(string[] identifiers = null, params object[] arguments) where View : UserControl
        {
            UserControl userControl = Activator.CreateInstance<View>();
            var viewType = typeof(View);
            foreach (var value in containerView)
                if (value.Value.Item2 == viewType)
                {
                    var vm = Activator.CreateInstance(value.Key, _containerDi.Resolve(value.Key, identifiers, arguments).ToArray());
                    userControl.DataContext = vm;
                    registryView.Add(vm, userControl);
                    break;
                }

            return userControl;
        }


        public void Modal() => window.ShowDialog();
        public void NonModal() => window.Show();
        public void Close(object viewModel)
        {
            var view = registryView[viewModel];
            if(view is Window window)
            {
                window.Close();
                registryView.Remove(viewModel);
            }
        }


        void checkKey<ViewModel>()
        {
            if (containerView.ContainsKey(typeof(ViewModel)))
                throw new ArgumentException($"ViewModel: -- {typeof(ViewModel).Name} -- уже зарегистрирована!");
        }
    }
}