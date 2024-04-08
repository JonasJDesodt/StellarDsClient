namespace StellarDsClient.Ui.Mvc.Delegates
{
    public delegate Task<byte[]> DownloadBlobFromApi(int tableId, string field, int record);
}
