# ioC библиотечка 

#### Алгоритм работы в классе APP:
1. `_container = new ContainerDi();` - Получаем контейнер
2. ` _serviceView = _container.GetDependency<IServiceView>();` -  Получаем сервис окон из контейнера(он уже внутри интегрированный)
3. Далее регистрируем

   Регистрация представлений
   
Регистрация VM может поисходить по отдельности как с Window, UserControl так и вместе, пример:
- `_serviceView.RegisterView<MainViewModel, MainWindow, MainUserControl>();` - регистрация сразу двух представлений
- `_serviceView.RegisterView<MainViewModel, MainWindow>();` - регистрация VM и Window
- `_serviceView.RegisterView<MainViewModel, MainUserControl>();` - регистрация VM и UserControl

   
Такая регистрация сделана, чтобы можно было работать с одной VM в разных представлениях в зависимости от
подхода к архитектуре, многооконное приложение или в TabControl отобрать вкладки.

  Регистрация зависимостей

Регистрация происходит как Transient так и Singleton, пример:
- `_container.RegisterTransient<Service, IService>();`
- `_container.RegisterSingle<IService>(new Service2() или заранее созданный);`

4. Первоначально из класса App вызываем метод открытия окна или получение вкладки для TabControl:
-  `_serviceView.Window<MainWindow>().NonModal();` - Если нужно немодальное окно
-  `_serviceView.Window<MainWindow>().Modal();` - Если нужно модальное окно
-  `_serviceView.UserControl<MainUserControl>();` - Получение вкладки

При создании окон или получения вкладки в них будет установлен DataContext то есть ViewModel, которая
регистрировалась в месте с окнами. Если ViewModel требуют зависимости перед установкой в DataContext, то
во ViewModel они автоматически внедряются из тех зависимостей, которые регистрировались ранее. Внерение 
зависимостей происходит рекурсивно, то есть если есть зависимости у зависимостей и т.д. то во всю
иерархию будут прокинуты зависимости, которые зарегистрированы, а также если нет, то будут браться из 
аргументов, которые мы передаем.

5. Дополнительные возможности:
- Регистрация зависимостей с идентификаторами, для того чтобы брать конкретные имплементации, пример:
  
`_container.RegisterTransient<Service1, IService>("1");`

`_container.RegisterTransient<Service2, IService>("2");`

- Вызов окна или вкладки с нужными имплементациями, пример:

`_serviceView.Window<MainWindow>(new[] { "1", "dsafsa" }).NonModal();`
`_serviceView.UserControl<MainUserControl>(new[] { "dfds", "4" });`

- Если ViewModel нужен какой-нибудь аргумент или их некое кол-во разных типов, можно передать при открытие окна, пример:

 `_serviceView.Window<MainWindow>(new[] { "1", "2" }, new Service1(), "sad", 4).NonModal();` - то есть передаем аргументы через запятую после идентификаторов string[] 
 
 `_serviceView.Window<MainWindow>(null, new Service1(), "sad", 4).NonModal();` - если идентификаторы не нужны, просто отправляем null или default

 - Если нужно открыть окно из ViewModel: то просто пишем в конструкторе IServiceView serviceView(или др. название), и сервис окон сам проброситься, пример:

`public MainViewModel(IServiceView serviceView) {  }`
