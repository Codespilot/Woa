using System.Reflection.Emit;
using System.Reflection;
using static Woa.Common.DynamicTypeCreatorBase;

namespace Woa.Common;

internal partial class DynamicTypeCreatorBase : IBaseObject, IAfterProperty, IAfterAttribute
{
	private readonly TypeBuilder _typeBuilder;
	private readonly List<(PropertyBuilder, MethodBuilder, MethodBuilder)> _properties = new();

	private readonly Type _parentType;

	private DynamicTypeCreatorBase(string className, Type parentType = null, Type[] interfaces = null)
	{
		_parentType = parentType;
		var assemblyName = new AssemblyName(className);
		var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
		var modelBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name!);
		_typeBuilder = modelBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class, _parentType, interfaces);
		_typeBuilder.SetParent(_parentType);
	}

	/// <summary>
	/// Begins creating type using the specified name and saved in the specified directory.
	/// Use this overload to save the resulting .dll in a specified directory.
	/// </summary>
	/// <param name="className">Class name for new type</param>
	/// <param name="parentType"></param>
	/// <param name="interfaces"></param>
	/// <returns></returns>
	public static IBaseObject Create(string className, Type parentType = null, Type[] interfaces = null)
	{
		return new DynamicTypeCreatorBase(className, parentType, interfaces);
	}

	/// <summary>
	/// Adds constructors to new type that match all constructors on base type.
	/// Parameters are passed to base type.
	/// </summary>
	/// <returns></returns>
	public IEmptyObject AddPassThroughConstructors()
	{
		if (_parentType == null)
		{
			return this;
		}

		foreach (var constructorInfo in _parentType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
		{
			var parameters = constructorInfo.GetParameters();
			var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
			var requiredModifiers = parameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
			var optionalModifiers = parameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();
			var constructorBuilder = _typeBuilder.DefineConstructor(MethodAttributes.Public, constructorInfo.CallingConvention, parameterTypes, requiredModifiers, optionalModifiers);
			for (var index = 0; index < parameters.Length; index++)
			{
				var parameter = parameters[index];
				var parameterBuilder = constructorBuilder.DefineParameter(index + 1, parameter.Attributes, parameter.Name);
				if (((int)parameter.Attributes & (int)ParameterAttributes.HasDefault) != 0)
				{
					parameterBuilder.SetConstant(parameter.RawDefaultValue);
				}

				foreach (var attributeBuilder in GetCustomAttrBuilders(parameter.CustomAttributes))
				{
					parameterBuilder.SetCustomAttribute(attributeBuilder);
				}
			}

			//ConstructorBuilder _cBuilder = _tBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Any, argTypes);
			var generator = constructorBuilder.GetILGenerator();
			generator.Emit(OpCodes.Nop);
			//arg0=new obj, arg1-arg3=passed params. Push onto stack for call to base class
			generator.Emit(OpCodes.Ldarg_0);
			for (var index = 1; index <= parameters.Length; index++)
			{
				generator.Emit(OpCodes.Ldarg, index);
			}

			generator.Emit(OpCodes.Call, constructorInfo);
			generator.Emit(OpCodes.Ret);
		}

		return this;
	}

	/// <summary>
	/// Adds a new property to type with specified name and type.
	/// </summary>
	/// <param name="name">Name of property</param>
	/// <param name="type">Type of property</param>
	/// <returns></returns>
	public IAfterProperty AddProperty(string name, Type type)
	{
		//base property
		var propertyBuilder = _typeBuilder.DefineProperty(name, PropertyAttributes.None, type, Type.EmptyTypes);
		//backing field
		var fieldBuilder = _typeBuilder.DefineField($"m_{name}", type, FieldAttributes.Private);

		//get method
		const MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
		var getterBuilder = _typeBuilder.DefineMethod($"get_{name}", methodAttributes, type, Type.EmptyTypes);
		var getterGenerator = getterBuilder.GetILGenerator();
		getterGenerator.Emit(OpCodes.Ldarg_0);
		getterGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
		getterGenerator.Emit(OpCodes.Ret);

		//set method
		var setterBuilder = _typeBuilder.DefineMethod($"set_{name}", methodAttributes, null, new[] { type });
		var setterGenerator = setterBuilder.GetILGenerator();
		setterGenerator.Emit(OpCodes.Ldarg_0);
		setterGenerator.Emit(OpCodes.Ldarg_1);
		setterGenerator.Emit(OpCodes.Stfld, fieldBuilder);
		setterGenerator.Emit(OpCodes.Ret);

		_properties.Add((propertyBuilder, getterBuilder, setterBuilder));
		return this;
	}

	/// <summary>
	/// Adds an attribute to a property just added.
	/// </summary>
	/// <param name="attributeType">Type of attribute</param>
	/// <param name="constructorArgTypes">Types of attribute's constructor parameters</param>
	/// <param name="constructorArgs">Values to pass in to attribute's constructor. Must match in type and order of constructorArgTypes parameter</param>
	/// <returns></returns>
	public IAfterAttribute AddPropertyAttribute(Type attributeType, Type[] constructorArgTypes, params object[] constructorArgs)
	{
		if (constructorArgTypes.Length != constructorArgs.Length)
		{
			throw new Exception("Type count must match arg count for attribute specification");
		}

		var attributeConstructor = attributeType.GetConstructor(constructorArgTypes);
		if (attributeConstructor == null)
		{
			throw new NullReferenceException("Attribute constructor not found");
		}

		for (var i = 0; i < constructorArgTypes.Length; i++)
		{
			var attributeBuilder = new CustomAttributeBuilder(attributeConstructor, constructorArgs);
			_properties.Last().Item1.SetCustomAttribute(attributeBuilder);
		}

		return this;
	}

	/// <summary>
	/// Completes building type, compiles it, and returns the resulting type
	/// </summary>
	/// <returns></returns>
	public Type Complete()
	{
		foreach (var (property, getter, setter) in _properties)
		{
			property.SetGetMethod(getter);
			property.SetSetMethod(setter);
		}

		return _typeBuilder.CreateType();
	}

	private static IEnumerable<CustomAttributeBuilder> GetCustomAttrBuilders(IEnumerable<CustomAttributeData> customAttributes)
	{
		return customAttributes.Select(attribute =>
		{
			var attributeArgs = attribute.ConstructorArguments
			                             .Select(a => a.Value)
			                             .ToArray();
			var namedPropertyInfos = attribute.NamedArguments!
			                                  .Select(a => a.MemberInfo)
			                                  .OfType<PropertyInfo>()
			                                  .ToArray();
			var namedPropertyValues = attribute.NamedArguments
			                                   .Where(a => a.MemberInfo is PropertyInfo)
			                                   .Select(a => a.TypedValue.Value)
			                                   .ToArray();
			var namedFieldInfos = attribute.NamedArguments
			                               .Select(a => a.MemberInfo)
			                               .OfType<FieldInfo>()
			                               .ToArray();
			var namedFieldValues = attribute.NamedArguments
			                                .Where(a => a.MemberInfo is FieldInfo)
			                                .Select(a => a.TypedValue.Value)
			                                .ToArray();
			return new CustomAttributeBuilder(attribute.Constructor, attributeArgs, namedPropertyInfos, namedPropertyValues, namedFieldInfos, namedFieldValues);
		}).ToArray();
	}
}

internal partial class DynamicTypeCreatorBase
{
	public interface IBaseObject
	{
		IEmptyObject AddPassThroughConstructors();
	}

	public interface IEmptyObject
	{
		IAfterProperty AddProperty(string name, Type type);
	}

	public interface IAfterProperty : IEmptyObject, IFinishBuild
	{
		IAfterAttribute AddPropertyAttribute(Type attributeType, Type[] constructorArgTypes, params object[] constructorArgs);
	}

	public interface IAfterAttribute : IEmptyObject, IFinishBuild
	{
	}

	public interface IFinishBuild
	{
		Type Complete();
	}
}