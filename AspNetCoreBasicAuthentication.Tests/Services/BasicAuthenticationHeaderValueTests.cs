using AspNetCoreBasicAuthentication.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AspNetCoreBasicAuthentication.Tests.Services
{
    public class BasicAuthenticationHeaderValueTests
    {
        [Fact]
        public void ReturnsInvalidResultForInvalidHeaderValue()
        {
            var headerValue = "Basic NotBase64";
            var actual = new BasicAuthenticationHeaderValue(headerValue);

            Assert.False(actual.IsValidBasicAuthenticationHeaderValue);
        }

        [Fact]
        public void ReturnsCorrectResult()
        {
            // George:P4$$w0rD
            var headerValue = "Basic R2VvcmdlOlA0JCR3MHJE";
            var actual = new BasicAuthenticationHeaderValue(headerValue);

            Assert.True(actual.IsValidBasicAuthenticationHeaderValue);
            Assert.Equal("George", actual.UserIdentifier);
            Assert.Equal("P4$$w0rD", actual.UserPassword);
        }

        [Fact]
        public void ReturnsCorrectResult_WithColonInPassword()
        {
            //George:pass:word
            var headerValue = "Basic R2VvcmdlOnBhc3M6d29yZA==";
            var actual = new BasicAuthenticationHeaderValue(headerValue);

            Assert.True(actual.IsValidBasicAuthenticationHeaderValue);
            Assert.Equal("George", actual.UserIdentifier);
            Assert.Equal("pass:word", actual.UserPassword);
        }
    }
}
