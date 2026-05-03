using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Game.Infrastructure.Assets
{
    public class AddressableLoader
    {
        private readonly Dictionary<string, object> loadedAssetsCache = new Dictionary<string, object>();
        public async Task<TAsset> LoadAssetByKeyAsync<TAsset>(string addressableKey, CancellationToken cancellationToken) where TAsset : class
        {
            // 检查缓存中是否已加载该资源
            if (loadedAssetsCache.TryGetValue(addressableKey, out object cachedAsset))
            {
                // 添加类型安全检查，避免运行时类型转换异常
                if (cachedAsset is TAsset typedAsset)
                {
                    return typedAsset;
                }
                // 类型不匹配时抛出明确异常
                throw new System.InvalidCastException($"缓存资源类型不匹配。键：{addressableKey}，期望类型：{typeof(TAsset).Name}，实际类型：{cachedAsset.GetType().Name}");
            }
            try
            {
                // 从Addressable加载资源
                TAsset asset = await Addressables.LoadAssetAsync<TAsset>(addressableKey).Task;
                // 缓存该资源
                loadedAssetsCache[addressableKey] = asset;
                return asset;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void Release(string addressableKey)
        {
            // 从缓存中获取实际加载的资源对象
            if (loadedAssetsCache.TryGetValue(addressableKey, out object asset))
            {
                Addressables.Release(asset);
                loadedAssetsCache.Remove(addressableKey);
            }
        }
    }
}
