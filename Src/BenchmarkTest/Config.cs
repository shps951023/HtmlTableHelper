using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;

namespace BenchmarkTest
{
    public class Config : ManualConfig
    {
        public const int Iterations = 500;

        public Config()
        {
            Add(ConsoleLogger.Default);

            Add(CsvExporter.Default);
            Add(MarkdownExporter.GitHub);
            Add(HtmlExporter.Default);

            var md = new MemoryDiagnoser();
            Add(md);
            Add(TargetMethodColumn.Method);
            Add(StatisticColumn.Mean);
            Add(BaselineScaledColumn.Scaled);
            Add(md.GetColumnProvider());

            Add(Job.ShortRun
                   .WithLaunchCount(1)
                   .WithWarmupCount(2)
                   .WithUnrollFactor(Iterations)
                   .WithIterationCount(1)
            );
            Set(new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest));
            SummaryPerType = false;
        }
    }

}
