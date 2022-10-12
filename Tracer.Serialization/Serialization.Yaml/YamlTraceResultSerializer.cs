using Core.Models;
using Serialization.Abstractions;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Serialization.Json
{
	public class YamlTraceResultSerializer : ITraceResultSerializer
	{
        public string Format { get; } = "yaml";
        public void Serialize(TraceResult traceResult, Stream to)
		{
			var result = Mapper.MapToSerializationModel(traceResult);
			
			var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var yamlResult = serializer.Serialize(result);

			using var sw = new StreamWriter(to);
			sw.Write(Encoding.Default.GetBytes(yamlResult));
            sw.Flush();	
		}
	}
}