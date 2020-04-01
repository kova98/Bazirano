using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Bazirano.Scraper;

namespace Bazirano.Tests.Scraper
{
    public class InMemoryPostedArticlesRepositoryTests
    {
        [Fact]
        void AddArticle_AddsArticle()
        {
            var repo = new InMemoryPostedArticlesRepository();
            var article = new Article();

            repo.AddArticle(article);

            Assert.Contains(article, repo.PostedArticles);
        }

        [Theory]
        [InlineData(49)]
        [InlineData(50)]
        void AddArticle_NumberOfArticlesContained_RemainsAt50Articles(int numberOfArticles)
        {
            var repo = new InMemoryPostedArticlesRepository();
            for (int i = 0; i < numberOfArticles; i++)
            {
                repo.PostedArticles.Add(GetTestArticle());
            }

            repo.AddArticle(new Article { KeywordsList = new string[] { "test" } });

            Assert.True(repo.PostedArticles.Count == 50);
        }

        [Fact]
        void AddArticle_MoreThan50Articles_RemovesOldestArticle()
        {
            var repo = new InMemoryPostedArticlesRepository();
            var oldestArticle = GetTestArticle();
            repo.PostedArticles.Add(oldestArticle);
            var secondOldestArticle = GetTestArticle();
            repo.PostedArticles.Add(secondOldestArticle);
            for (int i = 0; i < 49; i++)
            {
                repo.PostedArticles.Add(GetTestArticle());
            }

            repo.AddArticle(GetTestArticle());
            repo.AddArticle(GetTestArticle());

            Assert.DoesNotContain(oldestArticle, repo.PostedArticles);
            Assert.DoesNotContain(secondOldestArticle, repo.PostedArticles);
        }

        [Theory]
        [InlineData(true, "test1", "test2", "test3", "test4", "test5")]
        [InlineData(false, "test1", "test2", "test3", "test4", "test5", "test6")]
        [InlineData(false, "test1", "test2", "test3", "test4", "test5", "test6", "test7")]
        void AddArticle_RepoContainsArticleWithMoreThan5MatchingWords_DoesNotAddArticle(bool expected, params string[] keywords)
        {
            var repo = new InMemoryPostedArticlesRepository();
            repo.PostedArticles.Add(new Article
            {
                KeywordsList = keywords
            });
            var article = new Article
            {
                KeywordsList = new string[] { "test1", "test2", "test3", "test4", "test5", "test6", "test7" }
            };

            repo.AddArticle(article);

            Assert.Equal(expected, repo.PostedArticles.Contains(article));
        }

        private Article GetTestArticle()
        {
            return new Article
            {
                KeywordsList = new string[] { "test" }
            };
        }
    }
}
