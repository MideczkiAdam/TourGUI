using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.Sqlite;

namespace TourGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString;
        private SqliteConnection connection;
        private List<Versenyzo> datalist;
        public MainWindow()
        {
            InitializeComponent();
            connectionString = "Filename=tour.sql";
            connection = new(connectionString);
            connection.Open();
            datalist = [];
            string queryText = "SELECT versenyzo.id, versenyzo.nev, csapat.csapatNev, versenyzo.nemzetiseg FROM versenyzo INNER JOIN csapat ON versenyzo.csapatId = csapat.id";
            SqliteCommand command = new(queryText, connection);
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string nev = reader.GetString(1);
                string csapatNev = reader.GetString(2);
                string nemzetiseg = reader.GetString(3);
                datalist.Add(new(id, nev, csapatNev, nemzetiseg));
            }
            reader.Close();
            dataDg.ItemsSource = datalist;
            dataDg.SelectedIndex = 0;
        }

        private void dataDg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Versenyzo current = (Versenyzo)dataDg.SelectedItem;
            string queryText = "SELECT ido FROM eredmeny WHERE szakasz=5 AND versenyzoId=@id";
            SqliteCommand command = new(queryText, connection);
            command.Parameters.AddWithValue("@id", current.Id);
            string ido = (string)command.ExecuteScalar();
            szakaszLb.Content = "5. szakasz ideje: " + ido;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            csapatonkentLista.ItemsSource = datalist.GroupBy(v => v.CsapatNev)
                .Select(g =>  $"{g.Key}: {g.Count()} fő" )
                .ToList();
        }

        private void magyarBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Magyar versenyzők száma: {datalist.Count(v => v.Nemzetiseg == "HUN")}");
        }
    }
}