using SocialApis.Formatters;
using SocialApis.Test.TestUtils;
using SocialApis.Utils;
using System.Runtime.Serialization;
using Utf8Json;
using Xunit;

namespace SocialApis.Test.Formatters
{
    /// <summary>
    /// <see cref="StringIntFormatter"/>のテスト
    /// </summary>
    public class StringIntFormatterTest
    {
        public class IntClass
        {
            [DataMember(Name = "value", Order = 1)]
            [JsonFormatter(typeof(StringIntFormatter))]
            public int? Value { get; set; }
        }

        public class NullableIntClass
        {
            [DataMember(Name = "value")]
            [JsonFormatter(typeof(StringIntFormatter))]
            public int? Value { get; set; }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(2147483647)]
        [InlineData(-2147483648)]
        public void TestDeserialize1(int expected)
        {
            // {"value": ${expected}}
            var json = TestJsonUtil.CreateSingleJsonObject(key: "value", value: expected.ToString());
            var actual = JsonUtil.Deserialize<IntClass>(json).Value;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(2147483647)]
        [InlineData(-2147483648)]
        public void TestDeserializeNullableString(int expected)
        {
            var json = TestJsonUtil.CreateSingleJsonObject(key: "value", value: expected.ToString());
            var actual = JsonSerializer.Deserialize<IntClass>(json).Value;

            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(2147483647)]
        [InlineData(-2147483648)]
        public void TestDeserialize2(int? expected)
        {
            // {"value": ${expected}}
            var json = TestJsonUtil.CreateSingleJsonObject(key: "value", value: expected?.ToString());
            var actual = JsonUtil.Deserialize<NullableIntClass>(json).Value;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(2147483647)]
        [InlineData(-2147483648)]
        public void TestDeserializeNullableString2(int? expected)
        {
            var json = TestJsonUtil.CreateSingleJsonObject(key: "value", value: expected?.ToString());
            var actual = JsonSerializer.Deserialize<NullableIntClass>(json).Value;

            Assert.Equal(expected, actual);
        }
    }
}
