using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using SocialApis.Extensions;
using Utf8Json;

namespace SocialApis.Formatters
{
    /// <summary>
    /// <see cref="DateTimeOffset"/>のフォーマッタ
    /// </summary>
    public class TwitterDateTimeFormatter : IJsonFormatter<DateTimeOffset>, IJsonFormatter<DateTimeOffset?>
    {
        /// <summary>
        /// 日時書式
        /// </summary>
        private const string DateTimeFormat = "ddd MMM dd HH:mm:ss +ffff yyyy";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DateTimeOffset Parse(string value)
            => DateTimeOffset.ParseExact(value, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

        private static string ToString(DateTimeOffset value)
            => value.ToString(DateTimeFormat, CultureInfo.InvariantCulture);

        void IJsonFormatter<DateTimeOffset>.Serialize(ref JsonWriter writer, DateTimeOffset value, IJsonFormatterResolver formatterResolver)
        {
            formatterResolver.WriteValue(ref writer, ToString(value));
        }

        DateTimeOffset IJsonFormatter<DateTimeOffset>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            return Parse(formatterResolver.ReadValue<string>(ref reader));
        }

        void IJsonFormatter<DateTimeOffset?>.Serialize(ref JsonWriter writer, DateTimeOffset? value, IJsonFormatterResolver formatterResolver)
        {
            var dateTime = value is null ? null : ToString(value.Value);

            formatterResolver.WriteValue(ref writer, dateTime);
        }

        DateTimeOffset? IJsonFormatter<DateTimeOffset?>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var value = formatterResolver.ReadValue<string>(ref reader);

            return value is null ? (DateTimeOffset?)null : Parse(value);
        }
    }
}
