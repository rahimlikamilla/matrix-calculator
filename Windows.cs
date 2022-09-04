using System;
using System.Drawing;
using System.Windows.Forms;
using static Operation;


enum Operation { 
    Addition,
    Subtraction,
    Multiplication,
    Inverse,        // finding an inverse
    Systems     // solving linear systems
}

class MyForm : Form {
    TextBox matrix_a = new TextBox();
    TextBox matrix_b = new TextBox();
    Button calculate_button;
    bool error_message = false;
    string exit_message;

    Operation? current_operation = null;
    Color uni_text_color = Color.White;
    string uni_font = "Arial Rounded MT";
    int y_coordinate = 180;
    int padding = 25;
    int matrix_width = 200;

    string output = "";

    public MyForm() {
        ClientSize = new Size(1200, 500);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Matrix Calculator";
        BackColor = Color.FromArgb(36, 52, 71);
        matrix_a = new TextBox();
        matrix_b = new TextBox();

        // Menu
        ToolStripMenuItem[] list = {
            new ToolStripMenuItem("Addition", null, OnAddition),
            new ToolStripMenuItem("Subtraction", null, OnSubtraction),
            new ToolStripMenuItem("Product", null, OnMultiplication),
            new ToolStripMenuItem("Find An Inverse", null, OnInverse),
            new ToolStripMenuItem("Solve linear system", null, OnSystems)
        };

        ToolStripMenuItem[] operations = {
            new ToolStripMenuItem("Operations", null, list)
        };

        MenuStrip strip = new MenuStrip();
        operations[0].ForeColor = uni_text_color;
        strip.Padding = new Padding(5, 5, 5, 5);
        strip.Font = new Font(uni_font, 15);
        strip.Height = 30;
        strip.BackColor = Color.FromArgb(101, 119, 134);
        
        foreach (var item in operations)
            strip.Items.Add(item);

        Controls.Add(strip);
    }


    #region Error/Output/Help message

    // Print error message if input is incorrect
    public void ErrorMessage(Graphics g) {
        g.DrawString(exit_message, new Font(uni_font, 20), 
        new SolidBrush(Color.Red), new PointF(padding, 400));
    }
    

    public void OutputMessage(Graphics g) {
        g.DrawString(output, new Font(uni_font, 15), 
        new SolidBrush(Color.White), 
        new PointF( matrix_width * 4, y_coordinate + padding) );
        output = "";
    }


    void DrawHelp(Graphics g) {
        string text = 
        "1. Choose one of the operations.\n2. Click \"calculate\".";
        text += "\nOutput is rounded to 1 digit after the decimal point.";
        text += $"\nOperation = {current_operation}";
        g.DrawString(text, new Font(uni_font, 20), 
        new SolidBrush(Color.White), new PointF(padding, 50));
    }
    #endregion


    #region Processing the input

    // parsing text of textbox to the matrix
    Matrix ParseInputToMatrix(string s) {
        s = s.Trim();
        string[] lines = s.Split('\n');
        string[] tmp = lines[0].Split(null);
        
        int rows = lines.Length;
        int cols = 0;
        for (int i = 0; i < tmp.Length; ++i) {
            if (tmp[i] != " " && tmp[i] != "")
                cols += 1;
        }

        double[,] res = new double[rows, cols];
        int ind = 0;
        for (int i = 0; i < rows; ++i) {
            string[] entries = lines[i].Split();

            for (int j = 0; j < entries.Length; ++j) {
                if (entries[j] != " " && entries[j] != "") {
                    res[i, ind] = Convert.ToDouble(entries[j]);
                    ind += 1;
                }
            }
            ind = 0;
        }

        return new Matrix(res);
    }

    
    bool CheckInput(string s) {
        if (s == "") { 
            exit_message = "Text box is empty!";
            return false;
        }
        s = s.Trim();

        string[] lines = s.Split('\n');
        for (int i = 0; i < lines.Length; ++i) {
            string[] entries = lines[i].Split();
            foreach(string e in entries) {
                if (e == " " || e == "," || e == "")
                    continue;
                try {
                    Convert.ToDouble(e);
                }
                catch {
                    exit_message = "Use only numbers!";
                    return false;
                }    
            }
        }
        
        int cols = 0;
        string[] tmp = lines[0].Split(null);

        for (int i = 0; i < tmp.Length; ++i)
            if (tmp[i] != " " && tmp[i] != "")
                cols += 1;

        for (int i = 0; i < lines.Length; ++i) {
            string[] temp = lines[i].Split(null);
            int test_col = 0;
            for (int j = 0; j < temp.Length; ++j)
                if (temp[j] != " " && temp[j] != "")
                    test_col += 1;

            if (test_col != cols) {
                exit_message = "The number of columns doesn't match!";
                return false;
            }
        }

        return true;
    }


    bool CheckDimensions(params (int rows, int cols)[] d) {
        exit_message = "The entered dimensions are incorrect!";
        for (int i = 0; i < d.Length; ++i)
            if (d[i].rows == 0 || d[i].cols == 0)
                return false;
        if (current_operation == Addition || current_operation == Subtraction) {
            if (!(d[0].rows == d[1].rows && d[0].cols == d[1].cols))
                return false;
        }
        else if (current_operation == Multiplication) {
            if (d[0].cols != d[1].rows)
                return false;
        }
        else if (current_operation == Inverse) {
            if (d[0].rows != d[0].cols)
                return false;
        }
        exit_message = "";
        return true;
    }
    #endregion


    #region Button related methods

    public void calculate(object Sender, EventArgs e) {
        error_message = true;
        string texta = matrix_a.Text;
        string textb = matrix_b.Text;
        
        if (current_operation == Addition) {
            if (CheckInput(texta) && CheckInput(textb)) {
                Matrix a = ParseInputToMatrix(texta);
                Matrix b = ParseInputToMatrix(textb);
                if (CheckDimensions(a.dim, b.dim)) {
                    Matrix c = a + b;
                    error_message = false;
                    output = c.toString();
                }
            }
        }
        else if (current_operation == Subtraction) {
            if (CheckInput(texta) && CheckInput(textb)) {
                Matrix a = ParseInputToMatrix(texta);
                Matrix b = ParseInputToMatrix(textb);
                if (CheckDimensions(a.dim, b.dim)){
                    Matrix c = a - b;
                    error_message = false;
                    output = c.toString();
                }   
            }
        }
        else if (current_operation == Multiplication) {
            if (CheckInput(texta) && CheckInput(textb)) {
                Matrix a = ParseInputToMatrix(texta);
                Matrix b = ParseInputToMatrix(textb);
                if (CheckDimensions(a.dim, b.dim)){
                    Matrix c = a * b;
                    error_message = false;
                    output = c.toString();
                }   
            }
        }
        else if (current_operation == Inverse) {
            #nullable enable
            if (CheckInput(texta)) {
                Matrix a = ParseInputToMatrix(texta);
                if (CheckDimensions(a.dim)) {
                    Matrix? i = a.Inverse();
                    error_message = false;
                    exit_message = "The entered matrix is not regular!";
                    if (i == null)
                        error_message = true;
                    else
                        output = i.toString();
                }
            }
            #nullable disable
        }
        else { // Solving Systems
            if (CheckInput(texta)) {
                Matrix a = ParseInputToMatrix(texta);
                if (CheckDimensions(a.dim)) {
                    Matrix s = a.SolveSystems();
                    error_message = false;
                    output = s.toString();
                }
            }
        }

        Invalidate();
    }


    void BuildAButton(int x = 0) {
        calculate_button = new Button();
        calculate_button.Text = "Calculate";
        calculate_button.Name = "Calculate";
        calculate_button.ForeColor = Color.White;
        calculate_button.Click += new EventHandler(calculate);
        calculate_button.Location = new Point(
        350 + x + padding, 
        280 - calculate_button.Height);
        calculate_button.FlatStyle = FlatStyle.Flat;
        Controls.Add(calculate_button);
    }
    #endregion


    #region Removing Old Controls

    // removing old textboxes/buttons left from previous operation
    void RemoveOldControls() {
        matrix_a.Text = "";
        matrix_b.Text = "";
        for (int i = Controls.Count - 1; i >= 0; --i ) {
            Control c = Controls[i];
            if (!(c is MenuStrip)){
                Controls.RemoveAt(i);
            }
        }
    }

    void RemoveEverything(Operation op, int m, int x_coord = 0) {
        output = "";
        exit_message = "";
        current_operation = op;
        RemoveOldControls();
        DrawMatrices(m);
        BuildAButton(x_coord);
        Invalidate();
    }
    #endregion

    
    #region Operations
    void OnAddition(object Sender, EventArgs e) {
        RemoveEverything(Addition, 2, matrix_width);
    }


    void OnSubtraction(object Sender, EventArgs e) {
        RemoveEverything(Subtraction, 2, matrix_width);
    }


    void OnMultiplication(object Sender, EventArgs e) {
        RemoveEverything(Multiplication, 2, matrix_width);
    }


    void OnInverse(object Sender, EventArgs e) {
        RemoveEverything(Inverse, 1);
    }

    
    void OnSystems(object Sender, EventArgs e) {
        RemoveEverything(Systems, 1);
    }
    #endregion


    #region Drawing Objects
    void DrawMatrices(int n) { // number of matrices (1 or 2)
        TextBox[] matrices = {matrix_a, matrix_b};
        for (int i = 0; i < n; ++i) {
            matrices[i].Location = new Point(
            padding + (i * 300), y_coordinate
            );
            matrices[i].Multiline = true;
            matrices[i].Height = 200;
            matrices[i].Width = matrix_width;
            Controls.Add(matrices[i]);
        }
    }


    protected override void OnPaint(PaintEventArgs e) {
        Graphics g = e.Graphics;
        DrawHelp(g);
        if (error_message) { ErrorMessage(g); }
        OutputMessage(g);
    }
    #endregion
}