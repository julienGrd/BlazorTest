using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace BlazorTest.Client.Services
{
    public static class EnumExtensions
    {
        public static IDictionary<T, string> GetValuesWithDescription<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            List<T> lEnumValues = Enum.GetValues(typeof(T)).Cast<T>().ToList();

            return lEnumValues.ToDictionary(e => e,
                                            e => (e as Enum).GetDescription());
        }

        public static string GetDescription(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null) return null;
            var attribute = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute));
            return attribute?.Description ?? value.ToString();
        }
    }
}
