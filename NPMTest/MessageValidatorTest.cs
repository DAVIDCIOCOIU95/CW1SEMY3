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

        [TestMethod]
        public void ValidateHeader_IfFirstLetterIs_S_E_T_ShouldPass()
        {
            string header = "P123456789";

            try
            {
                string validatedHeader = MessageValidator.ValidateHeader(header);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The header must start with S, E or T", ex.Message.ToString());
            }
        }
    }
}
