using Xunit;
using CrispyWaffle.Serialization.Adapters;

namespace CrispyWaffle.Tests.Serialization
{
    public class TomlSerializerAdapterTests
    {
        private readonly TomlSerializerAdapter _serializer = new TomlSerializerAdapter();

        private class TestObject
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [Fact]
        public void Serialize_ShouldReturnTomlString()
        {
            var obj = new TestObject { Name = "John Doe", Age = 30 };
            var toml = _serializer.Serialize(obj);

            Assert.Contains("Name = \"John Doe\"", toml);
            Assert.Contains("Age = 30", toml);
        }

        [Fact]
        public void Deserialize_ShouldReturnObject()
        {
            var toml = "Name = \"John Doe\"\nAge = 30";
            var obj = _serializer.Deserialize<TestObject>(toml);

            Assert.Equal("John Doe", obj.Name);
            Assert.Equal(30, obj.Age);
        }

        [Fact]
        public void Serialize_And_Deserialize_ShouldReturnEquivalentObject()
        {
            var originalObj = new TestObject { Name = "Jane Doe", Age = 25 };
            var toml = _serializer.Serialize(originalObj);
            var deserializedObj = _serializer.Deserialize<TestObject>(toml);

            Assert.Equal(originalObj.Name, deserializedObj.Name);
            Assert.Equal(originalObj.Age, deserializedObj.Age);
        }
    }
}
