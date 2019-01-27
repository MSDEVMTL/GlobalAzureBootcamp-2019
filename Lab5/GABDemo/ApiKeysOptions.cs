namespace GABDemo
{
    public class ApiKeysOptions
    {
        public ComputerVisionOptions ComputerVision { get; set; }
        public StorageAccountOptions Storage { get; set; }
    }

    public class StorageAccountOptions
    {
        public string ConnectionString { get; set; }
    }

    public class ComputerVisionOptions
    {
        /// <summary>
        /// Your subscription key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The endpoint of the region in which your created your ComputerVision resource. i.e. https://westcentralus.api.cognitive.microsoft.com
        /// </summary>
        public string ApiEndPoint { get; set; }
    }
}
