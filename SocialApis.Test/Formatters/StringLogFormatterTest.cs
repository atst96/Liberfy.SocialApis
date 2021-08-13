using SocialApis.Formatters;
using SocialApis.Test.TestUtils;
using SocialApis.Utils;
using System.Runtime.Serialization;
using Utf8Json;
using Xunit;

namespace SocialApis.Test.Formatters
{
    /// <summary>
    /// <see cref="StringLongFormatter"/>のテスト
    /// </summary>
    public class StringLogFormatterTest
    {
        public class LongClass
        {
            [DataMember(Name = "value")]
            [JsonFormatter(typeof(StringLongFormatter))]
            public long Value { get; set; }
        }

        public class NullableLongClass
        {
            [DataMember(Name = "value")]
            [JsonFormatter(typeof(StringLongFormatter))]
            public long? Value { get; set; }
        }

        [Theory]
        [InlineData(0L)]
        [InlineData(100L)]
        [InlineData(long.MaxValue)]
        [InlineData(long.MinValue)]
        public void TestDeserialize1(long expected)
        {
            // {"value": ${expected}}
            var json = TestJsonUtil.CreateSingleJsonObject(key: "value", value: expected.ToString());
            var actual = JsonUtil.Deserialize<LongClass>(json).Value;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0L)]
        [InlineData(100L)]
        [InlineData(long.MaxValue)]
        [InlineData(long.MinValue)]
        public void TestDeserializeNullableString(long expected)
        {
            var json = TestJsonUtil.CreateSingleJsonObject(key: "value", value: expected.ToString());
            var actual = JsonSerializer.Deserialize<LongClass>(json).Value;

            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData(null)]
        [InlineData(0L)]
        [InlineData(100L)]
        [InlineData(long.MaxValue)]
        [InlineData(long.MinValue)]
        public void TestDeserialize2(long? expected)
        {
            // {"value": ${expected}}
            var json = TestJsonUtil.CreateSingleJsonObject(key: "value", value: expected?.ToString());
            var actual = JsonUtil.Deserialize<NullableLongClass>(json).Value;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0L)]
        [InlineData(100L)]
        [InlineData(long.MaxValue)]
        [InlineData(long.MinValue)]
        public void TestDeserializeNullableString2(long? expected)
        {
            var json = TestJsonUtil.CreateSingleJsonObject(key: "value", value: expected?.ToString());
            var actual = JsonSerializer.Deserialize<NullableLongClass>(json).Value;

            Assert.Equal(expected, actual);
        }
    }
}
