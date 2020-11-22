using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NapierBankingApp.Services.Validation;

namespace NapierBankingAppTests
{
    [TestClass]
    public class EmailValidatorTest: EmailValidator
    {
        // Validate Subject
        [TestMethod]
        public void ValidateSubject_Email_SIR_SubjectSpecified_ShouldPass()
        {
            List<string> fields = new List<string>() { "david@gmai.com", "SIR 02/07/1995", "99-99-99", "Theft", "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt.\"" };
            Assert.AreEqual("SIR", EmailValidator.ValidateSubject(fields, 1, @"^SIR \d{1,2}/\d{1,2}/\d{4}$", @"^[a-zA-Z0-9_]{0,20}"));
        }

        [TestMethod]
        public void ValidateSubject_Email_Sem_SubjectSpecified_ShouldPass()
        {
            List<string> fields = new List<string>() { "david@gmai.com", "Hello World", "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt.\"" };
            Assert.AreEqual("SEM", EmailValidator.ValidateSubject(fields, 1, @"^SIR \d{1,2}/\d{1,2}/\d{4}$", @"^[a-zA-Z0-9_]{0,20}"));
        }

        [TestMethod]
        public void ValidateSubject_Email_MissingSubject_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "david@gmai.com", "", "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt.\"" };
            Assert.ThrowsException<System.Exception>(() => EmailValidator.ValidateSubject(fields, 1, @"^SIR \d{1,2}/\d{1,2}/\d{4}$", @"^[a-zA-Z0-9_]{0,20}"));
        }

        [TestMethod]
        public void ValidateSubject_Email_SubjectLongerThan20Chars_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "david@gmai.com", "123456789012345678901", "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt.\"" };
            Assert.ThrowsException<System.Exception>(() => EmailValidator.ValidateSubject(fields, 1, @"^SIR \d{1,2}/\d{1,2}/\d{4}$", @"^[a-zA-Z0-9_]{0,20}"));
        }

        // Validate Sort Code
        [TestMethod]
        public void ValidateSortCode_Email_SIR_ValidSortCode_ShouldPass()
        {
            List<string> fields = new List<string>() { "david@gmai.com", "SIR 02/07/1995", "99-99-99", "Theft", "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt.\"" };
            Assert.AreEqual("SIR", EmailValidator.ValidateSubject(fields, 1, @"^SIR \d{1,2}/\d{1,2}/\d{4}$", @"^[a-zA-Z0-9_]{0,20}"));
        }

        [TestMethod]
        public void ValidateSortCode_Email_SIR_EmptySortCode_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "david@gmai.com", "SIR 02/07/1995", " ", "Theft", "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt.\"" };
            Assert.ThrowsException<System.Exception>(() => EmailValidator.ValidateSortCode(fields, 2, @"\b[0-9]{2}-?[0-9]{2}-?[0-9]{2}\b"));
        }

        [TestMethod]
        public void ValidateSortCode_Email_SIR_InvalidFormat_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "david@gmai.com", "SIR 02/07/1995", "hello", "Theft", "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt.\"" };
            Assert.ThrowsException<System.Exception>(() => EmailValidator.ValidateSortCode(fields, 2, @"\b[0-9]{2}-?[0-9]{2}-?[0-9]{2}\b"));
        }

        // Validate Incident Type
        [TestMethod]
        public void ValidateIncidentType_Email_SIR_ValidIncident_ShouldPass()
        {
            List<string> fields = new List<string>() { "david@gmai.com", "SIR 02/07/1995", "99-99-99", "Theft", "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt.\"" };
            Assert.AreEqual(fields[3], EmailValidator.ValidateIncidentType(fields, 3));
        }

        [TestMethod]
        public void ValidateIncidentType_Email_SIR_EmptyIncident_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "david@gmai.com", "SIR 02/07/1995", "99-99-99", "", "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt.\"" };
            Assert.ThrowsException<System.Exception>(() => EmailValidator.ValidateIncidentType(fields, 3));
        }

        [TestMethod]
        public void ValidateIncidentType_Email_SIR_InvalidIncident_ShouldThrowError()
        {
            List<string> fields = new List<string>() { "david@gmai.com", "SIR 02/07/1995", "99-99-99", "Hello", "\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt.\"" };
            Assert.ThrowsException<System.Exception>(() => EmailValidator.ValidateIncidentType(fields, 3));
        }
    }
}
