using SocialApis.Twitter;
using SocialApis.Utils;
using System;
using System.Text.Json;
using Xunit;

namespace SocialApis.Test.Twitter.Objects
{
    /// <summary>
    /// <see cref="User"/>のパーステスト
    /// </summary>
    public class UserTest
    {
        [Fact]
        public void ParseTest()
        {
            var user = JsonUtil.Deserialize<User>(Resource.Twitter_Object_User);

            Assert.Equal(6253282, user.Id);
            Assert.Equal("Twitter API", user.Name);
            Assert.Equal("TwitterAPI", user.ScreenName);
            Assert.Equal("San Francisco, CA", user.Location);
            Assert.Equal("https://example.com/url", user.Url);
            Assert.Equal("description", user.Description);
            Assert.True(user.IsProtected);
            Assert.True(user.IsVerified);
            Assert.Equal(10, user.FollowersCount);
            Assert.Equal(20, user.FriendsCount);
            Assert.Equal(30, user.ListedCount);
            Assert.Equal(40, user.FavoritesCount);
            Assert.Equal(50, user.StatusesCount);
            Assert.Equal(DateTimeOffset.Parse("2007/05/23 06:01:13+00:00"), user.CreatedAt);
            Assert.Equal("https://example.com/profile_banner_url", user.ProfileBannerUrl);
            Assert.Equal("https://example.com/profile_image_url_https", user.ProfileImageUrlHttps);
            Assert.False(user.IsDefaultProfile);
            Assert.False(user.IsDefaultProfileImage);
            Assert.Equal(new string[] { "A", "B" }, user.WithheldInCountries);
            Assert.Equal("user", user.WithheldScope);
            Assert.NotNull(user.Entities);
            Assert.False(user.IsSuspended);
        }

        [Fact]
        public void ParseTest2()
        {
            var user = JsonUtil.Deserialize<User>(Resource.Twitter_Object_User_Nullable);

            Assert.Equal(6253282, user.Id);
            Assert.Equal("Twitter API", user.Name);
            Assert.Equal("TwitterAPI", user.ScreenName);
            Assert.Null(user.Location);
            Assert.Null(user.Url);
            Assert.Null(user.Description);
            Assert.False(user.IsProtected);
            Assert.False(user.IsVerified);
            Assert.Equal(0, user.FollowersCount);
            Assert.Equal(0, user.FriendsCount);
            Assert.Equal(0, user.ListedCount);
            Assert.Equal(0, user.FavoritesCount);
            Assert.Equal(0, user.StatusesCount);
            Assert.Equal(DateTimeOffset.Parse("2007/05/23 06:01:13+00:00"), user.CreatedAt);
            Assert.Null(user.ProfileBannerUrl);
            Assert.Null(user.ProfileImageUrlHttps);
            Assert.True(user.IsDefaultProfile);
            Assert.True(user.IsDefaultProfileImage);
            Assert.Null(user.WithheldInCountries);
            Assert.Null(user.WithheldScope);
            Assert.Null(user.Entities);
            Assert.Null(user.IsSuspended);
        }
    }
}
