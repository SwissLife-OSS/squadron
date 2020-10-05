using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Squadron
{
    public class SquadronInitializer
    {
        private IList<ISquadronAsyncLifetime> _resources;

        [OneTimeSetUp]
        protected async Task OneTimeSetUp()
        {
            _resources = GetResources();

            foreach (ISquadronAsyncLifetime resource in _resources)
            {
                await resource.InitializeAsync().ConfigureAwait(false);
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

        protected TResource GetSquadronResource<TResource>()
            where TResource : ISquadronAsyncLifetime
        {
            return (TResource)_resources.First(p => p.GetType() == typeof(TResource));
        }

        private IList<ISquadronAsyncLifetime> GetResources()
        {
            var squadronResources = new List<ISquadronAsyncLifetime>();

            TryAddToListFromInterface(squadronResources);
            TryAddToListFromAttribute(squadronResources);

            return squadronResources;
        }

        private void TryAddToListFromInterface(List<ISquadronAsyncLifetime> squadronResources)
        {
            IList<Type> resourceTypes =
                SquadronNUnitHelpers.GetFromTypeByInterface(this.GetType());

            foreach (Type resourceType in resourceTypes)
            {
                if (squadronResources.All(p => p.GetType() != resourceType))
                {
                    ISquadronAsyncLifetime instance = GetSquadronResourceInstance(resourceType);
                    squadronResources.Add(instance);
                }
            }
        }

        private void TryAddToListFromAttribute(List<ISquadronAsyncLifetime> squadronResources)
        {
            IList<FieldInfo> fieldInfos = SquadronNUnitHelpers.GetFromTypeByAttribute(this.GetType());

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                Type resourceType = fieldInfo.FieldType;
                ISquadronAsyncLifetime resourceInstance =
                    squadronResources.FirstOrDefault(p => p.GetType() == resourceType);

                if (resourceInstance == null)
                {
                    resourceInstance = GetSquadronResourceInstance(resourceType);
                }

                fieldInfo.SetValue(this, resourceInstance);

                squadronResources.Add(resourceInstance);
            }
        }

        private ISquadronAsyncLifetime GetSquadronResourceInstance(Type resourceType)
        {
            var resourceInstance =
                (ISquadronAsyncLifetime)Activator.CreateInstance(resourceType);

            if (resourceInstance == null)
            {
                throw new ArgumentException(
                    $"The member must implement {nameof(ISquadronAsyncLifetime)} interface.");
            }

            return resourceInstance;
        }
    }
}
