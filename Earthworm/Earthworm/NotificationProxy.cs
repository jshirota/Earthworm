using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Earthworm
{
    internal static class NotificationProxy
    {
        private static readonly ConcurrentDictionary<Type, Type> TypeToType = new ConcurrentDictionary<Type, Type>();

        #region Private

        private static Type Derive(Type baseType)
        {
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("_" + Guid.NewGuid().ToString("N")), AssemblyBuilderAccess.Run);

            var typeBuilder = assembly.DefineDynamicModule("_").DefineType("_" + baseType.Name, TypeAttributes.Public | TypeAttributes.Class, baseType);

            var propertyInfos = baseType.GetMappedProperties().Select(p => p.PropertyInfo).Where(p =>
            {
                var g = p.GetGetMethod();
                var s = p.GetSetMethod();

                return g != null && g.IsPublic && g.IsVirtual && !g.IsFinal
                    && s != null && s.IsPublic && s.IsVirtual && !s.IsFinal;
            });

            foreach (var propertyInfo in propertyInfos)
            {
                var propertyBuilder = typeBuilder.DefineProperty(propertyInfo.Name, propertyInfo.Attributes, propertyInfo.PropertyType, null);

                var attributes = MethodAttributes.Public | MethodAttributes.Virtual;

                var getMethod = typeBuilder.DefineMethod("get_" + propertyInfo.Name, attributes, propertyInfo.PropertyType, null);
                var getGenerator = getMethod.GetILGenerator();
                getGenerator.Emit(OpCodes.Ldarg_0);
                getGenerator.Emit(OpCodes.Call, propertyInfo.GetGetMethod());
                getGenerator.Emit(OpCodes.Ret);

                var setMethod = typeBuilder.DefineMethod("set_" + propertyInfo.Name, attributes, typeof(void), new[] { propertyInfo.PropertyType });
                var setGenerator = setMethod.GetILGenerator();
                setGenerator.Emit(OpCodes.Ldarg_0);
                setGenerator.Emit(OpCodes.Ldarg_1);
                setGenerator.Emit(OpCodes.Call, propertyInfo.GetSetMethod());
                setGenerator.Emit(OpCodes.Ldarg_0);
                setGenerator.Emit(OpCodes.Ldstr, propertyInfo.Name);
                setGenerator.Emit(OpCodes.Call, baseType.GetMethod("RaisePropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance));
                setGenerator.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getMethod);
                propertyBuilder.SetSetMethod(setMethod);
            }

            return typeBuilder.CreateType();
        }

        #endregion

        public static T Create<T>()
        {
            return (T)Activator.CreateInstance(TypeToType.GetOrAdd(typeof(T), Derive));
        }
    }
}
