using Avalonia;
using Avalonia.Markup.Xaml;

namespace TestSites
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoaderPortableXaml.Load(this);
        }
    } 
}
