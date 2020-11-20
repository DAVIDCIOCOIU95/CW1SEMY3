using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NapierBankingApp.Services.Validation;

namespace NPMTest
{
    [TestClass]
    public class MessageValidatorTest
    {

        [TestMethod]
        public void ValidateHeader_WhenLengthIs10_ShouldPass()
        {
            string header = "S000000000";
            try
            {
                string validatedHeader = MessageValidator.ValidateHeader(header);
                Assert.AreEqual(header, validatedHeader);
            }
            catch (Exception ex)
            {
                Assert.Fail();
               
            }
        }

        [TestMethod]
        public void ValidateHeader_whenEmpty_ShouldThrowException()
        {
            string header = "";
            try
            {
                string validatedHeader = MessageValidator.ValidateHeader(header);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The header must have a length of 10.", ex.Message.ToString());
            }
        }

        [TestMethod]
        public void ValidateHeader_WhenLengthMinorThan10_ShouldThrowException()
        {
            string header = "S000";
            try
            {
                string validatedHeader = MessageValidator.ValidateHeader(header);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The header must have a length of 10.", ex.Message.ToString());
            }
        }

        [TestMethod]
        public void ValidateHeader_WhenLengthBiggerThan10_ShouldThrowException()
        {
            string header = "S000000000000";
            try
            {
                string validatedHeader = MessageValidator.ValidateHeader(header);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The header must have a length of 10.", ex.Message.ToString());
            }
        }

        // ValidateSender()

        [TestMethod]
        public void ValidateHeader_IfFirstLetterIs_S_ShouldPass()
        {
            string header = "S000000000";

            try
            {
                string validatedHeader = MessageValidator.ValidateHeader(header);
                Assert.AreEqual(header[0], validatedHeader[0]);
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ValidateHeader_IfFirstLetterIs_E_ShouldPass()
        {
            string header = "E000000000";

            try
            {
                string validatedHeader = MessageValidator.ValidateHeader(header);
                Assert.AreEqual(header[0], validatedHeader[0]);
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ValidateHeader_IfFirstLetterIs_T_ShouldPass()
        {
            string header = "T000000000";

            try
            {
                string validatedHeader = MessageValidator.ValidateHeader(header);
                Assert.AreEqual(header[0], validatedHeader[0]);
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
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

        [TestMethod]
        public void ValidateSender_SMS_BodySenderCorrect_ShouldPass()
        {
            List<string> fields = new List<string>() { "+098098098" };
            string senderRegex = @"^[\+0]\d{7,15}$";
            Dictionary<string, string> specialChars = new Dictionary<string, string>() { [" "] = "", ["    "] = "", ["_"] = "", ["-"] = "", ["#"] = "", ["*"] = "" };
            Assert.AreEqual(fields[0], MessageValidator.ValidateSender(fields, senderRegex, specialChars));
        }

        [TestMethod]
        public void ValidateSender_SMS_BodySenderCorrect_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "" };
            string senderRegex = @"^[\+]\d{7,15}$";
            Dictionary<string, string> specialChars = new Dictionary<string, string>() { [" "] = "", ["    "] = "", ["_"] = "", ["-"] = "", ["#"] = "", ["*"] = "" };
            Assert.ThrowsException<System.Exception>(() =>  MessageValidator.ValidateSender(fields, senderRegex, specialChars));
        }
    }
}
