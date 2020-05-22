﻿using JetBrains.Annotations;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.YouTube;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.YouTube
{
    
    public class YouTubeRelatedTrackSource : ISource
    {
        public string TrackId { get; set; }

        public override Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>
            {
                { "i", TrackId }
            };
        }

        public override void ApplyParameters(Dictionary<string, object> data)
        {
            data.TryGetValue("i", out var trackId);
            TrackId = trackId?.ToString();
        }

        public override async Task<SourceResponse> GetItemsAsync(int count, string token, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Call the YouTube API and get the items
            var tracks = await SoundByteService.Current.GetAsync<YouTubeVideoHolder>(ServiceTypes.YouTube, "search",
                new Dictionary<string, string>
                {
                    {"part", "id"},
                    {"maxResults", count.ToString()},
                    {"pageToken", token},
                    {"type", "video"},
                    {"topicId", "/m/04rlf"},
                    {"relatedToVideoId", TrackId}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!tracks.Response.Tracks.Any())
            {
                return new SourceResponse(null, null, false, "No related tracks", "This item has no related tracks.");
            }

            // We now need to get the content details (ugh)
            var youTubeIdList = string.Join(",", tracks.Response.Tracks.Select(m => m.Id.VideoId));

            var extendedTracks = await SoundByteService.Current.GetAsync<YouTubeVideoHolder>(ServiceTypes.YouTube, "videos",
                new Dictionary<string, string>
                {
                    {"part", "snippet,contentDetails"},
                    { "id", youTubeIdList }
                }, cancellationToken).ConfigureAwait(false);

            // Convert YouTube specific tracks to base tracks
            var baseTracks = new List<BaseSoundByteItem>();
            foreach (var track in extendedTracks.Response.Tracks)
            {
                if (track.Id.Kind == "youtube#video")
                {
                    baseTracks.Add(new BaseSoundByteItem(track.ToBaseTrack()));
                }
            }

            // Return the items
            return new SourceResponse(baseTracks, tracks.Response.NextList);
        }
    }
}