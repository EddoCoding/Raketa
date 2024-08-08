namespace CheckRaketa
{
    public interface IContainer
    {
        void RegisterTransient<Implementation, Interface>();
        void RegisterTransient<Implementation, Interface>(string identifier) where Implementation : class;
        void RegisterSingle<Interface>(object implementation);
        void RegisterSingle<Interface>(string identifier, object implementation);
        Interface GetDependency<Interface>();
        object GetDependency(string identifier, string[] identifiers = null, params object[] arguments);
        List<object> Resolve(Type viewModel, string[] identifiers = null, params object[] arguments);
    }
}