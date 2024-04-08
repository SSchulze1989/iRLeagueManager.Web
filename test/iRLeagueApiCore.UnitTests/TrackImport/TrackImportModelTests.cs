using iRLeagueApiCore.TrackImport.Models;
using System.Text.Json;

namespace iRLeagueApiCore.UnitTests.TrackImport;

public sealed class TrackImportModelTests
{
    [Fact]
    public void ShouldReadDataFromJson()
    {
        var data = testJson;
        var imported = JsonSerializer.Deserialize<TrackImportModel[]>(data);

        imported.Should().NotBeEmpty();
        var firstTrack = imported![0];
        firstTrack.ai_enabled.Should().BeFalse();
        firstTrack.award_exempt.Should().BeTrue();
        firstTrack.category.Should().Be("road");
        firstTrack.category_id.Should().Be(2);
        firstTrack.closes.Should().Be(DateTime.Parse("2018-10-31"));
        firstTrack.config_name.Should().Be("Full Course");
        firstTrack.corners_per_lap.Should().Be(7);
        firstTrack.created.Should().BeCloseTo(DateTime.Parse("2006-04-04T19:10:00Z"), TimeSpan.FromDays(1));
        firstTrack.free_with_subscription.Should().BeTrue();
        firstTrack.fully_lit.Should().BeFalse();
        firstTrack.grid_stalls.Should().Be(62);
        firstTrack.has_opt_path.Should().BeFalse();
        firstTrack.has_short_parade_lap.Should().BeTrue();
        firstTrack.track_name.Should().Be("[Legacy] Lime Rock Park - 2008");
        firstTrack.track_types.Should().HaveCount(1);
        var trackType = firstTrack.track_types![0];
        trackType.track_type.Should().Be("road");
    }

    public static string testJson = @"[
  {
    ""ai_enabled"": false,
    ""award_exempt"": true,
    ""category"": ""road"",
    ""category_id"": 2,
    ""closes"": ""2018-10-31"",
    ""config_name"": ""Full Course"",
    ""corners_per_lap"": 7,
    ""created"": ""2006-04-04T19:10:00Z"",
    ""free_with_subscription"": true,
    ""fully_lit"": false,
    ""grid_stalls"": 62,
    ""has_opt_path"": false,
    ""has_short_parade_lap"": true,
    ""has_start_zone"": false,
    ""has_svg_map"": true,
    ""is_dirt"": false,
    ""is_oval"": false,
    ""lap_scoring"": 0,
    ""latitude"": 41.9282105,
    ""location"": ""Lakeville, Connecticut, USA"",
    ""longitude"": -73.3839642,
    ""max_cars"": 66,
    ""night_lighting"": false,
    ""nominal_lap_time"": 53.54668,
    ""number_pitstalls"": 34,
    ""opens"": ""2018-04-01"",
    ""package_id"": 9,
    ""pit_road_speed_limit"": 45,
    ""price"": 0,
    ""priority"": 1,
    ""purchasable"": true,
    ""qualify_laps"": 2,
    ""restart_on_left"": false,
    ""retired"": false,
    ""search_filters"": ""road,lrp"",
    ""site_url"": ""http://www.limerock.com/"",
    ""sku"": 10021,
    ""solo_laps"": 8,
    ""start_on_left"": false,
    ""supports_grip_compound"": false,
    ""tech_track"": false,
    ""time_zone"": ""America/New_York"",
    ""track_config_length"": 1.53,
    ""track_dirpath"": ""limerock\\full"",
    ""track_id"": 1,
    ""track_name"": ""[Legacy] Lime Rock Park - 2008"",
    ""track_types"": [
      {
        ""track_type"": ""road""
      }
    ]
  },
  {
    ""ai_enabled"": false,
    ""award_exempt"": false,
    ""category"": ""road"",
    ""category_id"": 2,
    ""closes"": ""2022-10-31"",
    ""config_name"": ""Full Course"",
    ""corners_per_lap"": 20,
    ""created"": ""2006-05-28T04:20:00Z"",
    ""free_with_subscription"": false,
    ""fully_lit"": false,
    ""grid_stalls"": 62,
    ""has_opt_path"": false,
    ""has_short_parade_lap"": false,
    ""has_start_zone"": false,
    ""has_svg_map"": true,
    ""is_dirt"": false,
    ""is_oval"": false,
    ""lap_scoring"": 0,
    ""latitude"": 36.560008,
    ""location"": ""Alton, Virginia, USA"",
    ""longitude"": -79.2048,
    ""max_cars"": 72,
    ""night_lighting"": false,
    ""nominal_lap_time"": 114.92681,
    ""number_pitstalls"": 37,
    ""opens"": ""2022-03-01"",
    ""package_id"": 14,
    ""pit_road_speed_limit"": 45,
    ""price"": 14.95,
    ""priority"": 1,
    ""purchasable"": true,
    ""qualify_laps"": 2,
    ""restart_on_left"": false,
    ""retired"": false,
    ""search_filters"": ""road"",
    ""site_url"": ""http://www.virclub.com/vir"",
    ""sku"": 10031,
    ""solo_laps"": 4,
    ""start_on_left"": false,
    ""supports_grip_compound"": false,
    ""tech_track"": false,
    ""time_zone"": ""America/New_York"",
    ""track_config_length"": 3.27,
    ""track_dirpath"": ""virginia\\full"",
    ""track_id"": 2,
    ""track_name"": ""Virginia International Raceway"",
    ""track_types"": [
      {
        ""track_type"": ""road""
      }
    ]
  }
]";

}
