// <copyright file="StringExtensionsTest.ReplaceControlCharacters.g.cs" company="TVA">No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.</copyright>
// <auto-generated>
// This file contains automatically generated unit tests.
// Do NOT modify this file manually.
// 
// When Pex is invoked again,
// it might remove or update any previously generated unit tests.
// 
// If the contents of this file becomes outdated, e.g. if it does not
// compile anymore, you may delete this file and invoke Pex again.
// </auto-generated>
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace TVA
{
    public partial class StringExtensionsTest
    {
        #region [ Methods ]

        [TestMethod]
        [PexGeneratedBy(typeof(StringExtensionsTest))]
        public void ReplaceControlCharacters02()
        {
            string s;
            s = this.ReplaceControlCharacters((string)null);
            Assert.AreEqual<string>("", s);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(StringExtensionsTest))]
        public void ReplaceControlCharacters03()
        {
            string s;
            s = this.ReplaceControlCharacters("\0");
            Assert.AreEqual<string>(" ", s);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(StringExtensionsTest))]
        public void ReplaceControlCharacters04()
        {
            string s;
            s = this.ReplaceControlCharacters("\0\0");
            Assert.AreEqual<string>("  ", s);
        }
        
        #endregion
    }
}
