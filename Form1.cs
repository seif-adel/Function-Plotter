using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace micro_task1
{
    public partial class Form1 : Form
    {
        GraphPane gp;
        int no_of_points_plotted = 10;
        int xmin, xmax;
        int range;
        string inp_from_user;
        PointPairList points = new PointPairList();
        int flag= 0;
        bool stack_is_empty = false;
        public Form1()
        {
            InitializeComponent();
            gp = zedGraphControl1.GraphPane;
            zedGraphControl1.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }
        static bool isoperand(char a) 
        {
            return (a=='+' || a=='-' || a=='*' || a=='/');
        }
        /// ////////////////////////////// the following function to make error checking//////////////////
        private void button1_Click(object sender, EventArgs e)
        {
            inp_from_user = text_order.Text;
            //inp_from_user = inp_from_user.Trim(' ');
            inp_from_user = inp_from_user.Replace(" ", string.Empty);
            int left_par = inp_from_user.Split('(').Length - 1;
            int right_par = inp_from_user.Split(')').Length - 1;
            if (left_par != right_par || inp_from_user.IndexOf(')') < inp_from_user.IndexOf('('))
                flag = 5;
            if (inp_from_user[0] == '/' || inp_from_user[0] == '*')
                flag = 2;
            for (int i = 0; i < inp_from_user.Length; i++)
            {
                if (inp_from_user[i] >= 'a' && inp_from_user[i] <= 'z' && inp_from_user[i] != 'x')
                {
                    flag = 3;
                    break;
                }
                if (inp_from_user[i] >= 'A' && inp_from_user[i] <= 'Z' && inp_from_user[i] != 'X')
                {
                    flag = 3;
                    break;
                }
            }
            for (int i = 1; i < inp_from_user.Length; i++) 
            {
                if (inp_from_user[i] == '-' && inp_from_user[i - 1] == '^')
                {   
                    flag = 1;
                    break;
                }
                if(inp_from_user[i]=='0' && inp_from_user[i - 1] == '/') 
                {
                    flag = 4;
                    break;
                }
                if((inp_from_user[i]=='/' && inp_from_user[i-1]=='(')|| (inp_from_user[i] == '*' && inp_from_user[i - 1] == '('))
                {
                    flag = 2;
                    break;
                }
                if (isoperand(inp_from_user[i]) && isoperand(inp_from_user[i - 1]))
                {
                    flag = 2;
                    break;
                }

            }
            if (flag == 0)
            {
                label2.Visible = true;
                label4.Visible = true;
                label5.Visible = true;
                text_xmax.Visible = true;
                text_xmin.Visible = true;
                button2.Visible = true;
            }
            else if(flag==1)
            {
                label6.Visible = true;
            }
            else if (flag == 2) 
            {
                label_error_neg.Visible = true;
            }
            else if (flag == 3) 
            {
                label7.Visible= true;
            }
            else if (flag == 4)
            {
                label8.Visible = true;
            }
            else if (flag == 5)
            {
                label9.Visible = true;
            }
        }      
        ////////////////////////////////// end of error checking ////////////////////////////////////////////////
        private void button2_Click(object sender, EventArgs e)
        {
            xmax = Convert.ToInt32(text_xmax.Text);
            xmin = Convert.ToInt32(text_xmin.Text);
            range = xmax - xmin;
            button4.Visible = true;
            label2.Visible = false;
            text_xmax.Visible = false;
            text_xmin.Visible = false;
            button2.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
        }
        /// <summary>
        ////////////////////////////////////////////////////
        
        ////////////////// function to return the precedence of operator///////////////
        static int precedence(char op)
        {
            if (op == '+' || op == '-')
                return 1;
            if (op == '*' || op == '/')
                return 2;
            if (op == '^')
                return 3;
            return 0;
        }

        ////////////////// function to perform arithmetic operations/////////////////
        static double applyOp(double a, double b, char op)
        {
            switch (op)
            {
                case '+': return a + b;
                case '-': return a - b;
                case '*': return a * b;
                case '/': return a / b;
                case '^': return Math.Pow(a, b);
            }
            return -1;
        }
        static bool isdigit(char a) 
        {
            return a >= '0' && a <= '9';
        }
        

/// //////////////////////////////// the following function to evaluate the whole equation for specific value for x called numb//////
        static double evaluate(string tokens, double cuurent_val_of_x)
        {
            int neg_flag = 0; // flag to set by 1 if the current negative token is neg sign to number not arithmatic op.
            if (tokens[0] == '-') 
            {
                neg_flag = 1;
                tokens = tokens.Substring(1);
            }
            if (tokens[0] == '+')
                tokens = tokens.Substring(1);
  
            int i;

            // stack to store integer values.
            Stack<double> values= new Stack<double>();

            // stack to store operators.
            Stack<char> ops = new Stack<char>();

            for (i = 0; i < tokens.Length; i++)
            {
                // Current token is a whitespace,
                // skip it.
                if (tokens[i] == ' ') // block of empty token
                    continue;

                // Current token is an opening
                // brace, push it to 'ops'
                else if (tokens[i] == '(') // block of '(' token 
                {
                    ops.Push(tokens[i]);
                }

                // Current token is a number, push
                // it to stack for numbers.
                else if (isdigit(tokens[i]) || tokens[i]=='x' || tokens[i] == 'X') // block of digit or x token 
                    // as we substitute each x by it's current value
                {
                    int val = 0;

                    // There may be more than one
                    // digits in number.
                    while (i < tokens.Length &&
                                isdigit(tokens[i]) && tokens[i] !='x' && tokens[i] != 'X')
                    {
                        val = (val * 10) + (tokens[i] - '0');
                        i++;
                    }

                    if (i >= tokens.Length)
                        i--;

                    if (tokens[i] != 'x' && tokens[i] != 'X') // check if the current token is digit not x
                    {
                        if (neg_flag == 1)
                        {
                            values.Push(-val);
                            neg_flag = 0;
                        }
                        else
                        {
                            values.Push(val);
                        }
                        // right now the i points to
                        // the character next to the digit,
                        // since the for loop also increases
                        // the i, we would skip one
                        //  token position; we need to
                        // decrease the value of i by 1 to
                        // correct the offset.
                        if(i< tokens.Length-1)
                        i--;
                    }
                    else // current token is x not token
                    {
                        if (neg_flag == 1)
                        {
                            values.Push(-cuurent_val_of_x);
                            neg_flag = 0;
                        }
                        else
                        {
                            values.Push(cuurent_val_of_x);
                        }
                    }
                }
                // Closing brace encountered, solve
                // entire brace.
                else if (tokens[i] == ')') // block of ')' token
                {
                    while (!(ops.Count==0) && ops.Peek() != '(')
                    {
                        double val2 = values.Peek();
                        values.Pop();

                        double val1 = values.Peek();
                        values.Pop();

                        char op = ops.Peek();
                        ops.Pop();

                        values.Push(applyOp(val1, val2, op));
                    }

                    // pop opening brace.
                    if (!(ops.Count == 0))
                        ops.Pop();
                }

                else // block of operators ( + ,- ,* ,/ ,^ )
                {
                    if(i>0 && tokens[i - 1] == '(' && tokens[i] == '-')  // if current token is - and the prev 
                        // is ( so you must set the neg flag
                    {
                        neg_flag = 1;
                        continue;                       
                    }
                    // While top of 'ops' has same or greater
                    // precedence to current token, which
                    // is an operator. Apply operator on top
                    // of 'ops' to top two elements in values stack.
                    while (!(ops.Count == 0) && precedence(ops.Peek())
                                        >= precedence(tokens[i]))
                    {
                        double val2 = values.Peek();
                        values.Pop();

                        double val1 = values.Peek();
                        values.Pop();

                        char op = ops.Peek();
                        ops.Pop();

                        values.Push(applyOp(val1, val2, op));
                    }

                    // Push current token to 'ops'.
                    ops.Push(tokens[i]);
                }
            }

            // Entire expression has been parsed at this
            // point, apply remaining ops to remaining
            // values.
            while (!(ops.Count == 0))
            {
                    
                double val2 = values.Peek();
                values.Pop();

                double val1 = values.Peek();
                values.Pop();

                char op = ops.Peek();
                ops.Pop();

                values.Push(applyOp(val1, val2, op));
            }

            // Top of 'values' contains result, return it.
            return values.Peek();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            zedGraphControl1.Visible = true;
            generate_points();
            LineItem li = gp.AddCurve("the function", points, Color.Red ,SymbolType.Circle);
            zedGraphControl1.AxisChange();
        }

        private void text_order_TextChanged(object sender, EventArgs e)
        {

        }

        private void generate_points()
        {
            // loop to generate points by iterate on some number of x's determined by user
            for (double x = xmin; x < xmax; x += (double)((double)range / (double)no_of_points_plotted))
            {
                PointPair p = new PointPair(x, evaluate(inp_from_user,x));
                points.Add(p);               
            }
        }
    }
}
