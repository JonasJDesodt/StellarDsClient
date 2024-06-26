﻿using System.Net;

namespace StellarDsClient.Sdk.Dto.Transfer
{
    public class StellarDsErrorMessage
    {
        public required string Code { get; set; }

        public required string Message { get; set; }

        public required int Type { get; set; }
    }
}
