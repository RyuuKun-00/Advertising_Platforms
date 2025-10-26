

using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Core.Entities;

namespace AdvertisingPlatforms.DataAccess.Storage
{
    public class StorageSingleton : IStorage
    {
        public static StorageSingleton GetInstance { get; } = new StorageSingleton();

        public Dictionary<string, AdvertisingPlatformEntity> StorageAP { get; set; } = new();
    }
}
