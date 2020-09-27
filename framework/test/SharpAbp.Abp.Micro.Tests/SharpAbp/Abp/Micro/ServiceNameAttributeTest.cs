using SharpAbp.Abp.Micro.TestObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SharpAbp.Abp.Micro
{
    public class ServiceNameAttributeTest
    {
        [Fact]
        public void Should_Get_Specified_Name()
        {
            var name = ServiceNameAttribute
                  .GetServiceName<TestService2>();

            Assert.Equal("service2", name);
        }

        [Fact]
        public void Should_Get_Full_Class_Name_If_Not_Specified()
        {
            var expected = typeof(TestService1).FullName;

            var name = ServiceNameAttribute
                  .GetServiceName<TestService1>();
            Assert.Equal(expected, name);
        }


        [Fact]
        public void GetName_By_Type()
        {
            var name = ServiceNameAttribute.GetServiceName(typeof(TestService2));
            Assert.Equal("service2", name);
        }

    }
}
