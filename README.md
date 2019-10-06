<div style="text-align:center"><img src="https://raw.github.com/swissLife-oss/squadron-docs/master/website/static/img/logo_sl_squadron.png" height="200" /></div>

**Squadron is a testing framework for containerized and cloud services.**

Squadron is a helpful framework which enables you to write tests against dependent services without any overhead. Squadron can provide you isolation in tests through Container Providers or support for all other services through Cloud Providers.

## Features

### Container Providers

- [x] [MongoDB](https://swisslife-oss.github.io/squadron/docs/mongodb-getstarted)
- [x] [SQL Server](https://swisslife-oss.github.io/squadron/docs/sqlserver-getstarted)
- [x] [Elasticsearch](https://swisslife-oss.github.io/squadron/docs/elasticsearch-getstarted)
- [x] Azure Blob and Queues
- [x] RabittMQ
- [x] Redis
- [ ] PostgresSQL
- [ ] Kafka

### Cloud Providers
- [ ] Azure Service Bus

## Getting Started

As getting started we've prepared a simple example how to use Squadron with *MongoDB*.

### Install
Install the Squadron nuget package for MongoDB within your test project:

```bash
dotnet add package Squadron.Mongo
```

### Access
Inject the MongoResource into your test class constructor:

```csharp
public class AccountRepositoryTests
    : IClassFixture<MongoResource>
{
    private readonly MongoResource _mongoResource;

    public AccountRepositoryTests(
        MongoResource mongoResource)
    {
        _mongoResource = mongoResource;
    }
}
```

### Use
Use MongoResources to create a database and initialize your repository:

```csharp
[Fact]
public void CreateAccount_AccountExists()
{
    // arrange
    var database = _mongoResource.CreateDatabase();
    var accountRepository = new AccountRepository(database);
    var account = new Account();

    // act
    var addedAccount = accountRepository.Add(account);

    // assert
    Snapshot.Match(addedAccount);
}
```

## Community

This project has adopted the code of conduct defined by the [Contributor Covenant](https://contributor-covenant.org/)
to clarify expected behavior in our community. For more information, see the [Swiss Life OSS Code of Conduct](https://swisslife-oss.github.io/coc).
