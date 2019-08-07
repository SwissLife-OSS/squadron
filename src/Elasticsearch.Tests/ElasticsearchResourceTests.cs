using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using FluentAssertions;
using Nest;
using Squadron;
using Xunit;
using Xunit.Abstractions;

namespace Elasticsearch.Tests
{
    public class ElasticsearchResourceTests
        : IClassFixture<ElasticsearchResource>
    {
        private readonly ElasticsearchResource _elasticsearchResource;

        public ElasticsearchResourceTests(ElasticsearchResource elasticsearchResource)
        {
            _elasticsearchResource = elasticsearchResource;
        }

        [Fact]
        public async Task CreateIndexTest()
        {
            // arrange
            // act
            await _elasticsearchResource.CreateIndexAsync<Foo>();

            // assert
            var catIndices = _elasticsearchResource.Client.CatIndices();
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
            var index = await _elasticsearchResource.CreateIndexAsync<Foo>();
            await _elasticsearchResource.CreateDocumentsAsync(index, firstDocumentRequest, secondDocumentRequest);

            // assert
            var searchResponse = await _elasticsearchResource.Client.LowLevel.SearchAsync<SearchResponse<Foo>>(
                index, typeof(Foo).Name.ToLowerInvariant(), PostData.String(string.Empty));
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
            var firstIndex = await _elasticsearchResource.CreateIndexAsync<Foo>();
            await _elasticsearchResource.CreateDocumentsAsync(firstIndex, firstDocumentRequest);
            var secondIndex = await _elasticsearchResource.CreateIndexAsync<Foo>();
            await _elasticsearchResource.CreateDocumentsAsync(secondIndex, secondDocumentRequest);

            // act
            var mergedIndex = await _elasticsearchResource.MergeIndicesAsync(firstIndex, secondIndex);

            // assert
            var searchResponse = await _elasticsearchResource.Client.LowLevel.SearchAsync<SearchResponse<Foo>>(
                mergedIndex, typeof(Foo).Name.ToLowerInvariant(), PostData.String(string.Empty));
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
}
