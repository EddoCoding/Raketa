namespace Raketa
{
    public interface IContainerDi
    {
        void RegisterTransient<Implementation, Interface>();
        void RegisterTransient<Implementation, Interface>(string identifier);
        void RegisterTransient<Implementation>();

        void RegisterSingleton<Interface>(object impl);
        void RegisterSingleton<Interface>(object impl, string identifier);
        void RegisterSingleton<Implementation, Interface>();

        Interface GetDependency<Interface>();
        object GetDependency(string identifier);

        object Resolve(Type ctor, string[] identifiers = null, params object[] args);
        object Resolve<Type>(string[] identifiers = null, params object[] args);
    }
}