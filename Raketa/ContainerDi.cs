using System.Reflection;

namespace CheckRaketa
{
    public class ContainerDi : IContainer
    {
        Dictionary<object, Type> containerDependency = new();
        Dictionary<string, object> dependencyIdentifiers = new();

        public ContainerDi() => containerDependency.Add(new ServiceView(this), typeof(IServiceView));

        public void RegisterTransient<Implementation, Interface>()
        {
            checkKey<Implementation>();
            if (!typeof(Interface).IsAssignableFrom(typeof(Implementation)))
                throw new InvalidCastException($"Имплементация: {typeof(Implementation).Name} не реализует интерфейс {typeof(Interface).Name}");
            containerDependency.Add(typeof(Implementation), typeof(Interface));
        }

        public void RegisterTransient<Implementation, Interface>(string identifier) where Implementation : class
        {
            checkKey<Implementation>();
            checkIdentifier(identifier);

            containerDependency.Add(typeof(Implementation), typeof(Interface));
            dependencyIdentifiers.Add(identifier, typeof(Implementation));
        }

        public void RegisterSingle<Interface>(object implementation)
        {
            checkKey(implementation);
            checkInterface<Interface>(implementation);
            containerDependency.Add(implementation, typeof(Interface));
        }
        
        public void RegisterSingle<Interface>(string identifier, object implementation)
        {
            checkKey(implementation);
            checkIdentifier(identifier);
            checkInterface<Interface>(implementation);

            containerDependency.Add(implementation, typeof(Interface));
            dependencyIdentifiers.Add(identifier, implementation);
        }

        public Interface GetDependency<Interface>()
        {
            var implementation = containerDependency.FirstOrDefault(container => container.Value == typeof(Interface)).Key;
        
            if (implementation is not Type typeImplementation) return (Interface)implementation;
            return (Interface)Activator.CreateInstance(typeImplementation);
        }
        public object GetDependency(string identifier, string[] identifiers = null, params object[] arguments)
        {
            var implementation = dependencyIdentifiers.GetValueOrDefault(identifier, default);
            if (implementation is Type) return Activator.CreateInstance((Type)implementation, Resolve((Type)implementation, identifiers, arguments).ToArray());
            else return implementation;
        }
        
        public List<object> Resolve(Type constructor, string[] identifiers = null, params object[] arguments)
        {
            var ctor = constructor.GetConstructors().FirstOrDefault();
            var ctorParameters = ctor.GetParameters();
            if (ctorParameters.Length == 0) return new object[0].ToList();
            return GetDependencies(ctorParameters, identifiers, arguments);
        }

        List<object> GetDependencies(ParameterInfo[] ctorParameters, string[] identifiers = null, params object[] arguments)
        {
            List<object> dependencies = new();

            foreach (var parameter in ctorParameters)
            {
                var type = parameter.ParameterType;
                int count = 0;

                foreach (var value in containerDependency.Values)
                    if (type.IsAssignableFrom(value))
                        count++;

                if (count == 1)
                {
                    var obj = containerDependency.FirstOrDefault(container => container.Value == type).Key;
                    if (obj is not Type) dependencies.Add(obj);
                    else if (obj is Type)
                    {
                        var impl = Activator.CreateInstance((Type)obj, Resolve((Type)obj, identifiers, arguments).ToArray());
                        dependencies.Add(impl);
                    }
                }
                else if (count > 1)
                {
                    bool exit = false;
                    foreach (var identifier in identifiers)
                    {
                        foreach (var itemContainer in dependencyIdentifiers)
                        {
                            if (identifier == itemContainer.Key && parameter.ParameterType.IsAssignableFrom((Type)itemContainer.Value))
                            {
                                if (itemContainer.Value is not Type) dependencies.Add(itemContainer.Value);
                                else if (itemContainer.Value is Type)
                                {
                                    var impl = Activator.CreateInstance((Type)itemContainer.Value,
                                        Resolve((Type)itemContainer.Value, identifiers, arguments).ToArray());
                                    dependencies.Add(impl);
                                }
                                exit = true;
                                break;
                            }
                        }
                        if (exit) break;
                    }
                }
                else
                {
                    foreach (var argument in arguments)
                    {
                        if (argument is Type)
                        {
                            var impl = Activator.CreateInstance((Type)argument, Resolve((Type)argument, identifiers, arguments).ToArray());
                            dependencies.Add(impl);
                            break;
                        }
                        else
                        {
                            if (argument.GetType() == type)
                            {
                                dependencies.Add(argument);
                                break;
                            }
                        }
                    }
                }
            }


            return dependencies;
        }




        void checkKey<Implementation>()
        {
            if (containerDependency.ContainsKey(typeof(Implementation)))
                throw new ArgumentException($"Имплементация: -- {typeof(Implementation).Name} -- уже зарегистрирована!");
        }
        void checkKey(object implementation)
        {
            if (containerDependency.ContainsKey(implementation))
                throw new ArgumentException($"Имплементация {implementation} уже зарегистрирована!");
        }
        void checkInterface<Interface>(object implementation)
        {
            if (!typeof(Interface).IsAssignableFrom(implementation.GetType()))
                throw new InvalidOperationException($"Имплементация: {implementation.GetType()} не реализует интерфейс {typeof(Interface).Name}!");
        }
        void checkIdentifier(string identifier)
        {
            if (dependencyIdentifiers.ContainsKey(identifier))
                throw new ArgumentException($"Идентификатор: -- {identifier} -- уже зарегистрирован!");
        }
    }
}