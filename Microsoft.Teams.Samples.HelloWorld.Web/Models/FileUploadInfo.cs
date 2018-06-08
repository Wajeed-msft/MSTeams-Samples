using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Microsoft.Bot.Connector.Teams.Models
{
    using System.Linq;

    /// <summary>
    /// Upload information for the file.
    /// </summary>
    public partial class FileUploadInfo
    {
        /// <summary>
        /// Initializes a new instance of the FileUploadInfo class.
        /// </summary>
        public FileUploadInfo() { }

        /// <summary>
        /// Initializes a new instance of the FileUploadInfo class.
        /// </summary>
        /// <param name="name">File name.</param>
        /// <param name="uploadUrl">URL to an upload session for the file
        /// contents.</param>
        /// <param name="contentUrl">URL to the file.</param>
        /// <param name="uniqueId">Identifier that uniquely identifies the
        /// file.</param>
        /// <param name="fileType">File type.</param>
        public FileUploadInfo(string name = default(string), string uploadUrl = default(string), string contentUrl = default(string), string uniqueId = default(string), string fileType = default(string))
        {
            Name = name;
            UploadUrl = uploadUrl;
            ContentUrl = contentUrl;
            UniqueId = uniqueId;
            FileType = fileType;
        }

        /// <summary>
        /// Gets or sets file name.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets URL to an upload session for the file contents.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "uploadUrl")]
        public string UploadUrl { get; set; }

        /// <summary>
        /// Gets or sets URL to the file.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "contentUrl")]
        public string ContentUrl { get; set; }

        /// <summary>
        /// Gets or sets identifier that uniquely identifies the file.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "uniqueId")]
        public string UniqueId { get; set; }

        /// <summary>
        /// Gets or sets file type.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "fileType")]
        public string FileType { get; set; }

    }


    /// <summary>
    /// File consent card response invoke activity payload.
    /// </summary>
    public partial class FileConsentCardResponse
    {
        /// <summary>
        /// Initializes a new instance of the FileConsentCardResponse class.
        /// </summary>
        public FileConsentCardResponse() { }

        /// <summary>
        /// Initializes a new instance of the FileConsentCardResponse class.
        /// </summary>
        /// <param name="action">User action on the file consent card.
        /// Possible values include: 'accept', 'decline'</param>
        /// <param name="context">Context sent with the file consent
        /// card.</param>
        /// <param name="uploadInfo">Context sent back to the Bot if user
        /// declined. This is free flow schema and is sent back in Value
        /// field of Activity.</param>
        public FileConsentCardResponse(string action = default(string), object context = default(object), FileUploadInfo uploadInfo = default(FileUploadInfo))
        {
            Action = action;
            Context = context;
            UploadInfo = uploadInfo;
        }

        /// <summary>
        /// Gets or sets user action on the file consent card. Possible values
        /// include: 'accept', 'decline'
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets context sent with the file consent card.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "context")]
        public object Context { get; set; }

        /// <summary>
        /// Gets or sets context sent back to the Bot if user declined. This
        /// is free flow schema and is sent back in Value field of Activity.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "uploadInfo")]
        public FileUploadInfo UploadInfo { get; set; }


    }
}
