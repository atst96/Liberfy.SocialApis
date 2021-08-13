namespace SocialApis.Test.TestUtils
{
    internal static class TestJsonUtil
    {
        /// <summary>
        /// JSON文字列を生成する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CreateSingleJsonObject(string key, string value)
        {
            var jsonValue = value is null ? "null" : value;
            return $"{{\"{key}\":{jsonValue}}}";
        }

        /// <summary>
        /// JSON文字列を生成する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CreateSingleJsonString(string key, string value)
        {
            var jsonValue = value is null ? "null" : $"\"{value}\"";
            return $"{{\"{key}\":{jsonValue}}}";
        }
    }
}
