using System;
using System.Reflection;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Model
{
    public class ModelData
    {
        public virtual int id { get; set; }

        public ModelData() { }

        public ModelData(Dictionary<string, object> dict)
        {
            Update(dict);
        }

        public void Update(Dictionary<string, object> dict)
        {
            // reflection get properties & scripts field crypt.

            object obj;
            Type dataType = GetType();
            PropertyInfo[] objectProperties = dataType.GetProperties();
            for (int i = 0; i < objectProperties.Length; i++)
            {
                PropertyInfo propertyInfo = objectProperties[i];
                ModelUpdateIgnoreAttribute attribute = Attribute.GetCustomAttribute(propertyInfo, typeof(ModelUpdateIgnoreAttribute)) as ModelUpdateIgnoreAttribute;
                if (attribute != null) continue;

                if (propertyInfo.CanRead && propertyInfo.CanWrite)
                {
                    if (dict.TryGetValue(propertyInfo.Name, out obj))
                    {
                        if (obj.GetType().Equals(propertyInfo.PropertyType)) propertyInfo.SetValue(this, obj, null);
                        else
                        {
                            if (propertyInfo.PropertyType.Equals(typeof(ObscuredInt)))
                            {
                                ObscuredInt cryptInt = (int)obj;
                                propertyInfo.SetValue(this, cryptInt, null);
                            }
                            else if (propertyInfo.PropertyType.Equals(typeof(ObscuredFloat)))
                            {
                                ObscuredFloat cryptFlot = Convert.ToSingle(obj);
                                propertyInfo.SetValue(this, cryptFlot, null);
                            }
                            else if (propertyInfo.PropertyType.Equals(typeof(ObscuredString)))
                            {
                                ObscuredString cryptString = (string)obj;
                                propertyInfo.SetValue(this, cryptString, null);
                            }
                            else if (propertyInfo.PropertyType.Equals(typeof(ObscuredLong)))
                            {
                                ObscuredLong cryptLong = (long)obj;
                                propertyInfo.SetValue(this, cryptLong, null);
                            }
                            else if (propertyInfo.PropertyType.Equals(typeof(ObscuredShort)))
                            {
                                ObscuredShort cryptShort = Convert.ToInt16(obj);
                                propertyInfo.SetValue(this, cryptShort, null);
                            }
                            else if (propertyInfo.PropertyType.Equals(typeof(ObscuredByte)))
                            {
                                ObscuredByte cryptByte = Convert.ToByte(obj);
                                propertyInfo.SetValue(this, cryptByte, null);
                            }
                            else
                            {
                                propertyInfo.SetValue(this, Convert.ChangeType(obj, propertyInfo.PropertyType), null);
                            }
                        }
                    }
                }
            }
            OnUpdate();
        }

        protected virtual void OnUpdate() { }
    }
}
