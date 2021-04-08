using Microsoft.Extensions.DependencyInjection;

namespace Tests
{
    public class Fixture
    {

        internal ServiceProvider ServiceProvider { get; }

        public Fixture()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMemoryCache();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

    }
}