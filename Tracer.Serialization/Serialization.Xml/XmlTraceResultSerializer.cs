using Core.Models;
using Serialization.Abstractions;
using Serialization.Models;
using System.Xml;
using System.Xml.Serialization;

namespace Serialization.Json
{
	public class XmlTraceResultSerializer : ITraceResultSerializer
	{
        public string Format { get; } = "xml";
        public void Serialize(TraceResult traceResult, Stream to)
		{
			var result = Mapper.MapToSerializationModel(traceResult);
			var xmlSerializer = new XmlSerializer(typeof(SerializationTraceResult));
            var xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;

            using (var xmlWriter = XmlWriter.Create(to, xmlSettings))
            {
                xmlSerializer.Serialize(xmlWriter, result);
            }
        }
	}
}