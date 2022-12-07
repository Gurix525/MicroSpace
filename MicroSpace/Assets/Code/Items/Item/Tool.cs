namespace Items
{
    public class Tool : Item
    {
        public ToolType ToolType { get; private set; }

        public Tool(int itemModel, ToolType toolType) : base(itemModel)
        {
            ToolType = toolType;
        }

        public Tool(int itemModel, int id, ToolType toolType) : base(itemModel, id)
        {
            ToolType = toolType;
        }
    }
}