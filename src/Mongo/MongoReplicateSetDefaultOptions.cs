namespace Squadron
{
    /// <summary>
    /// Default Mongo ResplicaSet resource options
    /// </summary>
    public class MongoReplicateSetDefaultOptions : MongoDefaultOptions
    {
        internal string ReplicaSetName { get; }

        public MongoReplicateSetDefaultOptions(string replicaSetName)
        {
            ReplicaSetName = replicaSetName;
        }

        public MongoReplicateSetDefaultOptions()
            : this("rs0")
        {

        }

        public override void Configure(ContainerResourceBuilder builder)
        {
            builder.AddCmd("--replSet", ReplicaSetName);
            base.Configure(builder);
        }
    }
}

