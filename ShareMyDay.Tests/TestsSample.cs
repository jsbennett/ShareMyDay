using System;
using NUnit.Framework;

namespace ShareMyDay.Tests
{
    /*
     * This Class is used sample class of how to do testing 
     */
    [TestFixture]
    public class TestsSample
    {

        [SetUp]
        public void Setup()
        {
           
        }

        [TearDown]
        public void Tear() { }

        [Test]
        public void Pass()
        {
            Console.WriteLine("test1");
            Assert.True(true);
        }

        [Test]
        public void Fail()
        {
            Assert.False(true);
        }

        [Test]
        [Ignore("another time")]
        public void Ignore()
        {
            Assert.True(false);
        }

        [Test]
        public void Inconclusive()
        {
            Assert.Inconclusive("Inconclusive");
        }
    }
}
