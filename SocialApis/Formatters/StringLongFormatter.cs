using System;
using Utf8Json;

namespace SocialApis.Formatters
{
    /// <summary>
    /// 文字列対応<see cref="long"/>、<see cref="long?"/>のフォーマッタ
    /// </summary>
    public class StringLongFormatter : IJsonFormatter<long>, IJsonFormatter<long?>
    {
        /// <summary>
        /// 数値または文字列を<see cref="long"/>にデシリアライズする
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="formatterResolver"></param>
        /// <returns></returns>
        long IJsonFormatter<long>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => reader.GetCurrentJsonToken() switch
            {
                JsonToken.Number => reader.ReadInt64(),
                JsonToken.String => long.Parse(reader.ReadString()),
                JsonToken.Null => throw new NullReferenceException(),
                _ => throw new NotSupportedException(),
            };

        /// <summary>
        /// 数値または文字列を<see cref="long?"/>にデシリアライズする
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="formatterResolver"></param>
        /// <returns></returns>
        long? IJsonFormatter<long?>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => reader.ReadIsNull() ? default(long?) : reader.GetCurrentJsonToken() switch
            {
                JsonToken.Number => reader.ReadInt64(),
                JsonToken.String => long.Parse(reader.ReadString()),
                _ => throw new NotSupportedException(),
            };

        /// <summary>
        /// <see cref="long"/>をJSONにシリアライズする
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="formatterResolver"></param>
        void IJsonFormatter<long>.Serialize(ref JsonWriter writer, long value, IJsonFormatterResolver formatterResolver)
            => writer.WriteString(value.ToString());

        /// <summary>
        /// <see cref="long?"/>をJSONにシリアライズする
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="formatterResolver"></param>
        void IJsonFormatter<long?>.Serialize(ref JsonWriter writer, long? value, IJsonFormatterResolver formatterResolver)
            => writer.WriteString(value?.ToString());
    }
}
