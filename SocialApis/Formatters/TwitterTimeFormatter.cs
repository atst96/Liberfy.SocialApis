using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Utf8Json;

namespace SocialApis.Formatters
{
    /// <summary>
    /// <see cref="DateTimeOffset"/>のフォーマッタ(ddd MMM dd HH:mm:ss +ffff yyyy)
    /// </summary>
    public class TwitterDateTimeFormatter : IJsonFormatter<DateTimeOffset>, IJsonFormatter<DateTimeOffset?>
    {
        /// <summary>
        /// 書式
        /// </summary>
        private const string DateTimeFormat = "ddd MMM dd HH:mm:ss +ffff yyyy";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DateTimeOffset Parse(string value)
            => DateTimeOffset.ParseExact(value, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ToString(DateTimeOffset value)
            => value.ToString(DateTimeFormat, CultureInfo.InvariantCulture);

        /// <summary>
        /// 文字列を<see cref="DateTimeOffset"/>のデシリアライズする
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="formatterResolver"></param>
        /// <returns></returns>
        DateTimeOffset IJsonFormatter<DateTimeOffset>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => Parse(reader.ReadString());

        /// <summary>
        /// 文字列を<see cref="DateTimeOffset?"/>のデシリアライズする
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="formatterResolver"></param>
        /// <returns></returns>
        DateTimeOffset? IJsonFormatter<DateTimeOffset?>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => reader.ReadIsNull() ? default(DateTimeOffset?) : Parse(reader.ReadString());

        /// <summary>
        /// <see cref="DateTimeOffset"/>をJSONにシリアライズする
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="formatterResolver"></param>
        void IJsonFormatter<DateTimeOffset>.Serialize(ref JsonWriter writer, DateTimeOffset value, IJsonFormatterResolver formatterResolver)
            => writer.WriteString(ToString(value));

        /// <summary>
        /// <see cref="DateTimeOffset?"/>をJSONにシリアライズする
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="formatterResolver"></param>
        void IJsonFormatter<DateTimeOffset?>.Serialize(ref JsonWriter writer, DateTimeOffset? value, IJsonFormatterResolver formatterResolver)
            => writer.WriteString(value is null ? default : ToString(value.Value));
    }
}
