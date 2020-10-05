using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Squadron
{
    public class NUnitSquadronInitializer
    {
        private IList<ISquadronAsyncLifetime> _resources;

        [OneTimeSetUp]
        protected async Task OneTimeSetUp()
        {
            _resources = new List<ISquadronAsyncLifetime>();

            var fields = this.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.IsDefined(typeof(NUnitSquadronInjectAttribute))).ToList();

            foreach (FieldInfo fieldInfo in fields)
            {
                var resourceInstance = (ISquadronAsyncLifetime)Activator.CreateInstance(fieldInfo.FieldType);
                if (resourceInstance == null)
                {
                    throw new ArgumentException($"The member must implement {nameof(ISquadronAsyncLifetime)} interface.");
                }
                fieldInfo.SetValue(this, resourceInstance);

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
    }
}
