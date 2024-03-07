using System;
using System.Collections.Generic;
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

using Point = OpenCvSharp.Point;

// OpenCV 사용을 위한 using
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

// Timer 사용을 위한 using
using System.Windows.Threading;

namespace WPF
{
    // OpenCvSharp 설치 시 Window를 명시적으로 사용해 주어야 함 (window -> System.Windows.Window)
    public partial class MainWindow : System.Windows.Window
    {
        // 필요한 변수 선언
        VideoCapture cam;
        Mat frame;
        DispatcherTimer timer;
        VideoWriter recodetest;
        bool is_initCam, is_initTimer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void windows_loaded(object sender, RoutedEventArgs e)
        {
            // 카메라, 타이머(0.01ms 간격) 초기화
            is_initCam = init_camera();
            is_initTimer = init_Timer(1);

            // 초기화 완료면 타이머 실행
            if (is_initTimer && is_initCam) timer.Start();
        }

        private bool init_Timer(double interval_ms)
        {
            try
            {
                timer = new DispatcherTimer();

                timer.Interval = TimeSpan.FromMilliseconds(interval_ms);
                timer.Tick += new EventHandler(timer_tick);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool init_camera()
        {
            try
            {
                // 0번 카메라로 VideoCapture 생성 (카메라가 없으면 안됨)
                cam = new VideoCapture(0);
                cam.FrameHeight = (int)Cam.Height;
                cam.FrameWidth = (int)Cam.Width;

                // 카메라 영상을 담을 Mat 변수 생성
                frame = new Mat();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //누르면 사진이 찍히게
            //lbl.Content = "test";
            cam.Read(frame);
            cam_test.Source = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(frame);
            recodetest = new VideoWriter("./recode.avi", FourCC.XVID, 15, frame.Size());//영상캡처용
            //Cv2.ImWrite("./Capture.png", frame);
            //string save = DateTime.Now.ToString("yyyy-MM-dd-hh시mm분ss초");
            //Cv2.ImWrite("./" + save + ".png", frame);
            lbl.Content = "캡처";
            //recodetest.Write(frame);//오류
        }

        private void timer_tick(object sender, EventArgs e)
        {
            //// 0번 장비로 생성된 VideoCapture 객체에서 frame을 읽어옴
            //cam.Read(frame);
            //// 읽어온 Mat 데이터를 Bitmap 데이터로 변경 후 컨트롤에 그려줌
            //Cam.Source = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(frame);
            frame = new Mat(); //
            cam.Read(frame); //입력할거

            Mat test = new Mat(); //출력 할거
            Cv2.CvtColor(frame, test, ColorConversionCodes.BGR2HSV);
            //             입력   출력  변환식              

            Mat color = new Mat();

            Cv2.InRange(test, new Scalar(0, 100, 100), new Scalar(360 / 2, 255, 255), color);//색깔 범위

            Mat result = new Mat();
            Cv2.BitwiseAnd(frame, frame, result, color);


            //Mat yellow = new Mat();
            Mat dst = result.Clone();

            Point[][] contours;
            HierarchyIndex[] hierarchy;


            Cv2.FindContours(color, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxTC89KCOS);

            List<Point[]> new_contours = new List<Point[]>();
            foreach (Point[] p in contours)
            {
                double length = Cv2.ArcLength(p, true);
                if (length > 100)
                {
                    new_contours.Add(p);
                }
            }

            Cv2.DrawContours(dst, new_contours, -1, new Scalar(255, 0, 0), 2, LineTypes.AntiAlias, null, 1);

            // 읽어온 Mat 데이터를 Bitmap 데이터로 변경 후 컨트롤에 그려줌
            Cam.Source = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(dst);

        }
    }
}