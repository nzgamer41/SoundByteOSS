﻿using System.Collections.Generic;

namespace SoundByte.Core.Models.Sources
{
    /// <summary>
    ///     Response generated by sources / music providers
    /// </summary>
    public class SourceResponse
    {
        /// <summary>
        ///     List of items to display
        /// </summary>
        public IEnumerable<Media.Media> Items { get; }

        /// <summary>
        ///     Token for the next request
        /// </summary>
        public string NextToken { get; }

        /// <summary>
        ///     If this request was successful
        /// </summary>
        public bool Successful { get; }

        /// <summary>
        ///     If the request was not successful, why? (title)
        /// </summary>
        public string MessageTitle { get; }

        /// <summary>
        ///     If the request was not successful, why? (description)
        /// </summary>
        public string MessageContent { get; }

        public SourceResponse(string errorTitle, string errorContent)
        {
            Items = null;
            NextToken = null;
            Successful = false;
            MessageTitle = errorTitle;
            MessageContent = errorContent;
        }

        public SourceResponse(Media.Media[] items, string nextToken)
        {
            Items = items;
            NextToken = nextToken;
            Successful = true;
            MessageTitle = null;
            MessageContent = null;
        }
    }
}