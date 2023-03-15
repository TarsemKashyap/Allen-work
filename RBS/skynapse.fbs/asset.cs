// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.asset
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System.Collections.Generic;
using System.Xml;

namespace skynapse.fbs
{
  public class asset : core_class
  {
    public long asset_id;
    public string name;
    public string code;
    public string description;
    public short capacity;
    public bool available_for_booking;
    public bool is_restricted;
    public XmlDocument properties;
    public int building_id;
    public int level_id;
    public long category_id;
    public long asset_type;
    public long asset_owner_group_id;
    public short status;
    public setting building;
    public setting level;
    public setting category;
    public setting type;
    public setting status_value;
    public user_group owner_group;
    public Dictionary<long, asset_property> asset_properties;
    public Dictionary<long, asset_document> documents;
    public bool is_view;
    public bool is_book;

    public asset_property get_property(long id)
    {
      foreach (long key in this.asset_properties.Keys)
      {
        asset_property assetProperty = this.asset_properties[key];
        if (assetProperty.asset_property_id == id)
          return assetProperty;
      }
      return (asset_property) null;
    }

    public asset_property get_property(string parameter)
    {
      foreach (long key in this.asset_properties.Keys)
      {
        asset_property assetProperty = this.asset_properties[key];
        if (assetProperty.property_name == parameter)
          return assetProperty;
      }
      return (asset_property) null;
    }
  }
}
