namespace Squadron
{
    public abstract class MongoInitOptions : MongoDefaultOptions
    {
        public abstract CreateDatabaseFromFilesOptions GetOptions();
    }
}
