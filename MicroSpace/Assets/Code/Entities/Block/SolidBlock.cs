using ExtensionMethods;
using UnityEngine;

namespace Entities
{
    public abstract class SolidBlock : Block
    {
        public void ExecuteMiningTask()
        {
            GetComponent<Collider2D>().enabled = false;
            Satellite satellite = this.GetComponentUpInHierarchy<Satellite>();
            transform.parent = null;
            satellite.UpdateSatellite();
            Destroy(gameObject);
        }
    }
}