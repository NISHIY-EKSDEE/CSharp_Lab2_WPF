using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CSharp_Lab2_WPF.model;
using System.Linq;

namespace CSharp_Lab2_WPF
{
    /// <summary>
    /// Логика взаимодействия для DifferencesWindow.xaml
    /// </summary>
    public partial class DifferencesWindow : Window
    {
        List<Threat> oldData;
        List<Threat> newData;
        public DifferencesWindow(List<Threat> oldList, List<Threat> newList, out int numberOfDifferences)
        {
            newData = newList.Except(oldList).ToList();
            oldData = oldList.Except(newList).ToList();
            foreach(var threat in oldData)
            {
                if(newData.Find(e => e.Id == threat.Id) == null)
                {
                    newData.Add(new Threat(threat.Id, "-", "-", "-", "-", "-", "-", "-"));
                }
            }
            foreach (var threat in newData)
            {
                if (oldData.Find(e => e.Id == threat.Id) == null)
                {
                    oldData.Add(new Threat(threat.Id, "-", "-", "-", "-", "-", "-", "-"));
                }
            }
            oldData.Sort((e1, e2) => (int)(e1.Id - e2.Id));
            newData.Sort((e1, e2) => (int)(e1.Id - e2.Id));
            numberOfDifferences = newData.Count;
            InitializeComponent();
            newDataGrid.ItemsSource = newData;
            oldDataGrid.ItemsSource = oldData;
        }

        private void oldDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            newDataGrid.SelectedIndex = oldDataGrid.SelectedIndex;
        }

        private void newDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            oldDataGrid.SelectedIndex = newDataGrid.SelectedIndex;
        }
    }
}
