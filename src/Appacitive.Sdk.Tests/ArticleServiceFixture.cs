﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class ArticleServiceFixture
    {

        [TestMethod]
        public async Task CreateArticleAsyncTest()
        {
            // Create article
            var now = DateTime.Now;
            dynamic obj = ObjectHelper.NewInstance();

            var service = ObjectFactory.Build<IArticleService>();
            CreateArticleResponse response = null;
            var waitHandle = new ManualResetEvent(false);

            response = await service.CreateArticleAsync(new CreateArticleRequest()
            {
                Article = obj,
                Environment = TestConfiguration.Environment
            });
            waitHandle.Set();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Status);
            Assert.IsTrue(response.Status.IsSuccessful);
            Assert.IsNotNull(response.Article);
            Console.WriteLine("Created article id {0}", response.Article.Id);
            Console.WriteLine("Time taken {0} seconds", response.TimeTaken);
        }

        [TestMethod]
        public async Task GetArticleAsyncTest()
        {
            // Create article
            var now = DateTime.Now;
            var article = await ObjectHelper.CreateNewAsync();


            // Get the article
            IArticleService service = new ArticleService();
            GetArticleResponse getResponse = null;
            getResponse = await service.GetArticleAsync(
                new GetArticleRequest()
                {   
                    Id = article.Id,
                    Type = article.Type
                });
            Assert.IsNotNull(getResponse);
            Assert.IsNotNull(getResponse.Status);
            Assert.IsTrue(getResponse.Status.IsSuccessful);
            Assert.IsNotNull(getResponse.Article);
            Console.WriteLine("Successfully read article id {0}", getResponse.Article.Id);
            Console.WriteLine("Time taken: {0} seconds", getResponse.TimeTaken);

        }

        [TestMethod]
        public void DeleteArticleAsyncTest()
        {
            Exception fault = null;
            var waitHandle = new ManualResetEvent(false);
            var action = new Action(async () =>
                {
                    try
                    {
                        // Create article
                        var now = DateTime.Now;
                        dynamic obj = new Article("object");
                        obj.intfield = 1;
                        obj.decimalfield = 10.0m;
                        obj.datefield = "2012-12-20";
                        obj.datetimefield = now.ToString("o");
                        obj.stringfield = "string value";
                        obj.textfield = "text value";
                        obj.boolfield = false;
                        obj.geofield = "11.5,12.5";
                        obj.listfield = "a";
                        obj.SetAttribute("attr1", "value1");
                        obj.SetAttribute("attr2", "value2");

                        var service = ObjectFactory.Build<IArticleService>();
                        CreateArticleResponse response = null;
                        response = await service.CreateArticleAsync(new CreateArticleRequest()
                        {
                            Article = obj
                        });
                        Assert.IsNotNull(response);
                        Assert.IsNotNull(response.Status);
                        Assert.IsTrue(response.Status.IsSuccessful);
                        Assert.IsNotNull(response.Article);
                        Console.WriteLine("Created article id {0}", response.Article.Id);
                        Console.WriteLine("Time taken: {0} seconds", response.TimeTaken);

                        // Delete the article
                        Status deleteArticleResponse = null;
                        deleteArticleResponse = await service.DeleteArticleAsync(new DeleteArticleRequest()
                        {
                            Id = response.Article.Id,
                            Type = response.Article.Type
                        });
                        Assert.IsNotNull(deleteArticleResponse, "Delete articler response is null.");
                        Assert.IsTrue(deleteArticleResponse.IsSuccessful == true, deleteArticleResponse.Message ?? "Delete article operation failed.");

                        // Try get the deleted article
                        var getArticleResponse = await service.GetArticleAsync(
                            new GetArticleRequest()
                            {
                                Id = response.Article.Id,
                                Type = response.Article.Type
                            });
                        Assert.IsNotNull(getArticleResponse, "Get article response is null.");
                        Assert.IsNull(getArticleResponse.Article, "Should not be able to get a deleted article.");
                        Assert.IsTrue(getArticleResponse.Status.Code == "404", "Error code expected was not 404.");
                    }
                    catch (Exception ex)
                    {
                        fault = ex;
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                });
            action();
            waitHandle.WaitOne();
            Assert.IsNull(fault);
        }

        [TestMethod]
        public void UpdateArticleAsyncTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
                {
                    try
                    {
                        // Create an article
                        var now = DateTime.Now;
                        dynamic obj = new Article("object");
                        obj.intfield = 1;
                        obj.decimalfield = 10.0m;
                        obj.datefield = "2012-12-20";
                        obj.stringfield = "initial";
                        obj.Tags.Add("initial");

                        var service = ObjectFactory.Build<IArticleService>();
                        var createdResponse = await service.CreateArticleAsync(new CreateArticleRequest()
                        {
                            Article = obj
                        });
                        Assert.IsNotNull(createdResponse, "Article creation failed.");
                        Assert.IsNotNull(createdResponse.Status, "Status is null.");
                        Assert.IsTrue(createdResponse.Status.IsSuccessful, createdResponse.Status.Message ?? "Create article failed.");
                        var created = createdResponse.Article;

                        // Update the article
                        var updateRequest = new UpdateArticleRequest()
                        {
                            Id = created.Id,
                            Type = created.Type
                        };
                        updateRequest.PropertyUpdates["intfield"] = "2";
                        updateRequest.PropertyUpdates["decimalfield"] = 20.0m.ToString();
                        updateRequest.PropertyUpdates["stringfield"] = null;
                        updateRequest.PropertyUpdates["datefield"] = "2013-11-20";
                        updateRequest.AddedTags.AddRange(new[] { "tag1", "tag2" });
                        updateRequest.RemovedTags.AddRange(new[] { "initial" });
                        var updatedResponse = await service.UpdateArticleAsync(updateRequest);

                        Assert.IsNotNull(updatedResponse, "Update article response is null.");
                        Assert.IsNotNull(updatedResponse.Status, "Update article response status is null.");
                        Assert.IsNotNull(updatedResponse.Article, "Updated article is null.");
                        var updated = updatedResponse.Article;
                        Assert.IsTrue(updated["intfield"] == "2");
                        Assert.IsTrue(updated["decimalfield"] == "20.0");
                        Assert.IsTrue(updated["stringfield"] == null);
                        Assert.IsTrue(updated["datefield"] == "2013-11-20");
                        Assert.IsTrue(updated.Tags.Count() == 2);
                        Assert.IsTrue(updated.Tags.Intersect(new[] { "tag1", "tag2" }).Count() == 2);
                    }
                    catch (Exception ex)
                    {
                        fault = ex;
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                };
            action();
            waitHandle.WaitOne();
            Assert.IsNull(fault);

        }

        [TestMethod]
        public async Task BugId14Test()
        {
            // Ref: https://github.com/appacitive/Gossamer/issues/14
            // Updating a null property with a null value fails 
            // Create an article
            var now = DateTime.Now;
            dynamic obj = new Article("object");
            obj.intfield = 1;
            obj.decimalfield = 10.0m;
            obj.stringfield = null;

            var service = ObjectFactory.Build<IArticleService>();
            var createdResponse = await service.CreateArticleAsync(new CreateArticleRequest()
            {
                Article = obj
            });
            Assert.IsNotNull(createdResponse, "Article creation failed.");
            Assert.IsNotNull(createdResponse.Status, "Status is null.");
            Assert.IsTrue(createdResponse.Status.IsSuccessful, createdResponse.Status.Message ?? "Create article failed.");
            var created = createdResponse.Article;

            // Update the article twice
            for (int i = 0; i < 2; i++)
            {
                var updateRequest = new UpdateArticleRequest()
                {
                    Id = created.Id,
                    Type = created.Type
                };
                updateRequest.PropertyUpdates["stringfield"] = null;
                var updatedResponse = await service.UpdateArticleAsync(updateRequest);

                Assert.IsNotNull(updatedResponse, "Update article response is null.");
                Assert.IsNotNull(updatedResponse.Status, "Update article response status is null.");
                Assert.IsTrue(updatedResponse.Status.IsSuccessful, updatedResponse.Status.Message ?? "NULL");
                Assert.IsNotNull(updatedResponse.Article, "Updated article is null.");
                var updated = updatedResponse.Article;
                Assert.IsTrue(updated["stringfield"] == null);   
            }
        }

        [TestMethod]
        public async Task FindAllArticleAsyncFixture()
        {
            // Create atleast one article
            var now = DateTime.Now;
            dynamic obj = new Article("object");
            obj.intfield = 66666;
            obj.decimalfield = 98765.0m;
            obj.datefield = "2012-12-20";
            obj.datetimefield = now.ToString("o");
            obj.stringfield = "Frank";
            obj.textfield = "Frand Sinatra was an amazing singer.";
            obj.boolfield = false;
            obj.geofield = "11.5,12.5";
            obj.listfield = "a";


            var service = ObjectFactory.Build<IArticleService>();
            CreateArticleResponse response = null;
            response = await service.CreateArticleAsync(new CreateArticleRequest()
            {
                Article = obj
            });

            ApiHelper.EnsureValidResponse(response);

            // Find all articles
            var findRequest = new FindAllArticleRequest() { Type = "object" };
            var findResponse = await service.FindAllAsync(findRequest);
            ApiHelper.EnsureValidResponse(findResponse);
            findResponse.Articles.ForEach(x => Console.WriteLine("Found article id {0}.", x.Id));
            Assert.IsNotNull(findResponse.PagingInfo);
            Assert.IsTrue(findResponse.PagingInfo.PageNumber == 1);
            Assert.IsTrue(findResponse.PagingInfo.TotalRecords > 0);
            Console.WriteLine("Paging info => pageNumber: {0}, pageSize: {1}, totalRecords: {2}",
                findResponse.PagingInfo.PageNumber,
                findResponse.PagingInfo.PageSize,
                findResponse.PagingInfo.TotalRecords);
        }

    }
}
