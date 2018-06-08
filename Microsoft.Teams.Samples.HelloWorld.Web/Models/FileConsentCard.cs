﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Bot.Connector.Teams.Models
{
    using System.Linq;

    /// <summary>
    /// File consent card attachment.
    /// </summary>
    public partial class FileConsentCard
    {
        /// <summary>
        /// Initializes a new instance of the FileConsentCard class.
        /// </summary>
        public FileConsentCard() { }

        /// <summary>
        /// Initializes a new instance of the FileConsentCard class.
        /// </summary>
        /// <param name="description">File description.</param>
        /// <param name="sizeInBytes">Size of the file to be uploaded in
        /// Bytes.</param>
        /// <param name="acceptContext">Context sent back to the Bot if user
        /// consented to upload. This is free flow schema and is sent back in
        /// Value field of Activity.</param>
        /// <param name="declineContext">Context sent back to the Bot if user
        /// declined. This is free flow schema and is sent back in Value
        /// field of Activity.</param>
        public FileConsentCard(string description = default(string), long? sizeInBytes = default(long?), object acceptContext = default(object), object declineContext = default(object))
        {
            Description = description;
            SizeInBytes = sizeInBytes;
            AcceptContext = acceptContext;
            DeclineContext = declineContext;
        }

        /// <summary>
        /// Gets or sets file description.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets size of the file to be uploaded in Bytes.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "sizeInBytes")]
        public long? SizeInBytes { get; set; }

        /// <summary>
        /// Gets or sets context sent back to the Bot if user consented to
        /// upload. This is free flow schema and is sent back in Value field
        /// of Activity.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "acceptContext")]
        public object AcceptContext { get; set; }

        /// <summary>
        /// Gets or sets context sent back to the Bot if user declined. This
        /// is free flow schema and is sent back in Value field of Activity.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "declineContext")]
        public object DeclineContext { get; set; }

    }
}