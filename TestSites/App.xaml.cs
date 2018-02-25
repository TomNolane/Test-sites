using Avalonia;
using Avalonia.Markup.Xaml;

namespace TestSites
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
