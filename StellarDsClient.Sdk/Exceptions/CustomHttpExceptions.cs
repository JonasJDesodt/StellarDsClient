using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarDsClient.Sdk.Exceptions
{
    public class CustomHttpException(string message, Exception innerException) : Exception(message, innerException) { }

    public class CustomNotFoundException(string message, Exception innerException) : CustomHttpException(message, innerException) { }

    public class CustomUnauthorizedException(string message, Exception innerException) : CustomHttpException(message, innerException) { }

    public class CustomBadRequestException(string message, Exception innerException) : CustomHttpException(message, innerException) { }

}
