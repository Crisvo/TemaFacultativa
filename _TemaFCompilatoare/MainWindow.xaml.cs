using _TemaFCompilatoare.Gramars;
using _TemaFCompilatoare.CompilerNS;
using System.Windows;


namespace _TemaFCompilatoare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        } 

        #region Members
        private Gramar _gramar = null;
        private DirectorSymbol _dirSymbol = null;
        private string _generatedCode = null;
        #endregion
        #region Methods
        /// <summary>
        /// Method for finding the text file in computer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            var result = dialog.ShowDialog();
            if(result == true)
            {
                var filePath = dialog.FileName;
                this.BrowseTextBox.Text = filePath;

                _gramar = new Gramar();
                _gramar.GetFromFile(this.BrowseTextBox.Text);
                this.GramarTextBox.Text = _gramar.ToString();
            }
        }
        /// <summary>
        /// Initiate the LL1 algorithm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitiateLL1Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _gramar.RemoveSameBeging();
                _gramar.RemoveLeftRecursion();
                _dirSymbol = new DirectorSymbol(ref _gramar);
                _dirSymbol.CalculateDirectorSymblos();
                if (!_dirSymbol.CheckLL1())
                {
                    MessageBox.Show("The gramar is not an LL1 gramar!\nThe analyse can not continue!", "Error", 
                                    MessageBoxButton.OK, 
                                    MessageBoxImage.Error);
                    return;
                }
                var CodeGenerator = new CodeLL1Generator(_gramar);
                _generatedCode = CodeGenerator.GenerateCode();
            }
            catch
            {
                MessageBox.Show("The gramar was not loaded!", "Error", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Information);
                return;
            }
            MessageBox.Show("LL1 is done!", "OK!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        /// <summary>
        /// Shows the gramar definition in textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowGramarButton_Click(object sender, RoutedEventArgs e)
        {
            if (_gramar != null)
            {
                this.GramarTextBox.Text = _gramar.ToString();
            }
            else
            {
                MessageBox.Show("The gramar was not loaded!", "Error", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Warning);
            }
        }
        /// <summary>
        /// Shows the director symbols in text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowDirSymb_Click(object sender, RoutedEventArgs e)
        {
            if(_dirSymbol != null)
            {
                this.GramarTextBox.Text = _dirSymbol.ToString();
            }
            else
            {
                MessageBox.Show("The LL1 analysis was not loaded!", "Error", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Warning);
            }
        }
        /// <summary>
        /// Compiles and executes the code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecudeCodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_generatedCode != null)
            {
                var compiler = new Compiler(_generatedCode);
                compiler.Complie();
            }
            else
            {
                MessageBox.Show("The code was not been generated!", "Error", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Warning);
            }
        }
        /// <summary>
        /// Shows the generated code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowCodeButton_Click(object sender, RoutedEventArgs e)
        {
            this.GramarTextBox.Text = _generatedCode;
        }
        #endregion
    }
}
