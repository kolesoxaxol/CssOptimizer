namespace CssOptimizerU.Models
{
    public class MediaCss
    {
        /// <summary>
        /// (min-width: 768px), (max-width: 767px)
        /// </summary>
        public string MediaSelectorName { get; set; }  

        /// <summary>
        /// text which be insight mediaCss
        /// </summary>
        public string Value { get; set; }
    }
}
