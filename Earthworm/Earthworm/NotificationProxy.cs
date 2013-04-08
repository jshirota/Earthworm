using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Earthworm
{
    internal static class NotificationProxy
    {
        private static readonly Dictionary<Type, Type> TypeToType = new Dictionary<Type, Type>();

        #region Private

        private static Type Derive(Type baseType)
        {
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("_" + Guid.NewGuid().ToString("N")), AssemblyBuilderAccess.Run);

            TypeBuilder typeBuilder = assembly.DefineDynamicModule("_").DefineType("_" + baseType.Name, TypeAttributes.Public | TypeAttributes.Class, baseType);

            IEnumerable<PropertyInfo> propertyInfos = baseType.GetMappedProperties().Select(p => p.PropertyInfo).Where(p =>
            {
                MethodInfo g = p.GetGetMethod();
                MethodInfo s = p.GetSetMethod();

                return g != null && g.IsPublic && g.IsVirtual & !g.IsFinal
                    && s != null && s.IsPublic && s.IsVirtual & !s.IsFinal;
            });

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyInfo.Name, propertyInfo.Attributes, propertyInfo.PropertyType, null);

                MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.Virtual;

                MethodBuilder getMethod = typeBuilder.DefineMethod("get_" + propertyInfo.Name, attributes, propertyInfo.PropertyType, null);
                ILGenerator getMethodGenerator = getMethod.GetILGenerator();
                getMethodGenerator.Emit(OpCodes.Ldarg_0);
                getMethodGenerator.Emit(OpCodes.Call, propertyInfo.GetGetMethod());
                getMethodGenerator.Emit(OpCodes.Ret);

                MethodBuilder setMethod = typeBuilder.DefineMethod("set_" + propertyInfo.Name, attributes, typeof(void), new[] { propertyInfo.PropertyType });
                ILGenerator setMethodGenerator = setMethod.GetILGenerator();
                setMethodGenerator.Emit(OpCodes.Ldarg_0);
                setMethodGenerator.Emit(OpCodes.Ldarg_1);
                setMethodGenerator.Emit(OpCodes.Call, propertyInfo.GetSetMethod());
                setMethodGenerator.Emit(OpCodes.Ldarg_0);
                setMethodGenerator.Emit(OpCodes.Ldstr, propertyInfo.Name);
                setMethodGenerator.Emit(OpCodes.Call, baseType.GetMethod("RaisePropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance));
                setMethodGenerator.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getMethod);
                propertyBuilder.SetSetMethod(setMethod);
            }

            return typeBuilder.CreateType();
        }

        #endregion

        public static T Create<T>()
        {
            Type type = typeof(T);

            if (!TypeToType.ContainsKey(type))
                TypeToType.Add(type, Derive(type));

            return (T)Activator.CreateInstance(TypeToType[type]);
        }
    }
}
