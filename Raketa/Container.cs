using System.Reflection;

namespace Raketa
{
    public class Container : IContainerDi
    {
        Dictionary<object, Type> containerDependency = new();
        Dictionary<string, object> registryDependency = new();

        public Container() => containerDependency.Add(new ServiceView(this), typeof(IServiceView));

        public void RegisterTransient<Implementation, Interface>()
        {
            checkKey<Implementation>();
            checkInterface<Implementation, Interface>();
            containerDependency.Add(typeof(Implementation), typeof(Interface));
        }
        public void RegisterTransient<Implementation, Interface>(string identifier)
        {
            checkKey<Implementation>();
            checkInterface<Implementation, Interface>();
            checkIdentifier(identifier);
            containerDependency.Add(typeof(Implementation), typeof(Interface));
            registryDependency.Add(identifier, typeof(Implementation));
        }
        public void RegisterTransient<Implementation>()
        {
            checkKey<Implementation>();
            containerDependency.Add(typeof(Implementation), typeof(Implementation));
        }

        public void RegisterSingleton<Interface>(object impl)
        {
            checkImplementation<Interface>(impl);
            containerDependency.Add(impl, typeof(Interface));
        }
        public void RegisterSingleton<Interface>(object impl, string identifier)
        {
            checkImplementation<Interface>(impl);
            checkIdentifier(identifier);
            containerDependency.Add(impl, typeof(Interface));
            registryDependency.Add(identifier, impl);
        }
        public void RegisterSingleton<Implementation, Interface>()
        {
            checkInterface<Implementation, Interface>();
            var impl = Resolve<Implementation>();
            containerDependency.Add(impl, typeof(Interface));
        }

        public Interface GetDependency<Interface>()
        {
            var implementation = containerDependency.FirstOrDefault(container => container.Value == typeof(Interface)).Key;

            if (implementation == null) throw new Exception($"Интерфейс - {typeof(Interface).Name} - не зарегистрирован!");

            if (implementation is not Type typeImplementation) return (Interface)implementation;
            return (Interface)Resolve(typeImplementation);
        }
        public object GetDependency(string identifier)
        {
            if (registryDependency.TryGetValue(identifier, out var impl))
            {
                if (impl is not Type) return impl;
                else return Resolve((Type)impl);
            }
            else throw new Exception($"Идентификатор: - {identifier} - не зарегистрирован!");
        }

        public object Resolve(Type ctor, string[] identifiers = null, params object[] args)
        {
            var constructor = ctor.GetConstructors().FirstOrDefault();
            var ctorParameters = constructor.GetParameters();
            if (ctorParameters.Length == 0) return Activator.CreateInstance(ctor);
            else return Activator.CreateInstance(ctor, GetDependencies(ctorParameters, identifiers, args));
        }
        public object Resolve<Type>(string[] identifiers = null, params object[] args)
        {
            var constructor = typeof(Type).GetConstructors().FirstOrDefault();
            var ctorParameters = constructor.GetParameters();
            if (ctorParameters.Length == 0) return Activator.CreateInstance(typeof(Type));
            else return Activator.CreateInstance(typeof(Type), GetDependencies(ctorParameters, identifiers, args));
        }


        object[] GetDependencies(ParameterInfo[] ctorParameters, string[] identifiers = null, params object[] args)
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
                        var impl = Resolve((Type)obj, identifiers, args);
                        dependencies.Add(impl);
                    }
                }
                else if (count > 1)
                {
                    bool exit = false;
                    foreach (var identifier in identifiers)
                    {
                        foreach (var itemContainer in registryDependency)
                        {
                            if (identifier == itemContainer.Key && parameter.ParameterType.IsAssignableFrom((Type)itemContainer.Value))
                            {
                                if (itemContainer.Value is not Type) dependencies.Add(itemContainer.Value);
                                else if (itemContainer.Value is Type)
                                {
                                    var impl = Resolve((Type)itemContainer.Value, identifiers, args);
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
                    foreach (var arg in args)
                    {
                        if (arg is Type)
                        {
                            var impl = Resolve((Type)arg, identifiers, args);
                            dependencies.Add(impl);
                            break;
                        }
                        else
                        {
                            if (arg.GetType() == type)
                            {
                                dependencies.Add(arg);
                                break;
                            }
                        }
                    }
                }
            }

            return dependencies.ToArray();
        }

        void checkKey<Implementation>()
        {
            if (containerDependency.ContainsKey(typeof(Implementation)))
                throw new ArgumentException($"Тип имплементации: - {typeof(Implementation).Name} - уже зарегистрирован!");
        }
        void checkInterface<Implementation, Interface>()
        {
            if (!typeof(Interface).IsAssignableFrom(typeof(Implementation)))
                throw new InvalidCastException($"Имплементация: {typeof(Implementation).Name} не реализует интерфейс {typeof(Interface).Name}");
        }
        void checkIdentifier(string identifier)
        {
            if (registryDependency.ContainsKey(identifier))
                throw new ArgumentException($"Идентификатор: - {identifier} - уже зарегистрирован!");
        }
        void checkImplementation<Interface>(object impl)
        {
            if (containerDependency.ContainsKey(impl))
                throw new ArgumentException($"Имплементация: - {impl} - уже зарегистрирована!");
            if (!typeof(Interface).IsAssignableFrom(impl.GetType()))
                throw new InvalidCastException($"Имплементация: {impl} не реализует интерфейс {typeof(Interface).Name}");
        }
    }
}