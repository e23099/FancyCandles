using System.Windows.Controls;

namespace FancyCandles
{
    /// <summary>
    /// see https://stackoverflow.com/questions/677171/using-a-listuielement-as-the-itemssource-for-an-itemscontrol-causes-the-datate
    /// </summary>
    public class ItemsControlForUiElements : ItemsControl
    {
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ContentPresenter);
        }
    }
}
