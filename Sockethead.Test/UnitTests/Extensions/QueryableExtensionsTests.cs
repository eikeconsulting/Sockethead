using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sockethead.Common.Extensions;
using Sockethead.Test.UnitTests.Mocks;
using Xunit;

namespace Sockethead.Test.UnitTests.Extensions
{
    public class QueryableExtensionsTests
    {
        [Fact]
        public void If_WhenConditionTrue_ShouldApplyTransform()
        {
            IQueryable<int> source = Enumerable.Range(1, 5).AsQueryable();
            bool condition = true;

            IQueryable<int> result = source.If(condition, s => s.Skip(2));

            Assert.Equal(new[] { 3, 4, 5 }, result);
        }

        [Fact]
        public void If_WhenConditionFalse_ShouldNotApplyTransform()
        {
            IQueryable<int> source = Enumerable.Range(1, 5).AsQueryable();
            bool condition = false;

            IQueryable<int> result = source.If(condition, s => s.Skip(2));

            Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result);
        }

        [Fact]
        public void WhereIf_WhenConditionTrue_ShouldIncludePredicate()
        {
            IQueryable<int> source = Enumerable.Range(1, 5).AsQueryable();
            bool condition = true;
            Expression<Func<int, bool>> predicate = x => x > 3;

            IQueryable<int> result = source.WhereIf(condition, predicate);

            Assert.Equal(new[] { 4, 5 }, result);
        }

        [Fact]
        public void WhereIf_WhenConditionFalse_ShouldNotIncludePredicate()
        {
            IQueryable<int> source = Enumerable.Range(1, 5).AsQueryable();
            bool condition = false;
            Expression<Func<int, bool>> predicate = x => x > 3;

            IQueryable<int> result = source.WhereIf(condition, predicate);

            Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result);
        }

        [Fact]
        public void Paginate_ShouldReturnRequestedPage()
        {
            IQueryable<int> source = Enumerable.Range(1, 10).AsQueryable();
            int pageNumber = 2;
            int pageSize = 3;

            IQueryable<int> result = source.Paginate(pageNumber, pageSize);

            Assert.Equal(new[] { 7, 8, 9 }, result);
        }
        
        [Fact]
        public void ForEachInChunks_ShouldSplitCollectionIntoChunksAndInvokeActionForEachEntity()
        {
            List<int> collection = new() { 1, 2, 3, 4, 5 };
            int chunkSize = 2;
            List<int> processedEntities = new();
        
            collection.AsQueryable().ForEachInChunks(chunkSize, entity => processedEntities.Add(entity));
        
            Assert.Equal(5, processedEntities.Count);
            Assert.Equal(new List<int> { 1, 2, 3, 4, 5 }, processedEntities);
        }
        
        [Fact]
        public async Task ForEachInChunksAsync_ShouldSplitCollectionIntoChunksAndInvokeActionForEachEntity()
        {
            List<string> collection = new() { "1", "2", "3", "4", "5" };
            Mock<DbSet<string>> mockSet = MockAsyncQueryCollection.GetMockDbSet(collection.AsQueryable());
            int chunkSize = 2;
            List<string> processedEntities = new();

            await mockSet.Object.ForEachInChunksAsync(chunkSize, entity => processedEntities.Add(entity));
        
            Assert.Equal(collection.Count, processedEntities.Count);
            Assert.Equal(collection, processedEntities);
        }
        
        [Fact]
        public async Task ChunkAsync_ReturnsExpectedChunks()
        {
            List<string> collection = new() { "1", "2", "3", "4", "5" };
            Mock<DbSet<string>> mockSet = MockAsyncQueryCollection.GetMockDbSet(collection.AsQueryable());
            int chunkSize = 2;

            int count = 0;
            await foreach (var chunked in mockSet.Object.ChunkAsync(chunkSize))
                count += chunked.Count;

            Assert.Equal( collection.Count, count); 
        }
    }
}