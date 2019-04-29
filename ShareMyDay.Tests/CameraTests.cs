using Android.Net;
using Java.IO;
using NUnit.Framework;

namespace ShareMyDay.Tests
{
    /*
     * This Class is used to test the voice recording functionality  of the application - Please note that only the main parts of the functionality is tested, with more time the author would have done more comprehensive testing 
     */
    [TestFixture]
    public class CameraTests
    {
        private Camera.Camera camera;

        [SetUp]
        public void Setup()
        {
            camera = new Camera.Camera();
           
        }

        /*
         * Method Name: GetFileLocation_LocatesTheFileToStorePicture_ReturnsFileType
         * Purpose: This test is used to test whether it finds the the location on the device to store the photos 
         */
        [Test]
        public void GetFileLocation_LocatesTheFileToStorePicture_ReturnsFileType()
        {
            Assert.IsInstanceOfType(typeof(File),camera.GetFileLocation(), "GetFileLocation_LocatesTheFileToStorePicture_ReturnsFileType: Did not return file of type file");
         
        }
        
        /*
         * Method Name: CreateFolder_LocatesTheFolderToStorePictures_ReturnsFileType
         * Purpose: This test is used to test whether it finds the the folder for pictures on the device to store the photos 
         */
        [Test]
        public void CreateFolder_LocatesTheFolderToStorePictures_ReturnsFileType()
        {
            var location = camera.GetFileLocation();
            Assert.IsInstanceOfType(typeof(File),camera.CreateFolder(location), "CreateFolder_LocatesTheFolderToStorePictures_ReturnsFileType: Did not return file of type file");
         
        } 
        
        /*
         * Method Name: CreateFolder_LocatesTheFolderToStorePictures_ReturnsNotNull
         * Purpose: This test is used to test whether it finds the the folder for pictures on the device to store the photos 
         */
        [Test]
        public void CreateImageFile_CreatesAnImageFile_ReturnsImageOfTypeFile()
        {
            var location = camera.GetFileLocation();
            location = camera.CreateFolder(location);
            Assert.IsInstanceOfType(typeof(File), camera.CreateImageFile(location), "CreateImageFile_CreatesAnImageFile_ReturnsImageOfTypeFile: Did not return image of type File");
        }
        
        /*
         * Method Name: GetUri_FindsTheUriOfImage_ReturnsUri
         * Purpose: This test is used to test whether it finds the the folder for pictures on the device to store the photos 
         */
        [Test]
        public void GetUri_FindsTheUriOfImage_ReturnsUri()
        {
            var location = camera.GetFileLocation();
            location = camera.CreateFolder(location);
            var image = camera.CreateImageFile(location); 
            Assert.IsInstanceOfType(typeof(Uri), camera.GetUri(image), "camera.CreateImageFile(location): Did not return image URI");
        }



       
    }
}
