namespace Inventory
{
    public class ContainerItem
    {
        public int ModelId { get; private set; }
        public float Mass { get; set; }

        public ContainerItem(int modelId, float mass)
        {
            ModelId = modelId;
            Mass = mass;
        }
    }
}