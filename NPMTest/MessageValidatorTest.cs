using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NapierBankingApp.Services.Validation;

namespace NPMTest
{
    [TestClass]
    public class MessageValidatorTest
    {
        #region Header Validation
        [TestMethod]
        public void ValidateHeader_WhenLengthIs10_ShouldPass()
        {
            string header = "S000000000";
            Assert.AreEqual(header, MessageValidator.ValidateHeader(header));
        }

        [TestMethod]
        public void ValidateHeader_whenEmpty_ShouldThrowException()
        {
            string header = "";
            Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateHeader(header));
        }

        [TestMethod]
        public void ValidateHeader_WhenLengthMinorThan10_ShouldThrowException()
        {
            string header = "S000";
            Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateHeader(header));
        }

        [TestMethod]
        public void ValidateHeader_WhenLengthBiggerThan10_ShouldThrowException()
        {
            string header = "S000000000000";
            Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateHeader(header));
        }


        [TestMethod]
        public void ValidateHeader_IfFirstLetterIs_S_ShouldPass()
        {
            string header = "S000000000";
            Assert.AreEqual(header[0], MessageValidator.ValidateHeader(header)[0]);
        }

        [TestMethod]
        public void ValidateHeader_IfFirstLetterIs_E_ShouldPass()
        {
            string header = "E000000000";
            Assert.AreEqual(header[0], MessageValidator.ValidateHeader(header)[0]);


        }

        [TestMethod]
        public void ValidateHeader_IfFirstLetterIs_T_ShouldPass()
        {
            string header = "T000000000";
            Assert.AreEqual(header[0], MessageValidator.ValidateHeader(header)[0]);
        }

        [TestMethod]
        public void ValidateHeader_IfFirstLetterIs_X_ShouldThrowException()
        {
            string header = "X000000000";

                Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateHeader(header));
        }

        [TestMethod]
        public void ValidateHeader_IfFirstLetterIs_Y_ShouldThrowException()
        {
            string header = "Y000000000";

            Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateHeader(header));
        }

        [TestMethod]
        public void ValidateHeader_FirstLetterFollowedByOnlyNumbers_ShouldPass()
        {
            string header = "S000000000";

            Assert.AreEqual(header, MessageValidator.ValidateHeader(header));
        }

        [TestMethod]
        public void ValidateHeader_FirstLetterFollowedByOnlyNumbers_ShouldThrowException()
        {
            string header = "S000000XYZ";

            Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateHeader(header));
        }

        [TestMethod]
        public void ValidateHeader_LowerCaseIsCapitalized_ShouldPass()
        {
            string header = "s000000000";
            string expectedHeader = "S000000000";

            Assert.AreEqual(expectedHeader, MessageValidator.ValidateHeader(header));
        }
        #endregion

        #region Sender Validation
        // SMS Sender Validation
        [TestMethod]
        public void ValidateSender_SMS_BodySenderCorrect_ShouldPass()
        {
            List<string> fields = new List<string>() { "+1234567" };
            string senderRegex = @"^[\+0]\d{7,15}$";
            Dictionary<string, string> specialChars = new Dictionary<string, string>() { [" "] = "", ["    "] = "", ["_"] = "", ["-"] = "", ["#"] = "", ["*"] = "" };
            Assert.AreEqual(fields[0], MessageValidator.ValidateSender(fields, senderRegex, specialChars));
        }

        [TestMethod]
        public void ValidateSender_SMS_BodySenderEmpty_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "" };
            string senderRegex = @"^[\+]\d{7,15}$";
            Dictionary<string, string> specialChars = new Dictionary<string, string>() { [" "] = "", ["    "] = "", ["_"] = "", ["-"] = "", ["#"] = "", ["*"] = "" };
            Assert.ThrowsException<System.Exception>(() =>  MessageValidator.ValidateSender(fields, senderRegex, specialChars));
        }

        [TestMethod]
        public void ValidateSender_SMS_BodySenderLengthLessThan7_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "+123456" };
            string senderRegex = @"^[\+]\d{7,15}$";
            Dictionary<string, string> specialChars = new Dictionary<string, string>() { [" "] = "", ["    "] = "", ["_"] = "", ["-"] = "", ["#"] = "", ["*"] = "" };
            Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateSender(fields, senderRegex, specialChars));
        }

        [TestMethod]
        public void ValidateSender_SMS_BodySenderLengthMoreThan15_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "+1234567890123456" };
            string senderRegex = @"^[\+]\d{7,15}$";
            Dictionary<string, string> specialChars = new Dictionary<string, string>() { [" "] = "", ["    "] = "", ["_"] = "", ["-"] = "", ["#"] = "", ["*"] = "" };
            Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateSender(fields, senderRegex, specialChars));
        }

        // Tweet Sender Validation
        [TestMethod]
        public void ValidateSender_Tweet_BodySenderEmpty_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "" };
            string senderRegex = @"^\@[a-zA-Z0-9_]{1,15}$";
            Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateSender(fields, senderRegex));
        }

        [TestMethod]
        public void ValidateSender_Tweet_BodySenderCorrect_ShouldPass()
        {
            List<string> fields = new List<string>() { "@david" };
            string senderRegex = @"^\@[a-zA-Z0-9_]{1,15}$";
            Assert.AreEqual(fields[0], MessageValidator.ValidateSender(fields, senderRegex));
        }

        [TestMethod]
        public void ValidateSender_Tweet_BodySenderLengthMax15_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "@1234567890123456" };
            string senderRegex = @"^\@[a-zA-Z0-9_]{1,15}$";
            Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateSender(fields, senderRegex));
        }

        [TestMethod]
        public void ValidateSender_Tweet_BodySenderNotInTweetIdForm_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "david" };
            string senderRegex = @"^\@[a-zA-Z0-9_]{1,15}$";
            Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateSender(fields, senderRegex));
        }

        // Email Sender Validation
        [TestMethod]
        public void ValidateSender_Email_BodySenderEmpty_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "" };
            string senderRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateSender(fields, senderRegex));
        }

        [TestMethod]
        public void ValidateSender_Email_BodySenderCorrect_ShouldPass()
        {
            List<string> fields = new List<string>() { "david@gmai.com" };
            string senderRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            Assert.AreEqual(fields[0], MessageValidator.ValidateSender(fields, senderRegex));
        }

        [TestMethod]
        public void ValidateSender_Email_BodySenderNotEmailFormat_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "david@" };
            string senderRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateSender(fields, senderRegex));
        }

        #endregion

        #region Text Validation
        [TestMethod]
        public void ValidateText_SMS_BodyTextLenghtSmallerEqualThan140_ShouldPass()
        {
            List<string> fields = new List<string>() { "+1234567", "hello" };
            Assert.AreEqual(fields[1], MessageValidator.ValidateText(fields, 1, 140));
        }

        [TestMethod]
        public void ValidateText_SMS_BodyTextLenghtBiggerThan140_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "+1234567", "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.\"" };
            Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateText(fields, 1, 140));
        }

        [TestMethod]
        public void ValidateText_Tweet_BodyTextLenghtSmallerEqualThan140_ShouldPass()
        {
            List<string> fields = new List<string>() { "@david", "hello" };
          
            Assert.AreEqual(fields[1], MessageValidator.ValidateText(fields, 1, 140));
        }

        [TestMethod]
        public void ValidateText_Tweet_BodyTextLenghtBiggerThan140_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "@david", "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.\"" };
         
            Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateText(fields, 1, 140));
        }

        [TestMethod]
        public void ValidateText_Email_SEM_BodyTextLenghtSmallerEqualThan1028_ShouldPass()
        {
            List<string> fields = new List<string>() { "david@gmai.com", "This is the subject", "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt.\"" };
            Assert.AreEqual(fields[2], MessageValidator.ValidateText(fields, 2, 1028));
        }

        [TestMethod]
        public void ValidateText_Email_SIR_BodyTextLenghtSmallerEqualThan1028_ShouldPass()
        {
            List<string> fields = new List<string>() { "david@gmai.com", "SIR 02/07/1995", "99-99-99", "Theft", "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt.\"" };
            Assert.AreEqual(fields[4], MessageValidator.ValidateText(fields, 4, 1028));
        }

        [TestMethod]
        public void ValidateText_Email_SEM_BodyTextLenghtBiggerThan1028_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "david@gmai.com", "This is the subject", "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequatLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.\"" };
            Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateText(fields, 2, 1028));
        }

        [TestMethod]
        public void ValidateText_Email_SIR_BodyTextLenghtBiggerThan1028_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "david@gmai.com", "SIR 02/07/1995", "99-99-99", "Theft", "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequatLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.\"" };
            Assert.ThrowsException<System.Exception>(() => MessageValidator.ValidateText(fields, 4, 1028));
        }

        #endregion
    }
}
