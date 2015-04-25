using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Earthworm
{
    internal static class Proxy
    {
        private static readonly Func<Type, Type> DeriveMemoized = Memoization.Memoize<Type, Type>(Derive);

        private static Type Derive(Type baseType)
        {
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("_" + Guid.NewGuid().ToString("N")), AssemblyBuilderAccess.Run);

            var typeBuilder = assembly.DefineDynamicModule("_").DefineType("_" + baseType.Name, TypeAttributes.Public | TypeAttributes.Class, baseType);

            var properties = baseType.GetProperties().Where(p =>
            {
                var g = p.GetGetMethod();
                var s = p.GetSetMethod();

                return g != null && g.IsPublic && g.IsVirtual && !g.IsFinal
                    && s != null && s.IsPublic && s.IsVirtual && !s.IsFinal;
            });

            foreach (var p in properties)
            {
                var mapped = Attribute.GetCustomAttributes(p).OfType<Mapped>().SingleOrDefault();

                if (mapped == null)
                    continue;

                var propertyBuilder = typeBuilder.DefineProperty(p.Name, p.Attributes, p.PropertyType, null);

                var attributes = MethodAttributes.Public | MethodAttributes.Virtual;

                var getMethod = typeBuilder.DefineMethod("get_" + p.Name, attributes, p.PropertyType, null);
                var getGenerator = getMethod.GetILGenerator();
                getGenerator.Emit(OpCodes.Ldarg_0);
                getGenerator.Emit(OpCodes.Ldstr, mapped.FieldName);
                getGenerator.Emit(OpCodes.Call, baseType.GetMethod("get_Item"));
                getGenerator.Emit(OpCodes.Unbox_Any, p.PropertyType);
                getGenerator.Emit(OpCodes.Ret);

                var setMethod = typeBuilder.DefineMethod("set_" + p.Name, attributes, typeof(void), new[] { p.PropertyType });
                var setGenerator = setMethod.GetILGenerator();
                setGenerator.Emit(OpCodes.Ldarg_0);
                setGenerator.Emit(OpCodes.Ldarg_1);
                setGenerator.Emit(OpCodes.Call, p.GetSetMethod());
                setGenerator.Emit(OpCodes.Ldarg_0);
                setGenerator.Emit(OpCodes.Ldstr, mapped.FieldName);
                setGenerator.Emit(OpCodes.Ldarg_1);
                setGenerator.Emit(OpCodes.Box, p.PropertyType);
                setGenerator.Emit(OpCodes.Call, baseType.GetMethod("set_Item"));
                setGenerator.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getMethod);
                propertyBuilder.SetSetMethod(setMethod);
            }

            return typeBuilder.CreateType();
        }

        public static T Create<T>() where T : IEntity
        {
            return (T)Activator.CreateInstance(DeriveMemoized(typeof(T)));
        }
    }
}
