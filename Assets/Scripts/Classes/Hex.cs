using System;
using DataManagement;
using Utilities;

namespace Classes
{
    public class Hex
    {
        public string id;
        public string hexName;
        public string hexDescription;
        public string hexDetail;
        public Entity owner;
        public HexQuality hexQuality;
        public Action HexEffect;

        public Hex(string name)
        {
            id = name;
            
            var config = ResourceReader.ReadHexConfig(name);
            hexName = config.hexName;
            hexDescription = config.description;
            hexDetail = config.detail;
            Enum.TryParse(config.quality, true, out HexQuality quality);
            hexQuality = quality;
        }

        public virtual void OnHexGet(Entity entity)
        {
            owner = entity;
        }
        
        public virtual void OnHexRemove()
        {
            owner = null;
        }
    }
}
