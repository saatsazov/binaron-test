using System;
using System.Diagnostics;
using System.IO;
using Binaron.Serializer;
using Binaron.Serializer.CustomObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var input = new Employee
            {
                FirstName = "Hello",
                LastName = "Worlds",
                BirthDate = DateTime.UtcNow,
            };

            using (var stream = new MemoryStream())
            {
                BinaronConvert.Serialize(input, stream, new SerializerOptions
                {
                    SkipNullValues = true,
                    CustomObjectIdentifierProviders = { new PersonIdentifierProvider() }
                });
                stream.Position = 0;
                var person = BinaronConvert.Deserialize<Employee>(stream, new DeserializerOptions
                {
                    CustomObjectFactories = { new PersonFactory() }
                });

                stream.Position = 0;
            }
        }


        [TestMethod]
        public void TestMethod2()
        {
            var input = new Employee
            {
                FirstName = "Hello",
                LastName = "Worlds",
                BirthDate = DateTime.UtcNow,
            };

            using (var stream = new MemoryStream())
            {
                BinaronConvert.Serialize(input, stream, new SerializerOptions
                {
                    SkipNullValues = true,
                    CustomObjectIdentifierProviders = { new PersonIdentifierProvider() }
                });
                stream.Position = 0;
                // this should fail?
                var person = BinaronConvert.Deserialize<Customer>(stream, new DeserializerOptions
                {
                    CustomObjectFactories = { new PersonFactory() }
                });

                stream.Position = 0;
            }
        }

        [TestMethod]
        public void TestMethod3()
        {
            var input = new Employee
            {
                FirstName = "Hello",
                LastName = "Worlds",
                BirthDate = DateTime.UtcNow,
            };

            using (var stream = new MemoryStream())
            {
                BinaronConvert.Serialize(input, stream, new SerializerOptions
                {
                    SkipNullValues = true,
                    CustomObjectIdentifierProviders = { new PersonIdentifierProvider() }
                });
                stream.Position = 0;
                // this should  work
                var person = BinaronConvert.Deserialize<IPerson>(stream, new DeserializerOptions
                {
                    CustomObjectFactories = { new PersonFactory() }
                });

                stream.Position = 0;
            }
        }

        public interface IPerson
        {
            string FirstName { get; set; }
            string LastName { get; set; }
            DateTime BirthDate { get; set; }
        }

        public class Employee : IPerson
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime BirthDate { get; set; }

            public string Department { get; set; }
            public string JobTitle { get; set; }
        }

        public class Customer : IPerson
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime BirthDate { get; set; }

            public string Email { get; set; }
        }

        public class PersonIdentifierProvider : CustomObjectIdentifierProvider<IPerson>
        {
            public override object GetIdentifier(Type objectType) => objectType.Name;
        }

        public class PersonFactory : CustomObjectFactory<IPerson>
        {
            public override object Create(object identifier)
            {
                return (identifier as string) switch
                {
                    nameof(Employee) => new Employee(),
                    nameof(Customer) => new Customer(),
                    _ => null
                };
            }
        }
    }
}
