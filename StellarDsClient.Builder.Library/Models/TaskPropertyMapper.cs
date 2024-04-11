using StellarDsClient.Builder.Library.Attributes;

namespace StellarDsClient.Builder.Library.Models
{
    internal class TaskPropertyMapper
    {
        [StellarDsType("NVarChar(255)")]
        public required string Title { get; set; }

        [StellarDsType("Boolean")]
        public bool Done { get; set; }

        [StellarDsType("Int")]
        public required int ListId { get; set; }

        [StellarDsType("DateTime")]
        public required DateTime Created { get; set; }

        [StellarDsType("DateTime")]
        public required DateTime Updated { get; set; }
    }
}