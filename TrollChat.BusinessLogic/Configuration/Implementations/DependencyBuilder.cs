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
            if (globalName.Contains('.'))
            {
                // Reduce "Project.Web" to "Project"
                globalName = globalName.Split('.')[0];
            }

            var mismatchList = new Dictionary<Type, bool>();

            Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(assembly => assembly.Name.StartsWith(globalName)).ToList().ForEach(assemblyType =>
            {
                // Find interfaces in assemblies
                Assembly.Load(assemblyType).GetTypes().Where(assemblyInterface => assemblyInterface.GetTypeInfo().IsInterface).ToList().ForEach(myInterface =>
                {
                    // If interface inherits interface of type <T> add to list
                    if (myInterface.GetInterfaces().Contains(typeof(T)))
                    {
                        mismatchList.Add(myInterface, false);
                    }
                });

                // Find classes in assemblies
                Assembly.Load(assemblyType).GetTypes().Where(assemblyClass => assemblyClass.GetTypeInfo().IsClass).ToList().ForEach(implementationToInject =>
                {
                    // If interface of class inherits <T> register it
                    var interfaceToInject = implementationToInject.GetInterfaces().FirstOrDefault(myInterface => myInterface.GetInterfaces().Contains(typeof(T)));

                    if (interfaceToInject == null) return;

                    services.AddScoped(interfaceToInject, implementationToInject);
                    mismatchList[interfaceToInject] = true;
                    Debug.WriteLine($"Registered {typeof(T).Name} interface {interfaceToInject.Name} to {implementationToInject.Name}");
                });
            });

            if (mismatchList.All(x => x.Value)) return;

            foreach (var missing in mismatchList.Where(i => i.Value == false).Select(i => i.Key.Name))
            {
                Debug.WriteLine($"Mismatch between interfaces and implementations for {typeof(T).Name}: {missing}");
            }
            // This error is thrown when a class isn't registered but its interface that inherits <T> exists
            throw new NotImplementedException();
        }
    }
}