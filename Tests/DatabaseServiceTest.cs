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
        
        [Test]
        public void DatabaseService_ShouldImplementIDatabaseServiceInterface()
        {
            // Assert
            Assert.That(_databaseService, Is.InstanceOf<IDatabaseService>());
        }
    }
}