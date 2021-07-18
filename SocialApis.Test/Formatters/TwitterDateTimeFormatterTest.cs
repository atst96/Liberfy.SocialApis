using SocialApis.Formatters;
using SocialApis.Utils;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Utf8Json;
using Xunit;

namespace SocialApis.Test.Formatters
{
    /// <summary>
    /// <see cref="TwitterDateTimeFormatter"/>のテストクラス
    /// </summary>
    public class TwitterDateTimeFormatterTest
    {
        public class Class1
        {
            [DataMember(Name = "value")]
            [JsonFormatter(typeof(TwitterDateTimeFormatter))]
            public DateTimeOffset Value { get; set; }
        }

        public class Class2
        {
            [DataMember(Name = "value")]
            [JsonFormatter(typeof(TwitterDateTimeFormatter))]
            public DateTimeOffset? Value { get; set; }
        }

        private static string CreateSingleJsonObjectString(string key, string value)
        {
            var jsonValue = value is null ? "null" : $"\"{value}\"";
            return $"{{\"{key}\":{jsonValue}}}";
        }

        [Theory]
        [InlineData("Thu Jul 01 12:34:56 +1234 2021", "2021-07-01T12:34:56.1234+00:00")]
        public void TestDeserialize(string testInput, string expectedDateTime)
        {
            var json = CreateSingleJsonObjectString(key: "value", value: testInput);

            var actual = JsonUtil.Deserialize<Class1>(json).Value;
            var expected = DateTimeOffset.Parse(expectedDateTime);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("2021-07-01T12:34:56.1234+00:00", "Thu Jul 01 12:34:56 +1234 2021")]
        public void TestSerialize(string testInput, string expectedDateTime)
        {
            var obj = new Class1 { Value = DateTimeOffset.Parse(testInput) };

            using var ms = new MemoryStream();
            JsonSerializer.Serialize(ms, obj);
            ms.Position = 0;

            var actual = Encoding.UTF8.GetString(ms.ToArray());
            var expected = CreateSingleJsonObjectString(key: "value", value: expectedDateTime);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("Thu Jul 01 12:34:56 +1234 2021", "2021-07-01T12:34:56.1234+00:00")]
        public void TestDeserializeNullable(string testInput, string expectedDateTime)
        {
            var json = CreateSingleJsonObjectString(key: "value", value: testInput);

            var actual = JsonSerializer.Deserialize<Class2>(json).Value;
            var expected = expectedDateTime is null
                ? (DateTimeOffset?)null
                : DateTimeOffset.Parse(expectedDateTime);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("2021-07-01T12:34:56.1234+00:00", "Thu Jul 01 12:34:56 +1234 2021")]
        public void TestSerializeNullable(string testInput, string expectedDateTime)
        {
            var obj = new Class2
            {
                Value = testInput is null ? null : DateTimeOffset.Parse(testInput)
            };

            using var ms = new MemoryStream();
            JsonSerializer.Serialize(ms, obj);
            ms.Position = 0;

            var actual = Encoding.UTF8.GetString(ms.ToArray());
            var expected = CreateSingleJsonObjectString(key: "value", value: expectedDateTime);

            Assert.Equal(expected, actual);
        }
    }
}
