using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using FluentAssertions;
using Nest;
using Squadron;
using Xunit;
using Xunit.Abstractions;

namespace Elasticsearch.Tests;

public class ElasticsearchResourceTests(ElasticsearchResource elasticsearchResource)
    : IClassFixture<ElasticsearchResource>
{
    [Fact]
    public async Task CreateIndexTest()
    {
        // arrange
        // act
        await elasticsearchResource.CreateIndexAsync<Foo>();

        // assert
        CatResponse<CatIndicesRecord> catIndices =
            await elasticsearchResource.Client.Cat.IndicesAsync();
        catIndices.IsValid.Should().BeTrue();
        catIndices.Records.Count.Should().BeGreaterOrEqualTo(1);
    }

    [Fact]
    public async Task CreateDocumentsTest()
    {
        // arranges
        var firstDocument = new Foo { Id = Guid.NewGuid().ToString(), FooDescription = "Some foo description" };
        var secondDocument = new Foo { Id = Guid.NewGuid().ToString(), FooDescription = "Another foo description" };
        var firstDocumentRequest = new CreateDocumentRequest<Foo> { Id = firstDocument.Id, Document = firstDocument };
        var secondDocumentRequest = new CreateDocumentRequest<Foo> { Id = secondDocument.Id, Document = secondDocument };

        // act
        var index = await elasticsearchResource.CreateIndexAsync<Foo>();
        await elasticsearchResource.CreateDocumentsAsync(index, firstDocumentRequest, secondDocumentRequest);

        // assert
        ISearchResponse<Foo> searchResponse = await elasticsearchResource.Client
            .SearchAsync<Foo>(new SearchRequest<Foo>(Indices.Index(index)));
        searchResponse.IsValid.Should().BeTrue();
        searchResponse.Documents.Should().HaveCount(2);
        searchResponse.Documents.Should().Contain(foo => foo.Id == firstDocument.Id);
        searchResponse.Documents.Should().Contain(foo => foo.Id == secondDocument.Id);
    }

    [Fact]
    public async Task MergeIndicesTest()
    {
        // arranges
        var firstDocument = new Foo { Id = Guid.NewGuid().ToString(), FooDescription = "Some foo description" };
        var secondDocument = new Foo { Id = Guid.NewGuid().ToString(), FooDescription = "Another foo description" };
        var firstDocumentRequest = new CreateDocumentRequest<Foo> { Id = firstDocument.Id, Document = firstDocument };
        var secondDocumentRequest = new CreateDocumentRequest<Foo> { Id = secondDocument.Id, Document = secondDocument };
        var firstIndex = await elasticsearchResource.CreateIndexAsync<Foo>();
        await elasticsearchResource.CreateDocumentsAsync(firstIndex, firstDocumentRequest);
        var secondIndex = await elasticsearchResource.CreateIndexAsync<Foo>();
        await elasticsearchResource.CreateDocumentsAsync(secondIndex, secondDocumentRequest);

        // act
        var mergedIndex = await elasticsearchResource.MergeIndicesAsync(firstIndex, secondIndex);

        // assert
        ISearchResponse<Foo> searchResponse = await elasticsearchResource.Client
            .SearchAsync<Foo>(new SearchRequest<Foo>(Indices.Index(mergedIndex)));
        searchResponse.IsValid.Should().BeTrue();
        searchResponse.Documents.Should().HaveCount(2);
        searchResponse.Documents.Should().Contain(foo => foo.Id == firstDocument.Id);
        searchResponse.Documents.Should().Contain(foo => foo.Id == secondDocument.Id);
    }

    private class Foo
    {
        public string Id { get; set; }
        public string FooDescription { get; set; }
    }
}