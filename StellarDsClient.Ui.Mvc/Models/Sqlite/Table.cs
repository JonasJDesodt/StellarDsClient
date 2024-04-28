namespace StellarDsClient.Ui.Mvc.Models.Sqlite
{
    public class Table
    {
        public int Id { get; set; }

        public required string Key { get; set; }

        public required int Identity { get; set; }
    }
}