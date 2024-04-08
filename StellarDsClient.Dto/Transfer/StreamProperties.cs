namespace StellarDsClient.Dto.Transfer
{
    public class StreamProperties
    {
        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }

        public bool CanSeek { get; set; }

        public bool CanTimeout { get; set; }

        public long Length { get; set; }

        public long Position { get; set; }

        public int ReadTimeout { get; set; }

        public int WriteTimeout { get; set; }
    }
}