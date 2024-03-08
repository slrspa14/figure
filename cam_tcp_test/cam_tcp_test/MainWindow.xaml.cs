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
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
//tcp
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

// OpenCV 사용을 위한 using


using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Point = OpenCvSharp.Point;
// Timer 사용을 위한 using
using System.Windows.Threading;

namespace WPF
{
    // OpenCvSharp 설치 시 Window를 명시적으로 사용해 주어야 함 (window -> System.Windows.Window)
    public partial class MainWindow : System.Windows.Window
    {
        // 필요한 변수 선언
        VideoCapture cam;
        Mat frame, test;
        DispatcherTimer timer;
        bool is_initCam, is_initTimer;
        
        
        public MainWindow() 
        {
            InitializeComponent();
        }
        private void windows_loaded(object sender, RoutedEventArgs e)
        {
            // 카메라, 타이머(0.01ms 간격) 초기화
            is_initCam = init_camera();
            is_initTimer = init_Timer(0.01);

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


        private string GetShape(Point[] c)
        {
            string shape = "unidentified";
            double peri = Cv2.ArcLength(c, true);
            Point[] approx = Cv2.ApproxPolyDP(c, 0.04 * peri, true);


            if (approx.Length == 3) //if the shape is a triangle, it will have 3 vertices
            {
                shape = "triangle";
            }
            else if (approx.Length == 4)    //if the shape has 4 vertices, it is either a square or a rectangle
            {
                OpenCvSharp.Rect rect;
                rect = Cv2.BoundingRect(approx);
                double ar = rect.Width / (double)rect.Height;

                if (ar >= 0.95 && ar <= 1.05) shape = "square";
                else shape = "square";
            }
            else if (approx.Length == 5)    //if the shape has 5 vertice, it is a pantagon
            {
                shape = "pentagon";
            }
            else   //otherwise, shape is a circle
            {
                shape = "circle";
            }
            return shape;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Cam_cpature.Source = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(frame);
            string save = DateTime.Now.ToString("yyyy-MM-dd-hh시mm분ss초");// 현재 시간
            Cv2.ImWrite("../../" + save + ".png", frame);
            VideoWriter recodetest = new VideoWriter("./" + save + ".avi", FourCC.XVID, 24, frame.Size());
        }
        

        private void timer_tick(object sender, EventArgs e)
        {
            frame = new Mat(); //
            cam.Read(frame); //입력할거

            test = new Mat(); //출력 할거
            Cv2.CvtColor(frame, test, ColorConversionCodes.BGR2HSV);
            //             입력   출력  변환식              

            Mat mask1 = new Mat();
            Cv2.InRange(test, new Scalar(20, 50, 50), new Scalar(30, 255, 255), mask1); //노랑
            Mat mask2 = new Mat();
            Cv2.InRange(test, new Scalar(40, 70, 80), new Scalar(70, 255, 255), mask2); //초록
            Mat mask3 = new Mat();
            Cv2.InRange(test, new Scalar(0, 50, 120), new Scalar(10, 255, 255), mask3); //빨강
            Mat mask4 = new Mat();
            Cv2.InRange(test, new Scalar(90, 60, 0), new Scalar(121, 255, 255), mask4); //파랑

            Cv2.FindContours(mask1, out var contours1, out var hierarchy1, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
            Cv2.FindContours(mask2, out var contours2, out var hierarchy2, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
            Cv2.FindContours(mask3, out var contours3, out var hierarchy3, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
            Cv2.FindContours(mask4, out var contours4, out var hierarchy4, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

            int yel = 0, gre = 0, red = 0, blu = 0;



            foreach (var c in contours1)
            {
                var area = Cv2.ContourArea(c);
                if (area > 8000) //픽셀단위 8000이상일때만
                {
                    Moments m = Cv2.Moments(c);
                    Point pnt = new Point(m.M10 / m.M00, m.M01 / m.M00); //center point
                    Cv2.DrawContours(frame, new[] { c }, -1, Scalar.Yellow, 3); //윤곽선 그리는거 필요없음
                    string shape = GetShape(c); //형상구분함수 밑에 있음 string을 반환함
                    var M = Cv2.Moments(c);
                    var cx = (int)(M.M10 / M.M00);
                    var cy = (int)(M.M01 / M.M00); //이 3놈 중앙 찾음
                    Cv2.PutText(frame, pnt + " Yellow " + shape, new Point(cx, cy), HersheyFonts.HersheySimplex, 0.5, Scalar.Yellow, 2);
                    //화면에 표기 출력할놈 //텍스트           //좌표            //폰트                             글자색
                    yel++;
                }
            }
            foreach (var c in contours2)
            {
                var area = Cv2.ContourArea(c);
                if (area > 8000)
                {
                    Moments m = Cv2.Moments(c);
                    Point pnt = new Point(m.M10 / m.M00, m.M01 / m.M00); //center point
                    Cv2.DrawContours(frame, new[] { c }, -1, Scalar.Green, 3);
                    string shape = GetShape(c); //*형상구분

                    var M = Cv2.Moments(c);
                    var cx = (int)(M.M10 / M.M00);
                    var cy = (int)(M.M01 / M.M00);
                    Cv2.PutText(frame, pnt + " Green " + shape, new Point(cx, cy), HersheyFonts.HersheySimplex, 0.5, Scalar.Green, 2);
                    gre++;
                }
            }
            foreach (var c in contours3)
            {
                var area = Cv2.ContourArea(c);
                if (area > 8000)
                {
                    Moments m = Cv2.Moments(c);
                    Point pnt = new Point(m.M10 / m.M00, m.M01 / m.M00); //center point
                    Cv2.DrawContours(frame, new[] { c }, -1, Scalar.Red, 3);
                    string shape = GetShape(c); //*형상구분
                    var M = Cv2.Moments(c);
                    var cx = (int)(M.M10 / M.M00);
                    var cy = (int)(M.M01 / M.M00);
                    Cv2.PutText(frame, pnt + " Red " + shape, new Point(cx, cy), HersheyFonts.HersheySimplex, 0.5, Scalar.Red, 2);
                    red++;
                }
            }
            foreach (var c in contours4)
            {
                var area = Cv2.ContourArea(c);
                if (area > 8000)
                {

                    Moments m = Cv2.Moments(c);
                    Point pnt = new Point(m.M10 / m.M00, m.M01 / m.M00); //center point

                    Cv2.DrawContours(frame, new[] { c }, -1, Scalar.Blue, 3);
                    string shape = GetShape(c); //*형상구분

                    var M = Cv2.Moments(c);
                    var cx = (int)(M.M10 / M.M00);
                    var cy = (int)(M.M01 / M.M00);

                    Cv2.PutText(frame, pnt + " Blue " + shape, new Point(cx, cy), HersheyFonts.HersheySimplex, 0.5, Scalar.Blue, 2);
                    blu++;
                }

            }

            Cam.Source = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(frame); //첫화면 출력
            string server_ip = "10.10.20.102";
            int server_port = 33224;
            string client_ip = "127.0.0.1";
            int client_port = 10000;
            try
            {
                IPEndPoint serveraddress = new IPEndPoint(IPAddress.Parse(server_ip), server_port);
                IPEndPoint clientaddress = new IPEndPoint(IPAddress.Parse(client_ip), client_port);
                //IPEndPoint tlqkf = new IPEndPoint(IPAddress.Parse(server_ip), server_port);
                //System.Windows.MessageBox.Show($"{clientaddress.ToString()}, {serveraddress.ToString()}");

                TcpClient client = new TcpClient(clientaddress);
                //접속에러
                client.Connect(serveraddress);
                System.Windows.MessageBox.Show("서버접속성공");

                string message = "1/1번라인";
                byte[] test = System.Text.Encoding.Default.GetBytes(message);
                NetworkStream stream = client.GetStream();
                stream.Write(test, 0, test.Length);

                byte[] data = new byte[256];
                string recv_data = "";
                int bytes = stream.Read(data, 0, data.Length);
                recv_data = Encoding.Default.GetString(data, 0, bytes);
                System.Windows.MessageBox.Show(recv_data);
                switch(recv_data)
                {
                    case "1/":
                        {
                            //네모빨강
                            System.Windows.MessageBox.Show("1번");
                            break;
                        }

                    case "2":
                        //노랑원
                        System.Windows.MessageBox.Show("2번");
                        break;
                    case "3":
                        //초록 세모
                        System.Windows.MessageBox.Show("3번");
                        break;
                    case "4":
                        //파랑오각
                        System.Windows.MessageBox.Show("4번");
                        break;
                    case "5":
                        //네모빨강
                        System.Windows.MessageBox.Show("5번");
                        break;
                    case "6":
                        //노랑 원
                        System.Windows.MessageBox.Show("6번");
                        break;
                    case "7":
                        System.Windows.MessageBox.Show("7번");
                        break;
                    default:
                        break;
                        //초록 세모
                }
            }
            catch (SocketException ae)
            {
                Console.WriteLine(ae);
            }
        }
    }
}


