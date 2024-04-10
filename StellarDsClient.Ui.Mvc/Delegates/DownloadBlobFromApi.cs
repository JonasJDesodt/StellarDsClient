namespace StellarDsClient.Ui.Mvc.Delegates
{
    public delegate Task<byte[]> DownloadBlobFromApi(string table, string field, int record);
}
