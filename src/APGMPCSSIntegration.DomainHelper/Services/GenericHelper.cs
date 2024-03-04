using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APGDigitalIntegration.DomainHelper.Services
{
    public static class GenericHelper
    {

        public static string GetValue<T>(T obj,string propertyName)
        {
            Type t = obj.GetType();
            PropertyInfo prop = t.GetProperty(propertyName);
            return Convert.ToString(prop.GetValue(obj));
        }

        public static dynamic GetValueObject<T>(T obj, string propertyName)
        {
            Type t = obj.GetType();
            PropertyInfo prop = t.GetProperty(propertyName);
            return prop.GetValue(obj);
        }

    }
}
