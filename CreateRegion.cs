using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_PieHotKey
{
    public class CreateRegion
    {
        public Region[] RegionsList { get; private set; }
        public CreateRegion(int totalN, int[] InOutD)
        {
            Point CenterPt = new Point
            {
                X = InOutD[1] / 2 + 5,
                Y = InOutD[1] / 2 + 5
            };
            float Degree = 360F / totalN -2;
            float DegreeForTrans = 360F / totalN;
            double MathDegree1 = Math.PI / 180 * (Degree / 2);
            double MathDegree = Math.PI / 180 * (Degree);
            Rectangle rectIn = new Rectangle(CenterPt.X - InOutD[0] / 2, CenterPt.Y - InOutD[0] / 2, InOutD[0], InOutD[0]);
            Rectangle rectOut = new Rectangle(CenterPt.X - InOutD[1] / 2, CenterPt.Y - InOutD[1] / 2, InOutD[1], InOutD[1]);
            GraphicsPath path = new GraphicsPath();//建立曲線

            #region 繪製線條
            //建立直線 點位置
            PointF a1 = new PointF //外弧線上的起始點
            {
                X = Convert.ToSingle(InOutD[1] / 2 * Math.Cos(MathDegree1)) + CenterPt.X,
                Y = -Convert.ToSingle(InOutD[1] / 2 * Math.Sin(MathDegree1)) + CenterPt.Y//因為電腦Y座標向下
            };
            PointF a2 = new PointF //內弧線上的起始點
            {
                X = Convert.ToSingle(InOutD[0] / 2 * Math.Cos(MathDegree1)) + CenterPt.X,
                Y = -Convert.ToSingle(InOutD[0] / 2 * Math.Sin(MathDegree1)) + CenterPt.Y
            };
            PointF b1 = new PointF //外弧線上的終點
            {
                X = Convert.ToSingle(InOutD[1] / 2 * Math.Cos(MathDegree1 - MathDegree)) + CenterPt.X,
                Y = -Convert.ToSingle(InOutD[1] / 2 * Math.Sin(MathDegree1 - MathDegree)) + CenterPt.Y
            };
            PointF b2 = new PointF  //內弧線上的終點
            {
                X = Convert.ToSingle(InOutD[0] / 2 * Math.Cos(MathDegree1 - MathDegree)) + CenterPt.X,
                Y = -Convert.ToSingle(InOutD[0] / 2 * Math.Sin(MathDegree1 - MathDegree)) + CenterPt.Y
            };
            path.StartFigure();//開始繪製
            path.AddArc(rectOut, -Degree / 2, Degree);//外弧線
            path.AddLine(b1, b2);
            path.AddArc(rectIn, Degree / 2, -Degree);//內弧線
            path.AddLine(a2, a1);
            #endregion

            Matrix Mymatrix = new Matrix();//旋轉圖形
            Mymatrix.RotateAt(DegreeForTrans, new PointF(InOutD[1] / 2 + 5, InOutD[1] / 2 + 5));
            Region[] R = new Region[totalN];//建立要回傳的Region
            for (int i = 0; i < totalN; i++)
            {
                if (i == 0) //若為第一個則不旋轉 其餘則一直旋轉
                {
                    R[i] = new Region();
                    R[i] = new Region(path);
                }
                else
                {
                    path.Transform(Mymatrix);
                    R[i] = new Region();
                    R[i] = new Region(path);
                }
            }

            RegionsList = R;
        }

    }
}
