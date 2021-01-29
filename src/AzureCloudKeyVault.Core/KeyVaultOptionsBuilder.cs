using Squadron.AzureCloud;
using Squadron.Model;

namespace Squadron
{
    public class KeyVaultOptionsBuilder : AzureResourceOptionsBuilder
    {
        private KeyVaultModel _model = new KeyVaultModel();

        /// <summary>
        /// Creates a new empty builder
        /// </summary>
        /// <returns></returns>
        public static KeyVaultOptionsBuilder New() => new KeyVaultOptionsBuilder();

        private KeyVaultOptionsBuilder()
            : base()
        {
        }

        public KeyVaultOptionsBuilder Name(string name)
        {
            _model.Name = name;

            return this;
        }

        /// <summary>
        /// Builds the options
        /// </summary>
        /// <returns></returns>
        public KeyVaultModel Build()
        {
            return _model;
        }
    }
}
