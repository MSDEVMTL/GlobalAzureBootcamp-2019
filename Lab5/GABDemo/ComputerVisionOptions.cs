using System.ComponentModel.DataAnnotations;

namespace GABDemo
{
    public class ComputerVisionOptions
    {
        /// <summary>
        /// Your subscription key
        /// </summary>
        [Required]
        public string ApiKey { get; set; }

        /// <summary>
        /// The endpoint of the region in which your created your ComputerVision resource. i.e. https://westcentralus.api.cognitive.microsoft.com
        /// </summary>
        [Required]
        public string ApiEndPoint { get; set; }
    }
}