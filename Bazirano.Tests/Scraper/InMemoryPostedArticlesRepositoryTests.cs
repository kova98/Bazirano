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
                repo.PostedArticles.Add(GetTestArticle(1));
            }

            repo.AddArticle(GetTestArticle());

            Assert.True(repo.PostedArticles.Count == 50);
        }

        [Fact]
        void AddArticle_MoreThan50Articles_RemovesOldestArticle()
        {
            var repo = new InMemoryPostedArticlesRepository();
            var oldestArticle = GetTestArticle(1);
            repo.PostedArticles.Add(oldestArticle);
            var secondOldestArticle = GetTestArticle();
            repo.PostedArticles.Add(secondOldestArticle);
            for (int i = 0; i < 49; i++)
            {
                repo.PostedArticles.Add(GetTestArticle());
            }

            repo.AddArticle(GetTestArticle(2));
            repo.AddArticle(GetTestArticle(3));

            Assert.DoesNotContain(oldestArticle, repo.PostedArticles);
            Assert.DoesNotContain(secondOldestArticle, repo.PostedArticles);
        }

        [Theory]
        [InlineData(true, "test1", "test2", "test3", "test4", "test5")]
        [InlineData(false, "test1", "test2", "test3", "test4", "test5", "test6")]
        [InlineData(false, "test1", "test2", "test3", "test4", "test5", "test6", "test7")]
        void AddArticle_RepoContainsArticleWithMoreThan5MatchingWords_DoesNotAddArticle(bool articleGetsAdded, params string[] keywords)
        {
            var repo = new InMemoryPostedArticlesRepository();
            repo.PostedArticles.Add(new Article { KeywordsList = keywords, Guid = 1 });
            var article = GetTestArticle(2, "test1", "test2", "test3", "test4", "test5", "test6", "test7");

            repo.AddArticle(article);

            Assert.Equal(articleGetsAdded, repo.PostedArticles.Contains(article));
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, false)]
        [InlineData(2, true)]
        void AddArticle_ContainsGuid_DoesNotAddArticle(int guid, bool articleGetsAdded)
        {
            var repo = new InMemoryPostedArticlesRepository();
            repo.PostedArticles.Add(GetTestArticle(guid));
            var article = GetTestArticle(1);

            repo.AddArticle(article);

            Assert.Equal(articleGetsAdded, repo.PostedArticles.Contains(article));
        }

        private Article GetTestArticle(int guid = 0, params string[] keywords)
        {
            if (keywords.Length == 0)
            {
                keywords = new string[] { "test" };
            }

            return new Article { Guid = guid, KeywordsList = keywords };
        }
    }
}
