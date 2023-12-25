using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;

namespace Registration
{
    public partial class MainWindow : Form
    {
        public static Label Label2 { get; set; }
        public static DataGridView DataGridForForms { get; set; }
        public static Button SqlInfoForForm { get; set; }
        public static bool RuntimeChecker { get; set; }
        public static int ResultColumn  {get; set;}
        public bool FirstPass;
        public static string Path  = "";
        public static string FontData { get; set;}

        public int SelectedRowIndex;
        public int SelectedColumnIndex;
        public string RowName;
        public string ColumnName;
        public string CurrentCellValue;
        public string IdName;

        private Rectangle _buttonOriginalRectangle;
        private Rectangle _dataGridOriginalRectangle;     
        private Rectangle _githubTextOriginalRectangle;
        private Rectangle _linkGitHubTextOriginalRectangle;    
        private Rectangle _originalFormSize;
        private Rectangle _titleImageRectangle;
        private Rectangle _sqlInfoRectangle;
        private Rectangle _connectionStringRectangle;
        private Rectangle _connectionStatusRectangle;

        private ConnectionForm _connectionForm;
        private UserInput _userInput;

        public static bool changed;
        
        public MainWindow()
        {
            InitializeComponent();
        }
        
        public  void SetConnectionToOffline()
        {
            RuntimeChecker = false;
            LBL_CONNECTION_STATUS.Text = "O F F L I N E";
            LBL_CONNECTION_STATUS.ForeColor = Color.Red;
            ConnectionForm.SqlChecker = false;
            if (RuntimeChecker)
            {
                ConnectionForm.con.Close();
            }

            RuntimeChecker = false;
        }
        public static void ReadSettings(string fontPath)
        {
            if (!File.Exists(fontPath))
            {
                File.Create(fontPath);
            }
            else
            {
                string[] allLines = File.ReadAllLines(fontPath);
                DataGridForForms.Font = new Font(allLines[0] , float.Parse(allLines[1]));

                var foreGroundComponents = allLines[2].Split(',');
                var fcColor = GetColorFromComponents(foreGroundComponents);
                DataGridForForms.ForeColor = fcColor;

                var backGroundComponents = allLines[3].Split(',');
                var fbColor = GetColorFromComponents(backGroundComponents);
                DataGridForForms.BackgroundColor = fbColor;  
            }
        }

        private static Color GetColorFromComponents(IReadOnlyList<string> components)
        {
            return Color.FromArgb(Convert.ToInt32(components[0]),
                Convert.ToInt32(components[1]),
                Convert.ToInt32(components[2]),
                Convert.ToInt32(components[3]));
        }
         
        public void CreateDirIfItDoesNotExist(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataGridForForms = DATA_GRID;
            Path = "";
            var columnDirectoryPath = Application.StartupPath + "\\Columns";
            var dataDirectoryPath = Application.StartupPath + "\\Data";
            FontData = Application.StartupPath + "\\font.txt";
            
            ReadSettings(FontData);
           
            CreateDirIfItDoesNotExist(columnDirectoryPath);
            CreateDirIfItDoesNotExist(dataDirectoryPath);

            LBL_CONNECTION_STATUS.Text = @"O F F L I N E";
            LBL_CONNECTION_STATUS.ForeColor = Color.Red;
            ConnectionForm.SqlChecker = false;
            Search_form.dvg = DATA_GRID;
            
            Label2 = LBL_CONNECTION_STATUS;
            
            SqlInfoForForm = BTN_SQL_INFO;

            if (LBL_CONNECTION_STATUS.ForeColor == Color.Red)
            {
                BTN_SQL_INFO.Enabled = false;
            }
            if (File.Exists(Application.StartupPath + "/Columns/entire_data_columns_data.txt") == true) 
            {
                DATA_GRID.Columns.Clear();
                
                string[] limes = File.ReadAllLines(Application.StartupPath + "/Columns/entire_data_columns_data.txt");
                 foreach (var lime in limes)
                {
                    DATA_GRID.Columns.Add(lime + "_" , lime);
                }
                
            }
            DatabaseEntry.datagridview1 = DATA_GRID;
            if (File.Exists(Application.StartupPath + "/Data/entire_data.txt") == true)
            {
                string[] allLines = File.ReadAllLines(Application.StartupPath+ "/Data/entire_data.txt");
                var counterRow = 0;
                foreach (var line in allLines)
                {
                    if (line != "//")
                    {
                        counterRow++;
                    }
                    else if (line == "//")
                    {
                        break;
                    }
                }

                for (var i = 0; i < counterRow; i++)
                {
                    DATA_GRID.Rows.Add();
                }
                
                var s = 0;
                var l = 0;

                foreach (var line in allLines)
                {
                    if (line != "//")
                    {
                        DATA_GRID.Rows[l].Cells[s].Value = line;
                        
                        l++;
                    }

                    if (line == "//")
                    {
                        s++;
                        l = 0;
                    }    
                }
            }
            _originalFormSize = new Rectangle(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height);
            _buttonOriginalRectangle = new Rectangle(BTN_ADD.Location.X, BTN_ADD.Location.Y, BTN_ADD.Width, BTN_ADD.Height);
            
            _dataGridOriginalRectangle = new Rectangle(DATA_GRID.Location.X, DATA_GRID.Location.Y, DATA_GRID.Width, DATA_GRID.Height);
            _titleImageRectangle = new Rectangle(IMG_BOX_TITLE.Location.X, IMG_BOX_TITLE.Location.Y, IMG_BOX_TITLE.Width, IMG_BOX_TITLE.Height);
            _githubTextOriginalRectangle = new Rectangle(LBL_GiTHUB.Location.X, LBL_GiTHUB.Location.Y, LBL_GiTHUB.Width, LBL_GiTHUB.Height);
            _linkGitHubTextOriginalRectangle = new Rectangle(LINK_LBL_GITHUB.Location.X, LINK_LBL_GITHUB.Location.Y, LINK_LBL_GITHUB.Width, LINK_LBL_GITHUB.Height);
           
            _sqlInfoRectangle = new Rectangle(BTN_SQL_INFO.Location.X, BTN_SQL_INFO.Location.Y, BTN_SQL_INFO.Width, BTN_SQL_INFO.Height);
            _connectionStringRectangle = new Rectangle(LBL_CONNECTION.Location.X, LBL_CONNECTION.Location.Y, LBL_CONNECTION.Width, LBL_CONNECTION.Height);
            _connectionStatusRectangle = new Rectangle(LBL_CONNECTION_STATUS.Location.X, LBL_CONNECTION_STATUS.Location.Y, LBL_CONNECTION_STATUS.Width, LBL_CONNECTION_STATUS.Height);
        }

        private void ResizeControl(Rectangle r, Control c)
        {
            var xRatio = Width / (float)(_originalFormSize.Width);
            var yRatio = Height / (float)(_originalFormSize.Height);

            var newX = (int)(r.Location.X * xRatio);
            var newY = (int)(r.Location.Y * yRatio);

            var newWidth = (int)(r.Width * xRatio);
            var newHeight = (int)(r.Height * yRatio);

            c.Location = new Point(newX, newY);
            c.Size = new Size(newWidth, newHeight);
        }

        private void ResizeControlChildren()
        {
            ResizeControl(_buttonOriginalRectangle, BTN_ADD);
            
            ResizeControl(_dataGridOriginalRectangle, DATA_GRID);
            ResizeControl(_titleImageRectangle, IMG_BOX_TITLE);
            ResizeControl(_githubTextOriginalRectangle, LBL_GiTHUB);
            ResizeControl(_linkGitHubTextOriginalRectangle, LINK_LBL_GITHUB);
            
            ResizeControl (_sqlInfoRectangle, BTN_SQL_INFO);
            ResizeControl(_connectionStatusRectangle, LBL_CONNECTION_STATUS);
            ResizeControl(_connectionStringRectangle, LBL_CONNECTION);
        }

        private void DeleteRow()
        {
            if (LBL_CONNECTION_STATUS.ForeColor != Color.Red)
            {
                ConnectionForm.con.Open();
                var command = new SqlCommand()
                {
                    Connection = ConnectionForm.con,
                    CommandType = CommandType.Text,
                    CommandText = "DELETE FROM " + ConnectionForm.DataTable + " WHERE ID = " + DATA_GRID.CurrentCell.Value + ""
                };

                command.ExecuteNonQuery();
                ConnectionForm.con.Close();
                DATA_GRID.Rows.RemoveAt(this.DATA_GRID.SelectedRows[0].Index);

            }
            else
            {
                if (DATA_GRID.Rows.Count == 0)
                {
                    UserInput.id_count = 0;
                }

                if (DATA_GRID.Rows.Count == 1)
                {
                    DATA_GRID.Rows.Clear();
                    UserInput.id_count = 0;
                }
                else
                {

                    var counter = 0;
                    var selectedRow = DATA_GRID.CurrentCell.RowIndex;
                    selectedRow++;

                    var rowCount = DATA_GRID.Rows.Count;
                    var operationValue = rowCount - selectedRow;

                    while (counter < operationValue)
                    {
                        DATA_GRID.Rows[selectedRow].Cells[0].Value = int.Parse(DATA_GRID.Rows[selectedRow].Cells[0].Value.ToString()) - 1;

                        selectedRow++;
                        counter++;
                    }

                    DATA_GRID.Rows.RemoveAt(this.DATA_GRID.SelectedRows[0].Index);
                    var lastindex = DATA_GRID.Rows.Count - 1;

                    UserInput.id_count = int.Parse(DATA_GRID.Rows[lastindex].Cells[0].Value.ToString());
                }
            }
        }
      
        private void OnAddClicked(object sender, EventArgs e)
        {
            DataGridForForms = DATA_GRID;
            _userInput = new UserInput(_connectionForm)
            {
                DataGridView = DATA_GRID
            };

            _userInput.Show();
            
            DATA_GRID.AllowUserToAddRows = false;
        }

        private void OnGithubLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/SebastianDrela2") { UseShellExecute = true });
            
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            ResizeControlChildren();
        }
        
        private void Save()
        {
            UserInput.entire_data.Clear();
            var counter = DATA_GRID.Columns.Count;

            for (int i = 0; i < counter; i++)
            {
                for ( int j = 0; j < DATA_GRID.Rows.Count; j++)
                {
                    UserInput.entire_data.Add(DATA_GRID.Rows[j].Cells[i].Value.ToString());
                }
                
                UserInput.entire_data.Add("//");
            }

            var columns = new List<string>();
            for (int k = 0; k < DATA_GRID.Columns.Count; k++)
            {
                columns.Add(DATA_GRID.Columns[k].HeaderText);
            }
            changed = false;
            if (File.Exists(Path))
            {
                Console.WriteLine(Path);
                File.Create(Path).Close();
                File.WriteAllLines(Path, UserInput.entire_data);
                File.WriteAllLines(Application.StartupPath + "/Columns/" + DatabaseEntry.path_no_txt + "_columns_data.txt", columns);
            }
            
            File.Create(Application.StartupPath + "/Data/entire_data.txt").Close();
            File.WriteAllLines(Application.StartupPath + "/Data/entire_data.txt", UserInput.entire_data);
            File.Create(Application.StartupPath + "/Columns/entire_data_columns_data.txt").Close();
            File.WriteAllLines(Application.StartupPath + "/Columns/entire_data_columns_data.txt" , columns );
        }
            
        private void OnSaveClicked(object sender, EventArgs e)
        {
            SetConnectionToOffline();
            Save();

        }
        
        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (changed)
            {
                DialogResult dialogresult = MessageBox.Show("Unsaved Changes close?", "Exit", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (dialogresult != DialogResult.No)
                {
                    e.Cancel = false;
                }

                if (dialogresult != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }

            var columns = new List<string>();
            for (var i = 0; i < DATA_GRID.Columns.Count; i++)
            {
                columns.Add(DATA_GRID.Columns[i].HeaderText);
            }
        }
        

        private void OnExitClicked(object sender, EventArgs e)
        {
            if (changed)
            {
                DialogResult dialogresult = MessageBox.Show("Unsaved Changes close?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogresult != DialogResult.No)
                {
                    Application.Exit();
                }
            }
            else if (changed == false)
            {
                Application.Exit();
            }
        }
        
        private void OnDataGridCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (DATA_GRID.SelectedCells.Count >= 1 )
            {
                SelectedRowIndex = DATA_GRID.SelectedCells[0].RowIndex;
                SelectedColumnIndex = DATA_GRID.SelectedCells[0].ColumnIndex;
                RowName = DATA_GRID.Rows[SelectedRowIndex].ToString();
                ColumnName = DATA_GRID.Columns[SelectedColumnIndex].HeaderText;
                var selectedRow = DATA_GRID.Rows[SelectedRowIndex];
                CurrentCellValue = selectedRow.Cells[SelectedColumnIndex].Value.ToString();
                IdName = DATA_GRID.Rows[SelectedRowIndex].Cells[0].Value.ToString();

                if (FirstPass == false)
                {
                    FirstPass = true;
                }

                if (LBL_CONNECTION_STATUS.ForeColor == Color.Green && IdName != null && CurrentCellValue != null)
                {
                    
                    Console.WriteLine("ID : " + CurrentCellValue);
                    Console.WriteLine("Data: " + IdName);
                    SqlConnection connection = new SqlConnection(_connectionForm.ConnectionString);
                    connection.Open();
                    
                    SqlCommand command = new SqlCommand
                    {
                        Connection = connection,
                        CommandType = CommandType.Text,
                        CommandText = "UPDATE UserData SET " + ColumnName + " = '" + CurrentCellValue + "' WHERE ID = " + IdName

                    };
                    command.ExecuteNonQuery();
                    

                    Console.WriteLine("UPDATE UserData SET " + ColumnName + " = '" + CurrentCellValue + "' WHERE ID = " + IdName);
                }
            }
        }

        private void OnCellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            changed = true;
        }

        private void OnOpenClicked(object sender, EventArgs e)
        {
            if (Label2.ForeColor != Color.Green)
            {
                Save();
            }

            var openfiledialog = new OpenFileDialog();

            if (openfiledialog.ShowDialog() == DialogResult.OK)
            {
                SetConnectionToOffline();
               
                var setOfLists = new List<List<string>>();

                var filePath = openfiledialog.FileName;
                var fileExt = System.IO.Path.GetExtension(filePath);

                string file_path_no_txt = System.IO.Path.GetFileNameWithoutExtension(filePath);
                Path = filePath;

                string[] limes =
                    File.ReadAllLines(Application.StartupPath + "/Columns/" + file_path_no_txt + "_columns_data.txt");

                DATA_GRID.Columns.Clear();
                foreach (string lime_single in limes)
                {
                    DATA_GRID.Columns.Add(lime_single + "_", lime_single);
                }

                if (fileExt.CompareTo(".txt") == 0)
                {
                    try
                    {
                        int list_index = 0;

                        string[] result = File.ReadAllLines(filePath);

                        var firstRoll = false;
                        var counter = 0;
                        var finalCounter = 0;
                        setOfLists.Add(new List<string>());
                        foreach (var lines in result)
                        {
                            if (lines == "//")
                            {
                                list_index++;
                                setOfLists.Add(new List<string>());
                                if (firstRoll == false)
                                {
                                    finalCounter = counter;
                                    firstRoll = true;
                                }
                            }
                            else if (lines != "//")
                            {
                                setOfLists[list_index].Add(lines);
                                counter++;
                            }
                        }

                        DATA_GRID.Rows.Clear();

                        var columnIndex = 0;
                        for (var j = 0; j < finalCounter; j++)
                        {
                            DATA_GRID.Rows.Add();
                        }

                        foreach (var list in setOfLists)
                        {
                            for (int row_index = 0; row_index < list.Count; row_index++)
                            {
                                DATA_GRID.Rows[row_index].Cells[columnIndex].Value = list[row_index];
                            }

                            columnIndex++;
                        }

                        UserInput.id_count = finalCounter;
                        SetConnectionToOffline();
                    }
                    catch
                    {
                        //
                    }
                }
            }
        }

        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetConnectionToOffline();
            var f3 = new DatabaseEntry();
            f3.Show();
        }

        private void OnSaveAsClicked(object sender, EventArgs e)
        {
            SetConnectionToOffline();
            var saveFiledialog = new SaveFileDialog();

            if (saveFiledialog.ShowDialog() == DialogResult.OK)
            {

                try
                {
                    var filepath = saveFiledialog.FileName + ".txt";

                    var stream = File.Open(filepath, FileMode.Create);
                    var writer = new StreamWriter(stream);

                    var lastRun = false;
                    for (var i = 0; i < DATA_GRID.Columns.Count; i++)
                    {
                        for (var j = 0; j < DATA_GRID.Rows.Count; j++)
                        {
                            var data = (DATA_GRID.Rows[j].Cells[i].Value.ToString());
                            writer.WriteLine(data);

                            if (i == DATA_GRID.Columns.Count - 1)
                            {
                                lastRun = true;
                            }

                        }

                        if(lastRun == false)
                        {
                            writer.WriteLine("//");
                        }
                       
                    }

                    writer.Close();
                }
                catch
                {
                    // ignored
                }
            }
        }
        
        private void searchToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var form4 = new Search_form();
            form4.Show();
        }

        private void OnAddConnectionClicked(object sender, EventArgs e)
        {
            _connectionForm = new ConnectionForm(DATA_GRID);
            _connectionForm.Show();
        }

        private void BTN_SQL_INFO_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Ammount of Columns in SQl server : " + ResultColumn);
        }

        private void columnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
           var frm = new ColumnEditForm(_userInput.DataGridView);
           frm.Show();
        }

        private void OnHelpClicked(object sender, EventArgs e)
        {
            MessageBox.Show(
                @"Rows Can be deleted by pressing DELETE on keyboard and selecting given row File can be saved by pressing Ctrl + S");
        }

       

        private void graphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new GraphForm(DATA_GRID);
            frm.Show();
        }

        private void dataStyleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new StyleEditForm();
            frm.Show();

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ( e.KeyCode == Keys.Delete)
            {
                DeleteRow();
            }
            else if (e is { Control: true, KeyCode: Keys.S })
            { 
                Save();
                SetConnectionToOffline();
            }
        }

        private void sortToolStripMenuItem_Click_2(object sender, EventArgs e)
        {
            var frm = new SortForm();
            frm.Show();
        }
    }
    }
