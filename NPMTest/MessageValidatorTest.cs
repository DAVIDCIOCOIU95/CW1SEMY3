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
        public void ValidateHeader_WhenMinorThan10_ShouldThrowException()
        {
            string header = "123";
            try
            {
                MessageValidator.ValidateHeader(header);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The header must have a length of 10.", ex.Message.ToString());
            }
        }

        [TestMethod]
        public void ValidateHeader_whenEmpty_ShouldThrowException()
        {
            string header = "";
            try
            {
                MessageValidator.ValidateHeader(header);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The header must have a length of 10.", ex.Message.ToString());
            }
        }

        [TestMethod]
        public void ValidateHeader_whenBiggerThan10_ShouldThrowException()
        {
            string header = "12345678910";
            try
            {
                MessageValidator.ValidateHeader(header);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The header must have a length of 10.", ex.Message.ToString());
            }
        }

        [TestMethod]
        public void ValidateHeader_IfFirstLetterNot_S_E_T_ShouldThrowException()
        {
            string header = "P123456789";

            try
            {
                MessageValidator.ValidateHeader(header);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The header must start with S, E or T", ex.Message.ToString());
            }
        }

        [TestMethod]
        public void ValidateHeader_IfNonNumericAfterType_ShouldThrowException()
        {
            string header = "S12345678A";

            try
            {
                MessageValidator.ValidateHeader(header);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The header type must be followed by only numeric characters.", ex.Message.ToString());
            }
        }
    }
}
