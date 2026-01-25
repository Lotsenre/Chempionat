using System.Windows.Controls;

namespace VendingMachineDesktop.Views.Pages;

public partial class UnderDevelopmentPage : Page
{
    public UnderDevelopmentPage(string sectionName = "Раздел")
    {
        InitializeComponent();
        PageTitle.Text = $"{sectionName} - в разработке";
    }
}
