using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;

namespace FancyCandles.Graphs
{
    /// <summary>
    /// Interaction logic for SubgraphAddWindow.xaml
    /// </summary>
    public partial class SubgraphAddWindow : Window
    {
        public SubgraphAddWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public List<Subgraph> AvaliableSubgraphs
        {
            get { return avaliableSubgraphs; }
        }
        private List<Subgraph> avaliableSubgraphs = new List<Subgraph>
        {
            new Volume(),
            new TrueRange(),
        };


        private Subgraph selectedSubgraph;

        public Subgraph GetAddedSubgraph()
        {
            string typeName = selectedSubgraph.GetType().Name;
            Subgraph result = selectedSubgraph;
            if (typeName == "Volume")
                selectedSubgraph = new Volume();
            else if (typeName == "TrueRange")
                selectedSubgraph = new TrueRange();
            else
                result = null; 
            return result;
        }

        private void listAvaliableSubgraphs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listElement = (ListBox)sender;
            selectedSubgraph = listElement.SelectedItem as Subgraph;
            if (selectedSubgraph != null)
            {
                ParserContext context = new ParserContext();
                context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                context.XmlnsDictionary.Add("i", "clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity");
                context.XmlnsDictionary.Add("local", "clr-namespace:FancyCandles;assembly=FancyCandles");
                string name = selectedSubgraph.GetType().Name;

                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = $"FancyCandles.Graphs.{name}.{name}UiSetting.xml";
                var stream = assembly.GetManifestResourceStream(resourceName);
                subgraphEditor.Children.Clear();
                if (stream != null)
                {
                    UIElement indicatorEditorElement = (UIElement)XamlReader.Load(stream, context);
                    subgraphEditor.DataContext = selectedSubgraph;
                    subgraphEditor.Children.Add(indicatorEditorElement);
                }
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
    }
}
