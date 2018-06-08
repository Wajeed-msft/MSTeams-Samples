﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Bot.Connector.Teams.Models
{
    using System.Linq;

    /// <summary>
    /// File download info attachment.
    /// </summary>
    public partial class FileDownloadInfo
    {
        /// <summary>
        /// Initializes a new instance of the FileDownloadInfo class.
        /// </summary>
        public FileDownloadInfo() { }

        /// <summary>
        /// Initializes a new instance of the FileDownloadInfo class.
        /// </summary>
        /// <param name="downloadUrl">File download url.</param>
        /// <param name="uniqueId">Unique Id for the file.</param>
        /// <param name="fileType">Type of file.</param>
        public FileDownloadInfo(string downloadUrl = default(string), string uniqueId = default(string), string fileType = default(string))
        {
            DownloadUrl = downloadUrl;
            UniqueId = uniqueId;
            FileType = fileType;
        }

        /// <summary>
        /// Gets or sets file download url.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "downloadUrl")]
        public string DownloadUrl { get; set; }

        /// <summary>
        /// Gets or sets unique Id for the file.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "uniqueId")]
        public string UniqueId { get; set; }

        /// <summary>
        /// Gets or sets type of file.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "fileType")]
        public string FileType { get; set; }

    }
}