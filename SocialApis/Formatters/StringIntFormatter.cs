using System;
using Utf8Json;

namespace SocialApis.Formatters
{
    /// <summary>
    /// 文字列対応<see cref="int"/>、<see cref="int?"/>のフォーマッタ
    /// </summary>
    public class StringIntFormatter : IJsonFormatter<int>, IJsonFormatter<int?>
    {
        /// <summary>
        /// 数値または文字列を<see cref="int"/>にデシリアライズする
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="formatterResolver"></param>
        /// <returns></returns>
        int IJsonFormatter<int>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => reader.GetCurrentJsonToken() switch
            {
                JsonToken.Number => reader.ReadInt32(),
                JsonToken.String => int.Parse(reader.ReadString()),
                JsonToken.Null => throw new NullReferenceException(),
                _ => throw new NotSupportedException(),
            };

        /// <summary>
        /// 数値または文字列を<see cref="int?"/>にデシリアライズする
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="formatterResolver"></param>
        /// <returns></returns>
        int? IJsonFormatter<int?>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => reader.ReadIsNull() ? default(int?) : reader.GetCurrentJsonToken() switch
            {
                JsonToken.Number => reader.ReadInt32(),
                JsonToken.String => int.Parse(reader.ReadString()),
                _ => throw new NotSupportedException(),
            };

        /// <summary>
        /// <see cref="int"/>をJSONにシリアライズする
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="formatterResolver"></param>
        void IJsonFormatter<int>.Serialize(ref JsonWriter writer, int value, IJsonFormatterResolver formatterResolver)
            => writer.WriteString(value.ToString());

        /// <summary>
        /// <see cref="int?"/>をJSONにシリアライズする
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="formatterResolver"></param>
        void IJsonFormatter<int?>.Serialize(ref JsonWriter writer, int? value, IJsonFormatterResolver formatterResolver)
            => writer.WriteString(value?.ToString());
    }
}
