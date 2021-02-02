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
        /// <summary>
        /// Fetches BitInfo from a given enum
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="source">Particular enum value</param>
        /// <returns>BitInfo assosciated with given enum value</returns>
        public static BitInfo BitInfo<T>(this T source) where T : IConvertible
        {
            //fetch the attribute
            FieldInfo info = typeof(T).GetField(source.ToString());
            var attribute = (EnumInfo)info.GetCustomAttribute(typeof(EnumInfo));

            //return the BitInfo from the attribute
            return attribute.Info;
        }
    }
}
