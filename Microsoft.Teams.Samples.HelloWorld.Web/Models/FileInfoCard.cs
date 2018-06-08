using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Bot.Connector.Teams.Models
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// File info card.
    /// </summary>
    public partial class FileInfoCard
    {
        /// <summary>
        /// Initializes a new instance of the FileInfoCard class.
        /// </summary>
        public FileInfoCard() { }

        /// <summary>
        /// Initializes a new instance of the FileInfoCard class.
        /// </summary>
        /// <param name="uniqueId">Unique Id for the file.</param>
        /// <param name="fileType">Type of file.</param>
        public FileInfoCard(string uniqueId = default(string), string fileType = default(string))
        {
            UniqueId = uniqueId;
            FileType = fileType;
        }

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

    public static partial class CardExtensions
    {
        /// <summary>
        /// Creates a new attachment from <see cref="FileInfoCard"/>.
        /// </summary>
        /// <param name="card"> The instance of <see cref="FileInfoCard"/>.</param>
        /// <returns> The generated attachment.</returns>
        public static Attachment ToAttachment(this FileInfoCard card)
        {
            return new Attachment
            {
                Content = card,
                ContentType = FileInfoCard.ContentType
            };
        }

        /// <summary>
        /// Creates a new attachment from <see cref="FileConsentCard"/>.
        /// </summary>
        /// <param name="card"> The instance of <see cref="FileConsentCard"/>.</param>
        /// <returns> The generated attachment.</returns>
        public static Attachment ToAttachment(this FileConsentCard card)
        {
            return new Attachment
            {
                Content = card,
                ContentType = FileConsentCard.ContentType
            };
        }
    }

    /// <summary>
    /// Content type for <see cref="FileConsentCard"/>
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Using one file for all additional properties.")]
    public partial class FileConsentCard
    {
        /// <summary>
        /// Content type to be used in the type property.
        /// </summary>
        public const string ContentType = "application/vnd.microsoft.teams.card.file.consent";
    }

    /// <summary>
    /// Content type for <see cref="FileDownloadInfo"/>
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Using one file for all additional properties.")]
    public partial class FileDownloadInfo
    {
        /// <summary>
        /// Content type to be used in the type property.
        /// </summary>
        public const string ContentType = "application/vnd.microsoft.teams.file.download.info";
    }

    /// <summary>
    /// Content type for <see cref="FileConsentCard"/>
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Using one file for all additional properties.")]
    public partial class FileInfoCard
    {
        /// <summary>
        /// Content type to be used in the type property.
        /// </summary>
        public const string ContentType = "application/vnd.microsoft.teams.card.file.info";
    }
}