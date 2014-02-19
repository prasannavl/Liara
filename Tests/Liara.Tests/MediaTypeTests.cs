using System;
using Liara.Common;
using  Xunit;
using Liara;

namespace Liara.Tests
{
    
    public class MediaTypeTests
    {
        [Fact]
        public void CheckMediaTypeCompatibility()
        {
            var m1 = new MediaType("application/xml");
            var m2 = new MediaType("application/*");
            var m3 = new MediaType("application/json");
            var m4 = new MediaType("text/*");
            

            Assert.True(m1.IsCompatible(m2));
            Assert.True(m2.IsCompatible(m3));
            Assert.False(m2.IsCompatible(m4));
        }
    }
}
