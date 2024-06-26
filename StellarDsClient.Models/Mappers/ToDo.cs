﻿using System;
using StellarDsClient.Sdk.Attributes;

namespace StellarDsClient.Models.Mappers
{
    [StellarDsTable(true, "StellarDsClient table")]
    public class ToDo
    {
        [StellarDsProperty("NVarChar(255)")]
        public required string Title { get; set; }

        [StellarDsProperty("Boolean")]
        public bool Done { get; set; }

        [StellarDsProperty("Int")]
        public required int ListId { get; set; }

        [StellarDsProperty("DateTime")]
        public required DateTime Created { get; set; }

        [StellarDsProperty("DateTime")]
        public required DateTime Updated { get; set; }
    }
}