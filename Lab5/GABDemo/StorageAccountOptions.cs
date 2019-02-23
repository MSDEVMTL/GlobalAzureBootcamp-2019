using System.ComponentModel.DataAnnotations;

namespace GABDemo
{
    public class StorageAccountOptions
    {
        [Required]
        public string ConnectionString { get; set; }
    }
}