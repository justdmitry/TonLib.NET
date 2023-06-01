using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TonLibDotNet
{
    public class SamplesRunner : IHostedService
    {
        private readonly IServiceProvider services;
        private readonly ITonClient tonClient;

        public SamplesRunner(IServiceProvider services, ITonClient tonClient)
        {
            this.services = services;
            this.tonClient = tonClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await tonClient.InitIfNeeded();
            await tonClient.Sync();

            var samples = services.GetServices(typeof(ISample)).Cast<ISample>().OrderBy(x => x.GetType().FullName).ToList();

            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            while (!cts.IsCancellationRequested)
            {
                var trimLength = "TonLibDotNet.Samples.".Length;
                Console.WriteLine();
                Console.WriteLine("Available samples:");
                for (var i = 0; i < samples.Count; i++)
                {
                    Console.WriteLine("  {0}: {1}", i, samples[i].GetType().FullName![trimLength..]);
                }

                while (!cts.IsCancellationRequested)
                {
                    Console.ResetColor();
                    Console.Write("Type sample number (0..{0}) to run, or type 'quit' or press Ctrl+C to quit: ", samples.Count - 1);
                    var choice = Console.ReadLine();

                    // Ctrl+C need some time to propagate...
                    await Task.Delay(100, CancellationToken.None);

                    if (string.IsNullOrEmpty(choice))
                    {
                        continue;
                    }

                    if ("quit" == choice)
                    {
                        cts.Cancel();
                        break;
                    }

                    if (!byte.TryParse(choice, out var index))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Failed to parse {0} as number", choice);
                        continue;
                    }

                    if (index >= samples.Count)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Too big value: {0}", index);
                        continue;
                    }

                    var sample = samples[index];

                    Console.WriteLine();
                    Console.WriteLine("Running {0} sample...", sample.GetType().FullName);
                    Console.WriteLine();

                    await sample.Run(Program.UseMainnet).ConfigureAwait(false);

                    // Loggers need some time to flush data to screen/console.
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

                    // write demos list again
                    break;
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Do nothing
            return Task.CompletedTask;
        }
    }
}
