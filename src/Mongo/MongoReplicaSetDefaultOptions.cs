namespace Squadron
{
    /// <summary>
    /// Default Mongo ResplicaSet resource options
    /// </summary>
    public class MongoReplicaSetDefaultOptions : MongoDefaultOptions
    {
        internal string ReplicaSetName { get; }

        public MongoReplicaSetDefaultOptions(string replicaSetName)
        {
            ReplicaSetName = replicaSetName;
        }

        public MongoReplicaSetDefaultOptions()
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

