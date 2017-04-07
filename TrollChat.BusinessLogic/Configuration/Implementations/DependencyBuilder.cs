using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace TrollChat.BusinessLogic.Configuration.Implementations
{
    public class DependencyBuilder<T>
    {
        public void Register(IServiceCollection services)
        {
            var globalName = Assembly.GetEntryAssembly().GetName().Name;
            globalName = globalName.Substring(0, globalName.IndexOf(".", StringComparison.Ordinal));

            int countBeforeInjection = services.Count(x => x.Lifetime == ServiceLifetime.Scoped && x.ServiceType.ToString().Contains(globalName));
            int countGenericInterface = countBeforeInjection;
            var list = new Dictionary<Type, bool>();

            Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(assemly => assemly.Name.StartsWith(globalName)).ToList().ForEach(assemblyType =>
            {
                // Find interfaces in assemblies
                Assembly.Load(assemblyType).GetTypes().Where(assemblyClass => assemblyClass.GetTypeInfo().IsInterface).ToList().ForEach(myInterface =>
                {
                    // If interface inherits interface of type <T> increment counter
                    if (myInterface.GetInterfaces().Contains(typeof(T)))
                    {
                        countGenericInterface++;
                        list.Add(myInterface, false);
                    }
                });

                // Find classes in assemblies
                Assembly.Load(assemblyType).GetTypes().Where(assemblyClass => assemblyClass.GetTypeInfo().IsClass).ToList().ForEach(implementation =>
                {
                    // If class's interface inherits <T> register it
                    var myInterface = implementation.GetInterfaces().FirstOrDefault(Iimplementation => Iimplementation.GetInterfaces().Contains(typeof(T)));

                    if (myInterface != null)
                    {
                        services.AddScoped(myInterface, implementation);
                        list[myInterface] = true;
                        Debug.WriteLine($"Registered {typeof(T).Name} interface {myInterface.Name} to {implementation.Name}");
                    }
                });
            });

            int countAfterInjection = services.Count(x => x.Lifetime == ServiceLifetime.Scoped && x.ServiceType.ToString().Contains(globalName));

            if (countGenericInterface != countAfterInjection)
            {
                // This error is thrown when a class isn't registered but it's interface that inherits <T> exists
                foreach (var missing in list.Where(i => i.Value == false).Select(i => i.Key.Name))
                {
                    Debug.WriteLine($"Mismatch between interfaces and implementations for {typeof(T).Name}: {missing}");
                }

                throw new NotImplementedException();
            }
        }
    }
}