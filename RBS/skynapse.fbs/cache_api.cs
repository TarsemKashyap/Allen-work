// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.cache_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using log4net;
using System;
using System.Runtime.Caching;

namespace skynapse.fbs
{
  public class cache_api
  {
    private ObjectCache cache;
    private CacheItemPolicy policy;
    private double expiry_config;
    public static ILog log = LogManager.GetLogger("fbs_log");

    public cache_api(double config)
    {
      this.expiry_config = config;
      this.cache = (ObjectCache) MemoryCache.Default;
      this.policy = new CacheItemPolicy();
      this.policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(this.expiry_config);
    }

    public void set_cache(string parameter, object obj)
    {
      try
      {
        this.cache.Add(parameter, obj, this.policy);
      }
      catch (Exception ex)
      {
        cache_api.log.Error((object) "Cache Add Error -> ", ex);
      }
    }

    public bool has_cache(string item) => this.cache.Contains(item);

    public void remove_cache(string parameter)
    {
      try
      {
        this.cache.Remove(parameter);
      }
      catch (Exception ex)
      {
        cache_api.log.Error((object) "Cache Remove Error -> ", ex);
      }
    }

    public object get_cache(string parameter)
    {
      try
      {
        return this.cache[parameter];
      }
      catch (Exception ex)
      {
        cache_api.log.Error((object) "Cache get Error -> ", ex);
      }
      return (object) null;
    }
  }
}
