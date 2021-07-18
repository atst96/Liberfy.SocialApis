using SocialApis.Formatters;
using System;
using System.Runtime.Serialization;
using Utf8Json;

namespace SocialApis.Twitter
{
    [DataContract]
    public class User
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "screen_name")]
        public string ScreenName { get; set; }

        [DataMember(Name = "location")]
        public string? Location { get; set; }

        // TODO: [DataMember(Name = "derived")]

        [DataMember(Name = "url")]
        public string? Url { get; set; }

        [DataMember(Name = "description")]
        public string? Description { get; set; }

        [DataMember(Name = "protected")]
        public bool IsProtected { get; set; }

        [DataMember(Name = "verified")]
        public bool IsVerified { get; set; }

        [DataMember(Name = "followers_count")]
        public int FollowersCount { get; set; }

        [DataMember(Name = "friends_count")]
        public int FriendsCount { get; set; }

        [DataMember(Name = "listed_count")]
        public int ListedCount { get; set; }

        [DataMember(Name = "favourites_count")]
        public int FavoritesCount { get; set; }

        [DataMember(Name = "statuses_count")]
        public int StatusesCount { get; set; }

        [DataMember(Name = "created_at")]
        [JsonFormatter(typeof(TwitterDateTimeFormatter))]
        public DateTimeOffset? CreatedAt { get; set; }

        [DataMember(Name = "profile_banner_url")]
        public string ProfileBannerUrl { get; set; }

        [DataMember(Name = "profile_image_url_https")]
        public string ProfileImageUrlHttps { get; set; }

        [DataMember(Name = "default_profile")]
        public bool IsDefaultProfile { get; set; }

        [DataMember(Name = "default_profile_image")]
        public bool IsDefaultProfileImage { get; set; }

        [DataMember(Name = "withheld_in_countries")]
        public string[] WithheldInCountries { get; set; }

        [DataMember(Name = "withheld_scope")]
        public string WithheldScope { get; set; }

        [DataMember(Name = "entities")]
        public UserEntities Entities { get; set; }

        [DataMember(Name = "suspended")]
        public bool? IsSuspended { get; set; }
    }
}
