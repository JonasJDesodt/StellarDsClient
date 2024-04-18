using StellarDsClient.Builder.Library.Attributes;
using System;

namespace StellarDsClient.Models.Mappers
{
    [StellarDsTable(true, "StellarDsClient table")]
    public class List
    {
        [StellarDsProperty("NVarChar(100)")]
        public required string Title { get; set; }

        [StellarDsProperty("DateTime")]
        public required DateTime Created { get; set; }

        [StellarDsProperty("DateTime")]
        public required DateTime Updated { get; set; }

        [StellarDsProperty("DateTime")]
        public DateTime? Deadline { get; set; }

        [StellarDsProperty("Blob")]
        public string? Image { get; set; }

        [StellarDsProperty("NVarChar(255)")]
        public required string OwnerId { get; set; }

        [StellarDsProperty("NVarChar(255)")]
        public required string OwnerName { get; set; }
    }
}