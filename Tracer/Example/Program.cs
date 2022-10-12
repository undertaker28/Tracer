using Core.Tracers;
using Tests;
using Tracer.Serialization;

var tracer = new MainTracer();
var test = new TestMethods(tracer);

var t1 = new Thread(() =>
{
	test.M1();
	test.M2();
	test.M3();
});
t1.Start();

var t2 = new Thread(() =>
{
	test.M1();
	test.M2();
	test.M3();
});
t2.Start();

t1.Join();
t2.Join();

var traceResult = tracer.GetTraceResult();

const string pluginPath = "../../../Plugins/";
const string resultPath = "../../../TraceResults/TraceResult";

var pluginList = PluginLoader.Load(pluginPath);

for (int i = 0; i < pluginList.Count; i++)
{
	using (var fileStream = new FileStream($"{resultPath}.{pluginList[i].Format}", FileMode.Create, FileAccess.Write))
	{
        pluginList[i].Serialize(traceResult, fileStream);
    }
}