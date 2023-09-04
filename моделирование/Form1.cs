using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kr_work
{
    public partial class Form1 : Form
    {
        UInt16 A, B;                //делимое, делитель
        UInt32 AM, BM;
        UInt32 Do;                   //остаток
        UInt32 C;                   //частное
        Byte Cr;                    //счётчик
        UInt16 a;                   //состояние
        bool[] D, Q;                //множество D и Q сигналов
        bool[] y, x;                //Вектор y и x сигналов

        DataGridView[] dgvArray;
        bool run = true;
        int l = 0;

        public Form1()
        {
            InitializeComponent();
            dgvArray = new[] { dataGridViewA, dataGridViewB, dataGridViewRegAM, dataGridViewRegBM, dataGridViewRegC, dataGridViewRegCr };
            run = false;
            a = 0;
            Q = new bool[4];
            D = new bool[4];
            Q[0] = true;
            Q[1] = false;
            Q[3] = false;
            Q[2] = false;

            y = new bool[15];
            x = new bool[7];

            chkBxOUQ0.Checked = true;
            chkBxOUD1.Checked = true;
            stateChanged();
        }


        // Изменение значения ячейки исходных данных по клику
        private void dtGridSource_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == 0)
            {
                if ((sender as DataGridView)[e.ColumnIndex, e.RowIndex].Value == "0")
                    (sender as DataGridView)[e.ColumnIndex, e.RowIndex].Value = "1";
                else
                    (sender as DataGridView)[e.ColumnIndex, e.RowIndex].Value = "0";
                //помещение в textBox десятичного значения, введенного в dataGridView
                if ((sender as DataGridView).Name == "dataGridViewA")
                {
                   
                    fillTBbyValue(txBxA, getValue(sender as DataGridView));
                    if ((sender as DataGridView)[0, 0].Value == "1" && !txBxA.Text.Equals("0"))
                        txBxA.Text = txBxA.Text.Insert(0, "-");
                }
                else if ((sender as DataGridView).Name == "dataGridViewB")
                {
                    
                    fillTBbyValue(txBxB, getValue(sender as DataGridView));
                    if ((sender as DataGridView)[0, 0].Value == "1" && !txBxB.Text.Equals("0"))
                        txBxB.Text = txBxB.Text.Insert(0, "-");
                }
            }
        }

        // Получение десятичного значения числа, записанного в DataGridView
        private UInt32 getValue(DataGridView dtGridX)
        {
            UInt32 res = 0;
            for (int col = 1; col < dtGridX.ColumnCount; col++)
            {
                res += (UInt32)Math.Pow(2, dtGridX.ColumnCount - col - 1) * UInt32.Parse(dtGridX[col, 0].Value.ToString());
            }
            return res;
        }


        // Запись числа в TextBox
        private void fillTBbyValue(TextBox txBxX, UInt32 valX)
        {
            Double res = 0;
            if (txBxX.Name == "txBxC")
            {
                valX = valX % (UInt32)Math.Pow(2, 16);
                for (Byte pow = 15; pow >= 0 && pow < 16; pow--)
                {
                    res += valX / (UInt32)Math.Pow(2, pow) * Math.Pow(2, pow - 16);
                    valX = valX % (UInt32)Math.Pow(2, pow);
                }
            }
            else
            {
                valX = valX % (UInt32)Math.Pow(2, 15);
                for (Byte pow = 14; pow >= 0 && pow < 15; pow--)
                {
                    res += valX / (UInt32)Math.Pow(2, pow) * Math.Pow(2, pow - 15);
                    valX = valX % (UInt32)Math.Pow(2, pow);
                }
            }

            if (txBxX.Name == "txBxC")
            {
                res = Math.Round(res, 10);
                txBxX.Text = res.ToString("0.##########");
            }
            else
            {
                res = Math.Round(res, 5);
                txBxX.Text = res.ToString("0.#####");
            }

            if (dataGridViewRegC[0, 0].Value.ToString() == "1" && txBxX.Name == "txBxC")
                txBxC.Text = txBxC.Text.Insert(0, "-");
        }

        private void dtGridBindingContextChanged(object sender, EventArgs e)
        {
            for (Byte col = 0; col < (sender as DataGridView).Columns.Count; col++)
            {
                (sender as DataGridView)[col, 0].Value = "0";
            }
        }

        // Отображение в DataGridView значения valX
        private void setDgvs(DataGridView dgvX, UInt32 valX)
        {
            UInt32 saveValX = valX;

            if (dgvX.Name == "dataGridViewRegC")
            {

                for (Int32 col = dgvX.ColumnCount - 1; col > -1; col--)
                {
                    dgvX[col, 0].Value = valX % 2;
                    valX = valX / 2;
                }
                fillTBbyValue(txBxC, saveValX);
            }
            else
            {
                for (Int32 col = dgvX.ColumnCount - 1; col > -1; col--)
                {
                    dgvX[col, 0].Value = valX % 2;
                    valX = valX / 2;
                }
            }
        }

        #region расчет Y
        private void End()
        {
            //завершение
            btnStart.Enabled = false;
            run = false;
            setDgvs(dataGridViewRegC, C);
        }
        private void y1()
        {
            //AM(31:15) := A(14:0)
            UInt32 buff = (UInt32)(A & 0x7fff);
            AM = (buff & 0xFFFFFFFF) << 15;

            /*
            UInt32 bufff = (~AM >> 16) & 0x7FFF; //C(29:15) -> buff(14:0)
            bufff++; //C + 1
            bufff = bufff << 16; //buff(29:15)
            UInt32 d = AM & 0x8000FFFF;
            AM = (AM & 0x8000FFFF) & bufff;
            */
            setDgvs(dataGridViewRegAM, AM);
        }
        private void y2()
        {
            //BM(31:15) := B(14:0)
            UInt32 buff = (UInt32)(B & 0x7fff);
            BM = (buff & 0xFFFFFFFF) << 15;
            setDgvs(dataGridViewRegBM, BM);

            
        }
        private void y3()
        {
            //AM:=AM+11.-BM(29:0)+1
            UInt32 buff = (UInt32)((BM & 0x3FFFFFFF) ^ 0x3FFFFFFF);
            buff = buff | 0xC0000000;
            buff++;
            AM = AM + buff;

            setDgvs(dataGridViewRegAM, AM);
        }
        private void y4()
        {
            //AM := AM + BM(29:0)
            UInt32 buff = BM & 0x3FFFFFFF;
            AM = AM + buff;
            setDgvs(dataGridViewRegAM, AM);
        }
        private void y5()
        {
            //D := AM
            Do = AM;
            setDgvs(dataGridViewRegD, Do);
        }
        private void y6()
        {
            //BM: = R1(0.BM)
            BM = BM >> 1;
            setDgvs(dataGridViewRegBM, BM);
        }
        private void y7()
        {
            //C := 0
            C = 0;
            setDgvs(dataGridViewRegC, C);
        }
        private void y8()
        {
            //CH := 0
            Cr = 0x10;
            setDgvs(dataGridViewRegCr, Cr);
        }
        private void y9()
        {
            //C:= L1(C.1)
            C = C << 1;
            C = C | 0x1;
            setDgvs(dataGridViewRegC, C);
        }
        private void y10()
        {
            //C:= L1(C.0) А НУ 5 БЫСТРА!
            C = C << 1;
            setDgvs(dataGridViewRegC, C);
        }
        private void y11()
        {
            //AM := D
            AM = Do;
            setDgvs(dataGridViewRegAM, AM);
        }
        private void y12()
        {
            //CH := CH - 1
            Cr--;
            setDgvs(dataGridViewRegCr, Cr);
        }
        private void y13()
        {
            //C(16:1) := C(16:1) + 1
            UInt32 buff = (C >> 1) & 0xFFFF;
            buff++;
            buff = buff << 1;
            C = C | buff;
            setDgvs(dataGridViewRegC, C);
        }
        private void y14()
        {
            //C(16) := 1
            C = C | 0x10000;
            setDgvs(dataGridViewRegC, C);
        }
        private void y15()
        {
            //ПП:=1
            chkBxOUD0.Checked = false;
            txBxC.Text = "Переполнение";
            chkBxOUy4.Checked = false;
            btnStart.Enabled = false;
            chkBxOUQ0.Checked = false;
            run = false;
        }
        #endregion


        #region расчет Х
        private Boolean x0()
        {
            //Начало
            return run;
        }
        private Boolean x1()
        {
            //BM = 0
            return BM == 0;
        }
        private Boolean x2()
        {
            //AM = 0
            
            return AM == 0;
        }
        private Boolean x3()
        {
            //AM(31)
            
            return (((AM >> 31) % 2) == 1);
        }
        private Boolean x4()
        {
            //СЧ = 0
            
            return Cr == 0;
        }
        private Boolean x5()
        {
            //C(0)
           
            return (C % 2 == 1);
        }
        private Boolean x6()
        {
            //A(0) ⊕ B(0)
            
            return ((dataGridViewA[15, 0].Value.ToString() == "1" && dataGridViewB[15, 0].Value.ToString() == "0") ||
                    (dataGridViewA[15, 0].Value.ToString() == "0" && dataGridViewB[15, 0].Value.ToString() == "1"));
        }
        #endregion

        // Установка состояний
        private void stateChanged()
        {
            bool[] A_out1 = new bool[11];
            switch (a)
            {
                case 0:
                    A_out1[0] = true;
                    break;
                case 1:
                    A_out1[1] = true;
                    break;
                case 2:
                    A_out1[2] = true;
                    break;
                case 3:
                    A_out1[3] = true;
                    break;
                case 4:
                    A_out1[4] = true;
                    break;
                case 5:
                    A_out1[5] = true;
                    break;
                case 6:
                    A_out1[6] = true;
                    break;
                case 7:
                    A_out1[7] = true;
                    break;
                case 8:
                    A_out1[8] = true;
                    break;
                case 9:
                    A_out1[9] = true;
                    btnStart.Enabled = false;
                    run = false;
                    break;
                default:
                    break;
            }
            chkBxMa0.Checked = false;
            chkBxMa1.Checked = false;
            chkBxMa2.Checked = false;
            chkBxMa3.Checked = false;
            chkBxMa4.Checked = false;
            chkBxMa5.Checked = false;
            chkBxMa6.Checked = false;
            chkBxMa7.Checked = false;
            chkBxMa8.Checked = false;
            chkBxMa9.Checked = false;

            chkBxMa0.Checked = A_out1[0];
            chkBxMa1.Checked = A_out1[1];
            chkBxMa2.Checked = A_out1[2];
            chkBxMa3.Checked = A_out1[3];
            chkBxMa4.Checked = A_out1[4];
            chkBxMa5.Checked = A_out1[5];
            chkBxMa6.Checked = A_out1[6];
            chkBxMa7.Checked = A_out1[7];
            chkBxMa8.Checked = A_out1[8];
            chkBxMa9.Checked = A_out1[9];

            chkBxOUa0.Checked = false;
            chkBxOUa1.Checked = false;
            chkBxOUa2.Checked = false;
            chkBxOUa3.Checked = false;
            chkBxOUa4.Checked = false;
            chkBxOUa5.Checked = false;
            chkBxOUa6.Checked = false;
            chkBxOUa7.Checked = false;
            chkBxOUa8.Checked = false;
            chkBxOUa9.Checked = false;

            chkBxOUa0.Checked = A_out1[0];
            chkBxOUa1.Checked = A_out1[1];
            chkBxOUa2.Checked = A_out1[2];
            chkBxOUa3.Checked = A_out1[3];
            chkBxOUa4.Checked = A_out1[4];
            chkBxOUa5.Checked = A_out1[5];
            chkBxOUa6.Checked = A_out1[6];
            chkBxOUa7.Checked = A_out1[7];
            chkBxOUa8.Checked = A_out1[8];
            chkBxOUa9.Checked = A_out1[9];


        }

        // Микропрограмма
        private Boolean runMP()
        {
            switch (a)
            {
                case 0: //a0
                    if (x0())
                    {
                        y1();
                        y2();
                        a = 1; //a1
                    }
                    else
                    {
                        a = 0; //a0
                    }
                    break;

                case 1: //a1
                    if (x1())
                    {
                        //переполнение
                        y15();
                        a = 9; //a1(к)
                    }
                    else
                    {
                        if (x2())
                        {
                            y7();
                            a = 9; //a1(к)
                        }
                        else
                        {
                            y3();
                            a = 2; //a2
                        }
                    }
                    break;

                case 2: //a2
                    if (x3())
                    {
                        y4();
                        a = 3; //a3
                    }
                    else
                    {
                        y15();
                        a = 9; //a1(к)
                    }
                    break;

                case 3: //a3
                    y5();
                    y6();
                    y7();
                    y8();

                    a = 4;
                    break;

                case 4: //a4
                    y3();

                    a = 5; //a5
                    break;

                case 5: //a5
                    if (x3())
                    {
                        y10();
                        y11();
                        a = 6; //a6
                    }
                    else
                    {
                        y9();
                        a = 6; //a6
                    }
                    break;

                case 6://a6
                    y5();
                    y6();
                    y12();

                    a = 7; //a7
                    break;

                case 7: //a7
                    if (x4())
                    {
                        if (x5())
                        {
                            y13();
                            a = 8;
                        }
                        else
                        {
                            if (x6())
                            {
                                y14();
                            }
                            a = 9;// a1(к)                            
                        }
                    }
                    else
                    {
                        a = 4; //a4
                    }
                    break;

                case 8: //8
                    if (x6())
                    {
                        y14();
                    }
                    a = 9;
                    break;

                case 9: //а1
                    End(); //завершение
                    run = false;
                    a = 1; //a1
                    break;
            }
            stateChanged();
            return x0() && !rdBtnStep.Checked;
        }

        //запуск программы
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (l == 0)
            {
                A = (UInt16)getValue(dataGridViewA);
                B = (UInt16)getValue(dataGridViewB);
                setDgvs(dataGridViewA, A);
                setDgvs(dataGridViewB, B);
            }
            l++;
            run = true;
            if (rdBtnMP.Checked)
            {
                while (runMP())
                {
                }
            }
            else
            {
                
                while (runOAUA())
                {
                }
                if (!rdBtnStep.Checked)
                {
                    DC();
                    CS_D();
                    stateChanged();
                }
                
            }
        }


        //Сброс
        private void btnStop_Click(object sender, EventArgs e)
        {
            run = false;
            a = 0;
            stateChanged();
            A = 0;
            B = 0;
            C = 0;
            l = 0;
            txBxC.Text = "0";
            txBxA.Text = "0";
            txBxB.Text = "0";
            btnStart.Enabled = true;
            for (int i = 0; i < 7; i++)
            {
                x[i] = false;
            };

            //Сброс ячеек регистров
            foreach (DataGridView dtg in dgvArray)
            {
                for (Byte col = 0; col < dtg.Columns.Count; col++)
                {
                    dtg[col, 0].Value = "0";
                }
            }

            Q[0] = true;
            Q[1] = false;
            Q[3] = false;
            Q[2] = false;

            //сброс значений на схеме
            //состояния
            chkBxMa0.Checked = false;
            chkBxMa1.Checked = false;
            chkBxMa2.Checked = false;
            chkBxMa3.Checked = false;
            chkBxMa4.Checked = false;
            chkBxMa5.Checked = false;
            chkBxMa6.Checked = false;
            chkBxMa7.Checked = false;
            chkBxMa8.Checked = false;
            chkBxMa9.Checked = false;

            //Q, D
            chkBxOUD0.Checked = false;
            chkBxOUD1.Checked = true;
            chkBxOUD2.Checked = false;
            chkBxOUD3.Checked = false;

            chkBxOUQ0.Checked = true;
            chkBxOUQ1.Checked = false;
            chkBxOUQ2.Checked = false;
            chkBxOUQ3.Checked = false;

            //Y
            chkBxOUy1.Checked = false;
            chkBxOUy2.Checked = false;
            chkBxOUy3.Checked = false;
            chkBxOUy4.Checked = false;
            chkBxOUy5.Checked = false;
            chkBxOUy6.Checked = false;
            chkBxOUy7.Checked = false;
            chkBxOUy8.Checked = false;
            chkBxOUy9.Checked = false;
            chkBxOUy10.Checked = false;
            chkBxOUy11.Checked = false;
            chkBxOUy12.Checked = false;
            chkBxOUy13.Checked = false;
            chkBxOUy14.Checked = false;
            chkBxOUy15.Checked = false;

            //X
            chkBxOUx1_mem.Checked = true;
            chkBxOUx2_mem.Checked = false;
            chkBxOUx3_mem.Checked = false;
            chkBxOUx4_mem.Checked = false;
            chkBxOUx5_mem.Checked = false;
            chkBxOUx6_mem.Checked = false;
            //chkBxOUx7_mem.Checked = false;

        }

        private void rdBtnAuto_CheckedChanged(object sender, EventArgs e)
        {
            btnStart.Text = "Пуск";
        }

        private void rdBtnStep_CheckedChanged(object sender, EventArgs e)
        {
            btnStart.Text = "Такт";
        }

        // Моделирование взаимодействия УА и ОА
        private Boolean runOAUA()
        {
            //конец выполнения программы
            if (D[0] && D[3])
            {
                End();
                chkBxOUa0.Checked = true;
                chkBxOUx0_mem.Checked = false;
                chkBxOUD0.Checked = false;
            }

            Show_Q_signal(); //установка чекбоксов значений для ПС Q (Память состояний Q)
            CS_Y(); //установка выходного значения КС Y (Комбинационная схема Y)
            OA(); //установка значений для КС Y (Комбинационная схема Y)
            PLU(); //установка значений для памяти логических условий
            DC(); //установка значений для DC (Дешифратор)

            stateChanged(); //изменение чекбоксов для микропрогрпммы
            CS_D();//получение D сигнала от КС D (Комбинационная схема для D сигналов)
            stateMem();//изменение состояний на ПС Q(Память состояний Q)
            Show_D_signal(); //установка чекбоксов для выходных D сигналов
            showX(); //установка чекбоксов ПЛУ (Память логических условий)

            return x0() && !rdBtnStep.Checked;
        }

        private void PLU()
        {
            checkBox_plu_x2.Checked = false;
            checkBox_plu_x3.Checked = false;


            if (a == 1 && !x2())
            {
                checkBox_plu_x2.Checked = true;
            }
            if ((a == 2 && x3()) || (a == 5 && x3()))
            {
                checkBox_plu_x3.Checked = true;
            }
        }

        private void showX()
        {
            chkBxOUx1_mem.Checked = x1();
            chkBxOUx2_mem.Checked = x2();
            chkBxOUx3_mem.Checked = x3();
            chkBxOUx4_mem.Checked = x4();
            chkBxOUx5_mem.Checked = x5();
            chkBxOUx6_mem.Checked = x6();
        }

        private void Show_Q_signal()
        {
            chkBxOUQ0.Checked = (Q[0]);
            chkBxOUQ1.Checked = (Q[1]);
            chkBxOUQ2.Checked = (Q[2]);
            chkBxOUQ3.Checked = (Q[3]);
        }

        // Вывод D 
        private void Show_D_signal()
        {
            if (D[0] && D[3])
            {
                chkBxOUD0.Checked = true;
                chkBxOUD1.Checked = false;
                chkBxOUD2.Checked = false;
                chkBxOUD3.Checked = false;
            }
            else
            {
                chkBxOUD0.Checked = (D[0]);
                chkBxOUD1.Checked = (D[1]);
                chkBxOUD2.Checked = (D[2]);
                chkBxOUD3.Checked = (D[3]);
            }
        }

        // КС Y
        private void CS_Y()
        {
            bool[] Aarr = new bool[10];

            switch (a)
            {
                case 0:
                    Aarr[0] = true;
                    break;
                case 1:
                    Aarr[1] = true;
                    break;
                case 2:
                    Aarr[2] = true;
                    break;
                case 3:
                    Aarr[3] = true;
                    break;
                case 4:
                    Aarr[4] = true;
                    break;
                case 5:
                    Aarr[5] = true;
                    break;
                case 6:
                    Aarr[6] = true;
                    break;
                case 7:
                    Aarr[7] = true;
                    break;
                case 8:
                    Aarr[8] = true;
                    break;
                case 9:
                    Aarr[9] = true;
                    break;
                default:
                    break;
            }

            //y[0] = Aarr[0];
            y[1] = Aarr[0] && x0();
            y[2] = Aarr[0] && x0();
            y[3] = Aarr[1] && !x1() && !x2() || Aarr[4];
            y[4] = Aarr[2] && x3();
            y[5] = Aarr[3] || Aarr[6];
            y[6] = Aarr[3] || Aarr[6];
            y[7] = Aarr[3];
            y[8] = Aarr[3];
            y[9] = Aarr[5] && !x3();
            y[10] = Aarr[5] && x3();
            y[11] = Aarr[5] && x3();
            y[12] = Aarr[6];
            y[13] = Aarr[7] && x4() && x5();
            y[14] = Aarr[8] && x6() || Aarr[7] && x4() && !x5() && x6();
            y[0] = Aarr[9];


            chkBxOUy1.Checked = y[1];
            chkBxOUy2.Checked = y[2];
            chkBxOUy3.Checked = y[3];
            chkBxOUy4.Checked = y[4];
            chkBxOUy5.Checked = y[5];
            chkBxOUy6.Checked = y[6];
            chkBxOUy7.Checked = y[7];
            chkBxOUy8.Checked = y[8];
            chkBxOUy9.Checked = y[9];
            chkBxOUy10.Checked = y[10];
            chkBxOUy11.Checked = y[11];
            chkBxOUy12.Checked = y[12];
            chkBxOUy13.Checked = y[13];
            chkBxOUy14.Checked = y[14];
        }

        //ОА
        private void OA()
        {
            if (y[0]) End();
            if (y[1]) y1();
            if (y[2]) y2();
            if (y[3]) y3();
            if (y[4]) y4();
            if (y[5]) y5();
            if (y[6]) y6();
            if (y[7]) y7();
            if (y[8]) y8();
            if (y[9]) y9();
            if (y[10]) y10();
            if (y[11]) y11();
            if (y[12]) y12();
            if (y[13]) y13();
            if (y[14]) y14();
        }

        private void stateMem()
        {
            Q = D;
        }

        // Дешифратор 
        private void DC()
        {
            a = 0;
            if (Q[0]) a += 1;
            if (Q[1]) a += 2;
            if (Q[2]) a += 4;
            if (Q[3]) a += 8;
        }

        // КС D 
        private void CS_D()
        {
            bool[] Aarr = new bool[10];
            for (byte el = 0; el < Aarr.Length; el++) Aarr[el] = false;

            switch (a)
            {
                case 0:
                    Aarr[0] = true;
                    break;
                case 1:
                    Aarr[1] = true;
                    break;
                case 2:
                    Aarr[2] = true;
                    break;
                case 3:
                    Aarr[3] = true;
                    break;
                case 4:
                    Aarr[4] = true;
                    break;
                case 5:
                    Aarr[5] = true;
                    break;
                case 6:
                    Aarr[6] = true;
                    break;
                case 7:
                    Aarr[7] = true;
                    break;
                case 8:
                    Aarr[8] = true;
                    break;
                case 9:
                    Aarr[9] = true;
                    break;

                default:
                    break;
            }
            D[0] = Aarr[0] && x0() || Aarr[2] && x3() || Aarr[4] || Aarr[6] || Aarr[8];
            D[1] = Aarr[1] && !x1() && !x2() || Aarr[2] && x3() || Aarr[5] || Aarr[6];
            D[2] = Aarr[3] || Aarr[7] && !x4() || Aarr[4] || Aarr[5] || Aarr[6];
            D[3] = Aarr[7] && x4() && x5() || Aarr[8] || Aarr[7] && x4() && !x5() || Aarr[7] && x4() && !x5() ;
        }



        private void rdBtnOU_CheckedChanged(object sender, EventArgs e)
        {
            tabControlLevels.SelectedIndex = 1;
        }
    }
}
