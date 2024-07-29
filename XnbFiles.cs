namespace Xnb.Exporter
{
    public class XnbItem
    {
        /**
        * @brief Gets or sets the name of the XNB item.
        *
        * The name is typically used to identify the item within the XNB file or its associated context.
        */
        public string Name { get; set; }

        /**
        * @brief Gets or sets the path to the XNB item.
        *
        * The path represents the location of the item within the file system or the XNB file structure.
        */
        public string Path { get; set; }

    }
}
