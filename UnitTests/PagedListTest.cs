using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NuGet.Protocol.Core.Types;
using Reddit;
using Reddit.Models;
using Reddit.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class PagedListTest
    {
        public IPostsRepository GetPostsRepostory()
        {

            var dbName = Guid.NewGuid().ToString();    
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new ApplicationDbContext(options);



            dbContext.Posts.Add(new Post { Title = "Title 1", Content = "Content 1", Upvotes = 5, Downvotes = 10 });
            dbContext.Posts.Add(new Post { Title = "Title 2", Content = "Content 2", Upvotes = 122, Downvotes = 19 });
            dbContext.Posts.Add(new Post { Title = "Title 3", Content = "Content 3", Upvotes = 35, Downvotes = 71 });
            dbContext.Posts.Add(new Post { Title = "Title 4", Content = "Content 4", Upvotes = 201, Downvotes = 11 });
            dbContext.Posts.Add(new Post { Title = "Title 5", Content = "Content 5", Upvotes = 5000, Downvotes = 23 });
            dbContext.Posts.Add(new Post { Title = "Title 6", Content = "Content 6", Upvotes = 50003, Downvotes = 2123 });
            dbContext.Posts.Add(new Post { Title = "Title 7", Content = "Content ", Upvotes = 3, Downvotes = 23 });
            dbContext.SaveChanges();

            return new PostsRepository(dbContext);

          
        }

        [Fact]
        public async Task GetPosts_PAgination()
        {
            var postRepository = GetPostsRepostory();
            var posts = await postRepository.GetPosts(1,2,null,null,null);

            Assert.Equal(2,posts.Items.Count);
            Assert.Equal(7, posts.TotalCount);
            Assert.True(posts.HasNextPage);
            Assert.False(posts.HasPreviousPage);
        }
        //aq roca zemdetia page numberi da daafeilebs
        /*  [Fact]
          public async Task GetPosts_PAgination_ReturnFailure()
          {
              var postRepository = GetPostsRepostory();
              var posts = await postRepository.GetPosts(1, 2, null, null, null);

              Assert.Equal(2, posts.Items.Count);
              Assert.Equal(8, posts.TotalCount);
              Assert.True(posts.HasNextPage);
              Assert.False(posts.HasPreviousPage);
          } */

        [Fact]
        public async Task GePosts_ReturnCorectAnswer()
        {
            var postRepository = GetPostsRepostory();
            var posts = await postRepository.GetPosts(1, 3, null, "Popular", null);

            Assert.Equal(3, posts.Items.Count);
            Assert.Equal(7, posts.TotalCount);
            Assert.True(posts.HasNextPage);
            Assert.False(posts.HasPreviousPage);
        }

        [Fact]
        public async Task GePosts_ReturnCorectAnswers()
        {
            var postRepository = GetPostsRepostory();
            var posts = await postRepository.GetPosts(1, 4, null, "positivity", null);

            Assert.Equal(4, posts.Items.Count);
            Assert.Equal(7, posts.TotalCount);
            Assert.True(posts.HasNextPage);
            Assert.False(posts.HasPreviousPage);
        }

        [Fact]
        public async Task GetPosts_InvalidPage_ThrowsExeptioms()
        {
            var repository = GetPostsRepostory();

            var argexeption = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetPosts(page: 0, pageSize: 12, searchTerm: null, SortTerm: null));
            Assert.Equal("page", argexeption.ParamName);
        
        }


        [Fact]
        public async Task GEtPosts_InvalidPageSize_ThrowException()
        {
            var repository = GetPostsRepostory();

            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetPosts(page: 3, pageSize: 0, searchTerm: null, SortTerm: null));
            Assert.Equal("pageSize", exception.ParamName);
        
        }


        


    }

}
