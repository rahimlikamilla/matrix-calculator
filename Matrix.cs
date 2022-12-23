using System;

class Matrix {
    public double[,] m;


    public Matrix (double[,] arr) {
        m = arr;
    }


    public (int, int) dim => (m.GetLength(0), m.GetLength(1));


    public double this[int i, int j] {
        get { return m[i, j]; }
        set { m[i, j] = value; }
    }

    #region Simple Operations
    public static Matrix operator + (Matrix a, Matrix b) {
        (int rows, int cols) = a.dim;
        double[,] res = new double[rows, cols];
        
        for (int r = 0; r < rows; ++r)
            for (int c = 0; c < cols; ++c)
                res[r, c] += a[r, c] + b[r, c];
                
        return new Matrix(res);
    }


    public static Matrix operator - (Matrix a, Matrix b) {
        (int rows, int cols) = a.dim;
        double[,] res = new double[rows, cols];
        
        for (int r = 0; r < rows; ++r)
            for (int c = 0; c < cols; ++c)
                res[r, c] += a[r, c] - b[r, c];
                
        return new Matrix(res);
    }


    public static Matrix operator * (Matrix a, Matrix b) {
        (int rows, int cols) = (a.dim.Item1, b.dim.Item2);
        double[,] res = new double[rows, cols];

        for (int r = 0; r < rows; ++r)
            for (int c = 0; c < cols; ++c)
                for (int k = 0; k < a.dim.Item2; ++k)
                    res[r, c] += a[r, k] * b[k, c];

        return new Matrix(res);
    }
    #endregion


    #region Helper Method
    bool isRegular() {
        double d = 1;
        int rows = dim.Item1;
        for (int r = 0; r < rows; ++r) {
            for (int c = 0; c < rows; ++c)
                d *= m[r, r];
        }
        return d != 0;
    }

    
    public double[,] formAugmentedM(double[,] a) {
        int d = dim.Item1;
        double[,] agmt = new double[d, 2 * d];
        int pivot = d;
        for (int r = 0; r < d; ++r) {
            for (int c = 0; c < d * 2; ++c)
                if (c < d)
                    agmt[r, c] += a[r, c];
                else if (c == pivot)
                    agmt[r, c] += 1; 
                else 
                    agmt[r, c] += 0;

            pivot += 1;
        }
        return agmt;
    }


    // Swapping rows
    void swap(ref double[,] a, int rowa, int rowb) {
        int cols = a.GetLength(1);
        
        for (int c = 0; c < cols; ++c) {
            double temp = a[rowa, c];
            a[rowa, c] = a[rowb, c];
            a[rowb, c] = temp;
        }
    }


    // Swap rows, so that largest leftmost non-zero entry is on top
    int nonzeroOnTop(ref double[,] a, int pivot = 0, int col = 0) {
        int exchange_with = pivot;
        (int rows, int cols) = dim;

        double biggest = double.MinValue;
        for (int r = pivot; r < rows; ++r) {
            if (a[r, col] != 0) {
                if (a[r, col] > biggest) {
                    biggest = a[r, col];
                    exchange_with = r;
                }
            }
        }
        if (biggest == 0 || biggest == double.MinValue)
            return -1;

        swap(ref a, pivot, exchange_with);
        return pivot; 
    }


    // Finding RREF
    void findRREF(ref double[,] a, bool inverse = false) {
        int rows = a.GetLength(0);
        int cols = a.GetLength(1);
        int all_columns = cols;
        int pivot_row = -1;

        if (inverse) {
            all_columns = rows;
        }

        for (int c = 0; c < all_columns; ++c) {
            int result = nonzeroOnTop(ref a, pivot_row + 1, c);

            if (result == -1)
                continue;

            pivot_row = result;
            double scalar = a[pivot_row, c];

            for (int j = 0; j < cols; ++j)
                a[pivot_row, j] /= scalar;

            for (int r = 0; r < rows; ++r) {
                if (a[r, c] != 0 && r != pivot_row) {
                    scalar = a[r, c];
                    for (int col = 0; col < cols; ++col) {
                        a[r, col] = (a[r, col] - (scalar * a[pivot_row, col]));
                    }
                }
            }
        }
    }
    #endregion


    #region Advanced Operations
    #nullable enable
    public Matrix? Inverse() {
        int rows = dim.Item1;
        double[,] res = new double[rows, rows];
        double[,] au = formAugmentedM(m);
        findRREF(ref au, true); 
        for (int r = 0; r < rows; ++r) {
            for (int c = rows; c < rows * 2; ++c ){
                res[r, c - rows] = au[r, c];
            }
        }

        Matrix invrs = new Matrix(res);
        if (!isRegular())
            return null;
        return invrs;
    }
    #nullable disable


    public Matrix SolveSystems() {
        findRREF(ref m);
        return new Matrix(m);
    }
    #endregion


    // Return matrix in string form
    public string toString() {
        string res = "";
        (int rows, int cols) = dim;
        for (int r = 0; r < rows; ++r) {
            string line = "";
            for (int c = 0; c < cols; ++c) {
                line += $"{m[r, c]:f1} ";
            }
            line = line.Trim();
            res += line + "\n" + "\n";
        }
        return res;
    }
}
