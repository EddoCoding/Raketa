# ioC библиотечка с сервисом представлений

#### Алгоритм работы:
1. Создаем поля (в классе App):
```
IContainerDi _containerDi = new Container();
IServiceView _serviceView;
```

2. В поле получаем сервис представлений(интегрированный) (в классе App)
```
_serviceView = _containerDi.GetDependency<IServiceView>();
```

3. Регистрации. Для удобства делаем 2 метода, где вызываем методы регистрации (в классе App):

```
void RegisterView()
{
    _serviceView.RegisterTypeView<MainViewModel, MainWindow, MainUserControl>(); 
    _serviceView.RegisterTypeView<MainViewModel, MainWindow>();
    _serviceView.RegisterTypeView<MainViewModel, MainUserControl>();
}
```
С такой регистрацией View, можно работать как с окном так и вкладкой под одной ViemModel

```
void RegisterDependency()
{
Регистрация как Type, при каждом обращении получаем новый инстанс
    _containerDi.RegisterTransient<Service1, IService>();
    _containerDi.RegisterTransient<Service1, IService>("dsv");
    _containerDi.RegisterTransient<Service2, IService>("bf45");
    _containerDi.RegisterTransient<Service2>();

Регистрация как Object, при каждом обращении везде получаем один и тот же инстанс
    _containerDi.RegisterSingleton<IService>(new Service1());
    _containerDi.RegisterSingleton<IService>(new Service1(), "213");
    _containerDi.RegisterSingleton<IService>(new Service2(), "fddf");
}
```
Регистрация зависимостей происходит как:
- Тип имплементации | Интерфейс           `(Если каждый раз нужен новый экземпляр, по интерфейсу)`
- Идентификатор     | Тип имплементации   `(Если каждый раз нужен новый экземпляр, по идентифкатору)`
- Имплементация     | Интерфейс           `(Если каждый раз нужен существующий объект, по интерфейсу)`
- Идентификатор     | Имплементация       `(Если каждый раз нужен существующий объект, по идентифкатору)`
- Имлементация      | Имплементация       `(Если каждый раз нужен новый экземпляр, по его типу)`

4. Открываем(Получаем) View (в классе App или ViewModel):
```
_serviceView.Window<MainViewModel>().NonModal();         - Модальное окно 
_serviceView.Window<MainViewModel>().Modal();            - Немодальное окно
var tabItem = _serviceView.UserControl<MainViewModel>(); - Получаем вкладку
```

Если же нужно открыть окно или получить вкладку с разными имплементациями, то делаем так (в классе App или ViewModel):
```
_serviceView.Window<MainViewModel>(new[] { "dsv", "213" }).NonModal();
_serviceView.Window<MainViewModel>(new[] { "bf45", "fddf" }).Modal();
var tabItem = _serviceView.UserControl<MainViewModel>(new[] { "dsv", "fddf" });
```

Если же нужно передать доп. аргумент в вашу ViewModel, через зяпутую после string[] отправляем все что хотим (в классе App или ViewModel):
```
_serviceView.Window<MainViewModel>(new[] { "dsv", "213" }, new Dispose()).NonModal();
_serviceView.Window<MainViewModel>(new[] { "bf45", "fddf" }, "dsa", "213", 768).Modal();
var tabItem = _serviceView.UserControl<MainViewModel>(new[] { "dsv", "fddf" }, и т.д.);
```

Если же нужно просто передать аргументы, то вместо идентификаторов пишем null или default (в классе App или ViewModel):
```
_serviceView.Window<MainViewModel>(null, new Dispose()).NonModal();
_serviceView.Window<MainViewModel>(default, "dsa", "213", 768).Modal();
var tabItem = _serviceView.UserControl<MainViewModel>(null или default, и т.д.);
```
5. Закрытия окна.
Осуществляется через реализованный метод Exit() интерфейса IVIew, который нужно реализовать во View.
```
public partial class MainWindow : Window, IView
{
    public MainWindow() => InitializeComponent();
    public void Exit() => this.Close();
}
```
Во ViewModel вызываем метод Close сервиса представлений и типизируем моделью представления окна, которое нужно закрыть.
```
_serviceView.Close<MainViewModel>();
```
Сервис представлений найдет ваше окно и закроет.

ПРИМЕЧАНИЯ:
1. Сервис представлений изначально зарегистрирован при создании di-контейнера.
1. Ваши View должны реализовать встроенный интерфейс IView.
2. Если передаете последовательность примитивных данных, то и принимайте ее в той же последовательности.
3. Для закрытия окна через ViewModel, реализуйте во View метод выхода, интерфейса IView.

ОСОБЕННОСТИ:
1. Время жизни: Transient и Singleton.
2. Рекурсивная рефлексия - то есть внедрение зависимостей происходит даже для самих зависимостей и их зависимостей и т.д.
3. Возможность регистрировать имплементации под идентификаторами, для получения как во View так и во ViewModel конкретных реализаций.
4. Возможность получения имплементаций руками, там где нужно. (Метод GetDependency<Interface>() и GetDependency(string identifier)).
5. Настроены выпадающие исключения для разработчиков, чтобы отслеживать правильность использования библиотечки.
6. Возможность закрытия View из ViewModel абстрагировав логику через интерфейсе IView.
