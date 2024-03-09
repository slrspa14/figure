using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using System.Drawing;
//using System.Numerics;
//using System.Windows.Forms;
//using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.IO;


// OpenCV 사용을 위한 using

using System.Threading;
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
        string save;

        private NetworkStream stream;
        private TcpClient client;



        public MainWindow()
        {

            InitializeComponent();


            string bindIp = "10.10.20.113";
            int bindPort = 10000;
            string serverIp = "10.10.20.106";
            int serverPort = 33224;
            string message = "1 / 1번라인";
            string msg;

            try
            {
                IPEndPoint clientAddress = new IPEndPoint(IPAddress.Parse(bindIp), bindPort);
                IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

                client = new TcpClient(clientAddress);
                client.Connect(serverAddress);      // 연결
                MessageBox.Show("연결성공");
                byte[] data = Encoding.Default.GetBytes(message);

                stream = client.GetStream();
                //파일전송해보기,이미지
                string image_protocal = "4/";

                //4/파일
                FileStream image = new FileStream("images.jpg", FileMode.Open);//파일열기

                int len = (int)image.Length;//파일 크기 구하고
                byte[] test_size = Encoding.Default.GetBytes(image_protocal + len); //서버구별하게//byte로 변환
                stream.Write(test_size, 0, test_size.Length);//파일크기 전송하고

                byte[] file_image = new byte[image.Length];//파일담을 배열 파일크기만큼 크기설정
                BinaryReader real_file = new BinaryReader(image); //이미지 파일 읽어오고
                file_image = real_file.ReadBytes(1024); //byte 배열에 다 넣어주고
                stream.Write(file_image, 0, file_image.Length);//파일전송

                //int file_length = (int)image.Length;//크기가져오고
                //image_protocal += file_length.ToString();//서버 구별할 수 있게

                ////byte[] buf = BitConverter.GetBytes(file_length);
                //byte[] buf = Encoding.Default.GetBytes(image_protocal);
                //stream.Write(buf, 0, buf.Length);//파일크기전송

                //byte[] images = new byte[buf.Length];

                //BinaryReader reader = new BinaryReader(image);
                //buf = reader.ReadBytes(4096);
                //stream.Write(buf, 0, buf.Length);     // 메시지 보냄
                MessageBox.Show("파일전송확인");

                byte[] read_data = new byte[256];
                int bytes = stream.Read(read_data, 0, read_data.Length);    // 메세지 받음
                string read = Encoding.Default.GetString(read_data, 0, bytes); // -> 1/
                MessageBox.Show("받아온메시지: " + read);

                //switch (read)
                //{
                //    case "1/":
                //        MessageBox.Show(tor); //tor null 값임
                //        if (tor == "Red_square")
                //        {
                //            MessageBox.Show("1번 if시작");
                //            save = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");// 현재 시간
                //            msg = "2 / " + tor + " / " + save + " / 1번라인 / 박철두 / pass";
                //            byte[] data_1 = Encoding.Default.GetBytes(msg);
                //            stream.Write(data_1, 0, data_1.Length);     // 메세지 보냄
                //            MessageBox.Show("1번 if끝");
                //        }
                //        else
                //        {
                //            MessageBox.Show("1번 else시작");
                //            save = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");// 현재 시간
                //            msg = "2 /" + tor + " /" + save + "/ 1번라인 / 박철두 / fail";
                //            Cam_cpature.Source = WriteableBitmapConverter.ToWriteableBitmap(frame);
                //            Cv2.ImWrite("../../" + save + ".png", frame);
                //            VideoWriter recodetest = new VideoWriter("./" + save + ".avi", FourCC.XVID, 24, frame.Size());
                //            byte[] data_1 = Encoding.Default.GetBytes(msg);
                //            byte[] filename = new byte[4];
                //            stream.Write(data_1, 0, data_1.Length);     // 메세지 보냄
                //            MessageBox.Show("1번 else끝");
                //        }
                //        break;

                //    case "2/":
                //        if (tor == "Yellow_squre")
                //        {
                //            MessageBox.Show("2번 if시작");
                //            save = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");// 현재 시간
                //            msg = "2 / " + tor + " / " + save + " / 1번라인 / 박철두 / pass";
                //            byte[] data_1 = Encoding.Default.GetBytes(msg);
                //            stream.Write(data_1, 0, data_1.Length);     // 메세지 보냄
                //            MessageBox.Show("2번 if끝");
                //        }
                //        else
                //        {
                //            System.Windows.MessageBox.Show("1번 else시작");
                //            save = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");// 현재 시간
                //            msg = "2 /" + tor + " /" + save + "/ 1번라인 / 박철두 / fail";
                //            Cam_cpature.Source = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(frame);
                //            Cv2.ImWrite("../../" + save + ".png", frame);
                //            VideoWriter recodetest = new VideoWriter("./" + save + ".avi", FourCC.XVID, 24, frame.Size());
                //            byte[] data_1 = Encoding.Default.GetBytes(msg);
                //            byte[] filename = new byte[4];
                //            stream.Write(data_1, 0, data_1.Length);     // 메세지 보냄
                //            System.Windows.MessageBox.Show("1번 else끝");
                //        }

                //        break;

                //    case "3/":
                //        if (tor == "Green_squre")
                //        {
                //            MessageBox.Show("3번 if시작");
                //            save = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");// 현재 시간
                //            msg = "2 / " + tor + " / " + save + " / 1번라인 / 박철두 / pass";
                //            byte[] data_1 = Encoding.Default.GetBytes(msg);
                //            stream.Write(data_1, 0, data_1.Length);     // 메세지 보냄
                //            MessageBox.Show("3번 if끝");
                //        }
                //        else
                //        {
                //            MessageBox.Show("3번 else시작");
                //            save = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");// 현재 시간
                //            msg = "2 /" + tor + " /" + save + "/ 1번라인 / 박철두 / fail";
                //            Cam_cpature.Source = WriteableBitmapConverter.ToWriteableBitmap(frame);
                //            Cv2.ImWrite("../../" + save + ".png", frame);
                //            VideoWriter recodetest = new VideoWriter("./" + save + ".avi", FourCC.XVID, 24, frame.Size());
                //            byte[] data_1 = Encoding.Default.GetBytes(msg);
                //            byte[] filename = new byte[4];
                //            stream.Write(data_1, 0, data_1.Length);     // 메세지 보냄
                //            MessageBox.Show("3번 else끝");
                //        }
                //        break;

                //    default:
                //        break;
                //}
                stream.Close();
                client.Close();
            }
            catch (SocketException ex)
            {
                System.Windows.MessageBox.Show("오류");
                Console.WriteLine(ex);
            }

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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            string message = "2 / 네모 빨강 / 2023 - 01 - 23 - 16 - 32 - 12 / 1번라인 / 박철두 / pass";
            byte[] data = System.Text.Encoding.Default.GetBytes(message);

            stream = client.GetStream();

            stream.Write(data, 0, data.Length);
            System.Windows.MessageBox.Show("네모빨강전송");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Cam_cpature.Source = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(frame);//컨트롤에 표시용
            save = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");            // 현재 시간
            Cv2.ImWrite("../../"+ save +".png", frame);
            //VideoWriter recodetest = new VideoWriter("./" + save + ".avi", FourCC.XVID, 24, frame.Size());
        }


        private void timer_tick(object sender, EventArgs e)
        {
            //
            string tor;

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
                    tor = "Yellow_" + shape;
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
                    tor = "Green_" + shape;
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
                    tor = "Red_" + shape;
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
                    tor = "Blue_" + shape;
                    Cv2.PutText(frame, pnt + " Blue " + shape, new Point(cx, cy), HersheyFonts.HersheySimplex, 0.5, Scalar.Blue, 2);
                    blu++;
                }
            }
            Cam.Source = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(frame);

        }
    }
}


