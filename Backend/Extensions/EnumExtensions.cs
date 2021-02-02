using Backend.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Extensions
{
    public static class EnumExtensions
    {
        public static BitInfo BitInfo<T>(this T source) where T : IConvertible
        {
            FieldInfo info = typeof(T).GetField(source.ToString());
            var attribute = (EnumInfo)info.GetCustomAttribute(typeof(EnumInfo));

            return attribute.Info;
        }
    }
}
