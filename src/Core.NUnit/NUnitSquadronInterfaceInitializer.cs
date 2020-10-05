using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Squadron
{
    public class NUnitSquadronInterfaceInitializer
    {
        private IList<ISquadronAsyncLifetime> _resources;

        [OneTimeSetUp]
        protected async Task OneTimeSetUp()
        {
            _resources = new List<ISquadronAsyncLifetime>();

            IEnumerable<Type> resourceTypes = this.GetType().GetInterfaces().ToList();

            foreach (var resourceTypeWrapper in resourceTypes)
            {
                var resourceType = resourceTypeWrapper.GetGenericArguments().First();
                var resourceInstance =
                    (ISquadronAsyncLifetime)Activator.CreateInstance(resourceType);
                if (resourceInstance == null)
                {
                    throw new ArgumentException($"The member must implement {nameof(ISquadronAsyncLifetime)} interface.");
                }

                _resources.Add(resourceInstance);

                await resourceInstance.InitializeAsync();
            }
        }

        [OneTimeTearDown]
        protected async Task OneTimeTearDown()
        {
            foreach (ISquadronAsyncLifetime resource in _resources)
            {
                await resource.DisposeAsync();
            }
        }

        protected TResource GetSquadronResource<TResource>() where TResource : ISquadronAsyncLifetime
        {
            return (TResource)_resources.First(p => p.GetType() == typeof(TResource));
        }
    }
}
