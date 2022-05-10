using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab9
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        public static class RandomGen2 { 
        private static Random _global = new Random();
            [ThreadStatic]
            private static Random _local;
            public static double NextDouble() {
                Random inst = _local;
                if (inst == null)
                {
                    int seed;
                    lock (_global) seed = _global.Next();
                    _local = inst = new Random(seed);
                }
                return inst.NextDouble();
            }
        }
        public static (double, double) MinimalizujFunkcje(int LiczbaIteracji, double minX,double  minY,double maxX, double maxY, Func<double, double, double> d) {
            Random rnd = new Random();
            int n = LiczbaIteracji;
            double x_active, y_active;
            double najnizszeX = rnd.Next((int)minX, (int)maxX);
            double najnizszeY = rnd.Next((int)minY, (int)maxY);
            double wynik;
            object semafor = new object();
            ParallelOptions po = new ParallelOptions
            {
                MaxDegreeOfParallelism = 2
            };
            Parallel.For ( 0,  n, i=>
            {
                x_active = rnd.Next((int)minX, (int)maxX);
                y_active = rnd.Next((int)minY, (int)maxY);
                wynik = d(x_active, y_active);
                lock (semafor)
                if (y_active < minY)
                {
                    minY = y_active;
                    minX = x_active;
                    i = 0;
                }
            });



            return ((double)najnizszeX, (double)najnizszeY);

        }
        private async void btnStudenci_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Uczelnia;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            SqlCommand cmd = new SqlCommand("EXEC PobierzStudentow",conn);
            conn.Open();
            var czytnik = await cmd.ExecuteReaderAsync();
            while (czytnik.Read()) {
                lstStudenci.Items.Add(czytnik["Nazwisko"]);
            }
            czytnik.Close();
            conn.Close();
        }
    }
}
