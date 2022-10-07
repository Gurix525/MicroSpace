using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Data
{
    [Serializable]
    public class Save
    {
        public List<DBObject> DBObjects;
        public DBObject FocusedShip;

        public Save(List<DBObject> dbObjects, DBObject focusedShip)
        {
            DBObjects = dbObjects;
            FocusedShip = focusedShip;
        }
    }
}