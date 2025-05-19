using NUnit.Framework;
using Indiego_Backend.Services;
using Indiego_Backend.Settings;

namespace Indiego_Backend.Tests
{
    public class DatabaseServiceTest
    {
        private DatabaseSetting _databaseSetting = null!;
        private DatabaseService _databaseService = null!;
        
        [SetUp]
        public void Setup()
        {
            _databaseSetting = new DatabaseSetting
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "indiego_test_db"
            };
            
            _databaseService = new DatabaseService(_databaseSetting);
        }

        [Test]
        public void GenreCollection_ShouldReturnCorrectCollectionName()
        {
            // Act
            var collection = _databaseService.Genres;
            
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.CollectionNamespace.CollectionName, Is.EqualTo("genres"));
        }

        [Test]
        public void GameCollection_ShouldReturnCorrectCollectionName()
        {
            // Act
            var collection = _databaseService.Games;
            
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.CollectionNamespace.CollectionName, Is.EqualTo("games"));
        }

        [Test]
        public void SubscriptionCollection_ShouldReturnCorrectCollectionName()
        {
            // Act
            var collection = _databaseService.Subscriptions;
            
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.CollectionNamespace.CollectionName, Is.EqualTo("subscriptions"));
        }

        [Test]
        public void ReviewCollection_ShouldReturnCorrectCollectionName()
        {
            // Act
            var collection = _databaseService.Reviews;
            
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.CollectionNamespace.CollectionName, Is.EqualTo("reviews"));
        }

        [Test]
        public void PostCollection_ShouldReturnCorrectCollectionName()
        {
            // Act
            var collection = _databaseService.Posts;
            
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.CollectionNamespace.CollectionName, Is.EqualTo("posts"));
        }

        [Test]
        public void SubscriptionTypeCollection_ShouldReturnCorrectCollectionName()
        {
            // Act
            var collection = _databaseService.SubscriptionTypes;
            
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.CollectionNamespace.CollectionName, Is.EqualTo("subscriptionTypes"));
        }

        [Test]
        public void UserCollection_ShouldReturnCorrectCollectionName()
        {
            // Act
            var collection = _databaseService.Users;
            
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.CollectionNamespace.CollectionName, Is.EqualTo("users"));
        }
        
        [Test]
        public void CustomerCollection_ShouldReturnCorrectCollectionName()
        {
            // Act
            var collection = _databaseService.Customers;
            
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.CollectionNamespace.CollectionName, Is.EqualTo("users"));
        }
        
        [Test]
        public void DeveloperCollection_ShouldReturnCorrectCollectionName()
        {
            // Act
            var collection = _databaseService.Developers;
            
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.CollectionNamespace.CollectionName, Is.EqualTo("users"));
        }
        
        [Test]
        public void AdminCollection_ShouldReturnCorrectCollectionName()
        {
            // Act
            var collection = _databaseService.Admins;
            
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.CollectionNamespace.CollectionName, Is.EqualTo("users"));
        }
    }
}