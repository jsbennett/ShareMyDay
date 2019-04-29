using Android.Nfc;
using NUnit.Framework;
using ShareMyDay.NFC.NFC_Functions;

namespace ShareMyDay.Tests
{
    /*
     * This Class is used to test the NFC functionality  of the application - Please note that only the main parts of the functionality is tested, with more time the author would have done more comprehensive testing
     * Since most of the NFC functionality requires an NFC card to be tapped, the author could not emulate this 
     */
    [TestFixture]
    public class NFCTests
    {
        private NfcWrite _nfcWriter;

        [SetUp]
        public void Setup()
        {
           _nfcWriter = new NfcWrite(null,null,null,"Swimming","1");
           
        }

        /*
         * Method Name: CreateCardContent_WritesContentForCard_ReturnsNdefMessage
         * Purpose: This test is used to test whether it creates a NDEF Message
         */
        [Test]
        public void CreateCardContent_WritesContentForCard_ReturnsNdefMessage()
        {
            Assert.IsInstanceOfType(typeof(NdefMessage),NfcWrite.CreateCardContent(), "GetFileLocation_LocatesTheFileToStorePicture_ReturnsFileType: Did not return file of type file");
         
        }     
    }
}
