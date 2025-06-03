namespace Squadron;

/// <summary>
/// Default Mongo ReplicaSet resource options
/// </summary>
public class MongoReplicaSetDefaultOptions(string replicaSetName) : MongoDefaultOptions
{
    internal string ReplicaSetName { get; } = replicaSetName;

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