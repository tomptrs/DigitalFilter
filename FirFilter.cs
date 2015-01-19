using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oulu
{
    public class Model
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Model(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public class FirFilter
    {
        private double a1 = .069418714;
        private double a2 = .181740553;
        private double a3 = .241186237;
        private double a4 = .181740553;
        private double a5 = .069418714;
        private double thresholdValue = .1;
        private double threshold = .0;
        private ObservableCollection<Model> dataIn;
        private ObservableCollection<Model> dataOut;
        public FirFilter()
        {
            a1 = .069418714;
            a2 = .181740553;
            a3 = .241186237;
            a4 = .181740553;
            a5 = .069418714;
            dataOut = new ObservableCollection<Model>();
            
        }

        public FirFilter(double _a1, double _a2, double _a3, double _a4, double _a5)
        {
            a1 = _a1;
            a2 = _a2;
            a3 = _a3;
            a4 = _a4;
            a5 = _a5;
            dataOut = new ObservableCollection<Model>();
        }

        private void IncomingData(ObservableCollection<Model> data)
        {
            dataIn = data;
        }

        private double AverageVerticalAxis()
        {
            double avg = .0;

            for (int i = 0; i < dataOut.Count-1; i++)
            {

                avg += dataOut[i].Y;                
            }
            int totalData = dataOut.Count - 1;
            return avg/totalData;
        }

        private void CalculateThreshold()
        {
            

            double min = dataOut[0].Y;
            double max = dataOut[0].Y;
            for (int i = 1; i < dataOut.Count - 1; i++)
            {
                if (dataOut[i].Y < min)
                    min = dataOut[i].Y;

                if (dataOut[i].Y > max)
                    max = dataOut[i].Y;

            }
            threshold =  threshold - thresholdValue;
        }

        public ObservableCollection<Model> DoFir(ObservableCollection<Model> data)
        {
            IncomingData(data);

            for (int i = 4; i < dataIn.Count - 1; i++)
            {
                double yn = a1 * dataIn[i].Y + a2 * dataIn[i - 1].Y + a3 * dataIn[i - 2].Y + a4 * dataIn[i - 3].Y + a5 * dataIn[i - 4].Y;
                yn *= -1;
                dataOut.Add(new Model(i, yn));
            }

            return dataOut;
        }

        public double CountPullUps(ObservableCollection<Model> data)
        {
            DoFir(data);
            bool stijgend = false;
            bool prevStijgend = true;
            int pullups = 0;
            
            for (int i = 1; i < dataOut.Count - 6; i++)
            {
                if (dataOut[i].Y > dataOut[i - 1].Y)
                {
                    stijgend = true;
                }
                else
                    stijgend = false;

                if (prevStijgend != stijgend)
                {
                    if (stijgend == false && prevStijgend == true)
                        if (dataOut[i].Y > threshold) //TELT ENKEL MEE INDIEN HOGER DAN THRESHOLD
                            pullups++;
                }
                prevStijgend = stijgend;
               
            }

            return pullups;
 
        }

    }
}
