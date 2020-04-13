using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CSharp_Lab2_WPF.model;
using CSharp_Lab2_WPF.service;
using Microsoft.Win32;
using OfficeOpenXml;

namespace CSharp_Lab2_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        List<Threat> threats;
        List<Threat> visibleThreats;
        LocalStorageManager fsmanager;
        int curPage = 1;
        static int PAGE_SIZE = 25;
        int kekn = 0;
        public MainWindow()
        {
            InitializeComponent();
            image.Source = new BitmapImage(new Uri($"/keks/kek-{kekn}.jpg", UriKind.Relative));
            
            fsmanager = new LocalStorageManager();
            visibleThreats = new List<Threat>();

            if (fsmanager.FindLocalStorage(out threats))
            {
                MessageBox.Show("Локальное хранилище найдено!", "Good news", MessageBoxButton.OK);
                for (int i = 0; i < PAGE_SIZE && i < threats.Count; i++)
                    visibleThreats.Add(threats[i]);
            }
            else
            {

                
                var res = MessageBox.Show("Локальное хранилище не найдено!\nСкачать данные из официального банка данных угроз ФСТЭК России?", "Bad news", MessageBoxButton.YesNo);
                if(res == MessageBoxResult.Yes)
                {
                    try
                    {
                        FileInfo file;
                        fsmanager.DownloadFile(out file);
                        if (fsmanager.TryParseThreats(file, out threats))
                        {
                            for (int i = 0; i < PAGE_SIZE && i < threats.Count; i++)
                                visibleThreats.Add(threats[i]);
                        }
                    }
                    catch (WebException)
                    {
                        MessageBox.Show("Ошибка загрузки файла", "Bad News", MessageBoxButton.OK);
                        return;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("Недостаточно прав для создания директории локального хранилища", "Bad News", MessageBoxButton.OK);
                        return;
                    }
                }
            }

            this.dataGrid.ItemsSource = visibleThreats;   

        }

        private void UpdatePage(int page)
        {
            if (page > 0 && (page - 1) * PAGE_SIZE < threats.Count)
            {
                
                this.curPage = page;
                int i = (page - 1) * PAGE_SIZE;
                visibleThreats.Clear();
                while(i < page * PAGE_SIZE && i < threats.Count)
                {
                    visibleThreats.Add(threats[i++]);
                }
                this.dataGrid.Items.Refresh();
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatePage(curPage - 1);
            if (kekn == 0) kekn = 14;
            image.Source = new BitmapImage(new Uri($"/keks/kek-{--kekn % 14}.jpg", UriKind.Relative));
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatePage(curPage + 1);
            image.Source = new BitmapImage(new Uri($"/keks/kek-{++kekn % 14}.jpg", UriKind.Relative));
            
        }

        private void UpdateDBButton_Click(object sender, RoutedEventArgs e)
        {
            FileInfo file;
            try
            {
                fsmanager.DownloadFile(out file);
            }
            catch (WebException)
            {
                MessageBox.Show("Ошибка загрузки файла", "Bad News", MessageBoxButton.OK);
                return;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Недостаточно прав для создания директории локального хранилища", "Bad News", MessageBoxButton.OK);
                return;
            }
            List<Threat> newThreats;
            if(fsmanager.TryParseThreats(file, out newThreats))
            {
                int n;
                DifferencesWindow difWindow = new DifferencesWindow(threats, newThreats, out n);
                if (n == 0)
                {
                    MessageBox.Show("Никаких изменений не произошло", "Good news", MessageBoxButton.OK);
                    difWindow.Close();
                }
                else
                {
                    var res = MessageBox.Show($"Локальная база успешно обновлена.\nВсего изменений: {n}\nПоказать изменения?", "Good news", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        difWindow.Show();
                    }
                    threats = newThreats;
                    UpdatePage(1);
                    
                }
            }
            
        }

        private void SaveLocalStorage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (.xlsx)|*.xlsx";
            //saveFileDialog.
            if (saveFileDialog.ShowDialog() == true)
                fsmanager.SaveLocalStorage(new FileInfo(saveFileDialog.FileName));
        }

        private void dataGrid_SelectionChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            
        }
    }
}
