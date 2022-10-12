using Core.Models;
using Serialization.Abstractions;
using System.Text.Json;

namespace Serialization.Json
{
	public class JsonTraceResultSerializer : ITraceResultSerializer
	{
        public string Format { get; } = "json";
        public void Serialize(TraceResult traceResult, Stream to)
		{
			var result = Mapper.MapToSerializationModel(traceResult);
            var options = new JsonSerializerOptions { WriteIndented = true };
            JsonSerializer.Serialize(to, result, options);
		}
	}
}