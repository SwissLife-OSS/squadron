![Snapshooter](https://raw.github.com/swissLife-oss/squadron-docs/master/website/static/img/logo_sl_squadron_banner.png)

## [![Nuget](https://img.shields.io/nuget/v/Squadron.Core.svg?style=flat)](https://www.nuget.org/packages/Squadron.Core) [![GitHub Release](https://img.shields.io/github/release/SwissLife-OSS/Squadron.svg?style=flat)](https://github.com/SwissLife-OSS/Squadron/releases/latest) [![Build Status](https://dev.azure.com/swisslife-oss/swisslife-oss/_apis/build/status/Squadron.Release?branchName=master)](https://dev.azure.com/swisslife-oss/swisslife-oss/_build/latest?definitionId=11&branchName=master) [![Coverage Status](https://sonarcloud.io/api/project_badges/measure?project=SwissLife-OSS_Squadron&metric=coverage)](https://sonarcloud.io/dashboard?id=SwissLife-OSS_Squadron) [![Quality](https://sonarcloud.io/api/project_badges/measure?project=SwissLife-OSS_Squadron&metric=alert_status)](https://sonarcloud.io/dashboard?id=SwissLife-OSS_Squadron)

**Squadron is a testing framework for containerized and cloud services.**

Squadron is a helpful framework which enables you to write tests against dependent services without any overhead. Squadron can provide you isolation in tests through Container Providers or support for all other services through Cloud Providers.

To get more detailed information about Squadron, go to the [Squadron Docs](https://swisslife-oss.github.io/squadron/)

## Features

### Container Providers

- [x] [MongoDB](https://swisslife-oss.github.io/squadron/docs/mongodb)
- [x] [SQL Server](https://swisslife-oss.github.io/squadron/docs/sqlserver)
- [x] [Elasticsearch](https://swisslife-oss.github.io/squadron/docs/elasticsearch)
- [x] [Azure Blob and Queues](https://swisslife-oss.github.io/squadron/docs/azure-storage)
- [x] [RabittMQ](https://swisslife-oss.github.io/squadron/docs/rabbitmq)
- [x] [Redis](https://swisslife-oss.github.io/squadron/docs/redis)
- [x] [PostgresSQL](https://swisslife-oss.github.io/squadron/docs/postgresql)
- [ ] Kafka
- [ ] RavenDB
- [ ] MySQL (MariaDB)
- [ ] MongoDB with ReplicatSet


### Cloud Providers
- [x] [Azure Service Bus](https://swisslife-oss.github.io/squadron/docs/azure-cloud-servicebus)
- [ ] Azure Event Hub
- [ ] Azure Storage

## Getting Started

As getting started we've prepared a simple example how to use Squadron with *MongoDB*.

You can find samples with quick starts [here](https://github.com/SwissLife-OSS/squadron/tree/master/samples).

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
