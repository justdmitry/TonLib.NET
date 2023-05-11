using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Reflection;

namespace TonLibDotNet.Internal
{
    // Based on https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism?pivots=dotnet-7-0#configure-polymorphism-with-the-contract-model
    public class TonTypeResolver : DefaultJsonTypeInfoResolver
    {
        public static List<Assembly> AdditionalAsseblies { get; } = new List<Assembly>();

        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            var jsonTypeInfo = base.GetTypeInfo(type, options);

            if (type.IsAssignableTo(typeof(Types.TypeBase)))
            {
                jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    TypeDiscriminatorPropertyName = "@type",
                    IgnoreUnrecognizedTypeDiscriminators = true,
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                };

                AppendTypes(type, jsonTypeInfo.PolymorphismOptions.DerivedTypes);
            }

            return jsonTypeInfo;
        }

        protected void AppendTypes(Type type, IList<JsonDerivedType> list)
        {
            var types = AdditionalAsseblies.Append(GetType().Assembly)
                .SelectMany(a => a.GetExportedTypes()
                    .Where(x => x.IsAssignableTo(type) && !x.IsAbstract)
                    .Select(x => new
                    {
                        type = x,
                        schema = x.GetCustomAttribute<TLSchemaAttribute>(false),
                    }));

            foreach (var item in types.Where(x => x.schema != null))
            {
                list.Add(new JsonDerivedType(item.type, item.schema!.GetTLName()));
            }
        }
    }
}
