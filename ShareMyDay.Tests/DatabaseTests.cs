using System;
using NUnit.Framework;

namespace ShareMyDay.Tests
{
    /*
     * This Class is used to test the database functionality  of the application 
     */
    [TestFixture]
    public class DatabaseTests
    {
        private Database.Database _db;

        [SetUp]
        public void Setup()
        {
            _db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");
        }


        [TearDown]
        public void Tear() { }

        /*
         * Method Name: CreateConnection_CreatesDatabaseObject_ReturnsObject
         * Purpose: This test is used to test whether a connection is made with the connection details
         * Passes if the value returned from CreateConnection() is not null
         */
        [Test]
        public void CreateConnection_CreatesDatabaseObject_ReturnsObject()
        {
            Assert.IsNotNull(_db.CreateConnection(),"CreateConnection_CreatesDatabaseObject_ReturnsObject: Null Returned");
         
        }

        /*
         * Method Name: DatabaseDefaultSetup_InsertsDataCorrectly_ReturnsACountOf6
         * Purpose: This test is used to test whether the CardTypes are inserted into the table 
         * Passes if the value returned from DatabaseDefaultSetup is 6 (which is the number of inserted into the database)
         */
        [Test]
        public void Setup_InsertsDataCorrectly_ReturnsACountOf6()
        {
            _db.Create();
            const int expected = 6;
            var actual = _db.Setup();
            Assert.AreEqual(expected, actual,"DatabaseDefaultSetup_InsertsDataCorrectly_ReturnsACountOf6: Not inserted all CardTypes into database");
        }
    }
}
