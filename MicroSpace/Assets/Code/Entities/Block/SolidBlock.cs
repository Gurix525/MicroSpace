using ExtensionMethods;

namespace Entities
{
    public abstract class SolidBlock : Block
    {
        protected virtual void OnDestroy()
        {
            this.GetComponentUpInHierarchy<Satellite>().UpdateSatellite();
        }
    }
}