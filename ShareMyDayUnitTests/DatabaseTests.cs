using NUnit.Framework;
using ShareMyDay;
using ShareMyDay.Database;


namespace Tests
{
    public class DatabaseTests
    {
        private Database testDatabase;
        [SetUp]
        public void Setup()
        {
           testDatabase = new Database();
        }

        [Test]
        public void CreateDatabase_CreatesCorrectly_ReturnsTrue()
        {
            Assert.True(testDatabase.CreateDatabase(), "CreateDatabase_CreatesCorrectly_ReturnsTrue: Returned false");
        }
    }
}