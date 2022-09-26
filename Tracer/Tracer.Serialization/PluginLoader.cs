using Serialization.Abstractions;
using System.Reflection;

namespace Tracer.Serialization
{
	public static class PluginLoader
	{

		public static List<ITraceResultSerializer> Load(string directory)
		{
			var result = new List<ITraceResultSerializer>();

			DirectoryInfo pluginDirectory = new DirectoryInfo(directory);
			if (!pluginDirectory.Exists)
				pluginDirectory.Create();

			var pluginFiles = Directory.GetFiles(directory, "*.dll");
            var interfaceType = typeof(ITraceResultSerializer);
            foreach (var file in pluginFiles)
			{
                var assembly = Assembly.LoadFrom(file);

                var types = assembly.GetTypes()
                    .Where(t => t.GetInterfaces().Contains(interfaceType));

                foreach (var type in types)
                {
                    var plugin = Activator.CreateInstance(type) as ITraceResultSerializer;
                    result.Add(plugin);
                }
            }
			return result;
        }
	}
}