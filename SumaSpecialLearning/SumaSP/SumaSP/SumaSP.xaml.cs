//-------------------------------------------------------------------------------------
// <copyright file="squat(ROM)window.xaml.cs" company="Special Learning">
//     Copyright (c) Special Learning.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------

#region libraries references

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.Globalization;


// Coding4Fun Kinect Libraries (http://c4fkinect.codeplex.com/), 
//Libraries for Hoverbuttons, ScalePosition
using Coding4Fun.Kinect.Wpf;
using Coding4Fun.Kinect.Wpf.Controls;
using System.Media;
#endregion

#region Special Learning Ejercicio Suma

namespace SumaSP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor _sensor;
        
        
        // SpecialLearning Timer
        private readonly System.Windows.Threading.DispatcherTimer _timer;

        // (Int) TimerCounter, contains the increment of the timer tick
        public int TimerCounter;

        // (Int) TimeSelection, contains user time selection
        public int TimeSelection = 4;

        // (Int) Second, Timer seconds
        public int Second;

        // (Int) Minute, Timer minutes
        public int Minute;

        // (bool)BoolStartTimer, Enable/Disable Timer
        public bool BoolStartTimer;

        // (bool)BoolStartSquat, Enable/Disable squat detection
        public bool BoolStartSquat;

        // (bool)Repetition, Enable/Disable squat detection when user Performs a good squat
        public bool Repetitions;

        // (double) FinalAngle, Squat angle value
        public double FinalAngle;
        
        // (int)RepetitionCounter, number of repetitions chosen, default value (10)
        public int RepetitionCounter = 10; 

        public int RepetitionCounterCopy;

        public int RepsPerformed;

        public bool HideCursor = false;

        // max value returned
        const float MaxDepthDistance = 4095;

        // min value returned
        const float MinDepthDistance = 850;

        //Depth Value parameters
        const float MaxDepthDistanceOffset = MaxDepthDistance - MinDepthDistance;

        //(int) HigherAngle, squat maximum Angle, (-10) reference value 
        public int HigherAngle = -10;

        //(int) SmallerAngle, squat minimum Angle, (1000) reference value 
        public int SmallerAngle = 1000; 

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor _kinect;

        
        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;

        //private Int32Rect _colorImageBitmapRect;
        //private int _colorImageStride;
        private Skeleton[] _frameSkeletons;

        /// <summary>
        /// List of buttons for each interface
        /// </summary>
        List<Button> _buttons;
        //private List<Button> _btnSetConfigurations;
        //private List<Button> _btndashboard;
        //private List<Button> _btnKeyPad;

        static Button _selected;

        float _handX;
        float _handY;     
        
        //Booleans are Change to TRUE if user is Synchronized and ready
        public bool AngleIsCaptured;
        public bool AngleIsSet;                               
        
         

        /// <summary>
        /// Buttons Sound
        /// </summary>
        SoundPlayer _buttonSound;
        SoundPlayer _vozSuma;

        //My new Random Number
        Random randomNumber = new Random();


        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            IsButtonCollapsed(BtnAceptarCorrecto);
            IsButtonCollapsed(BtnAceptarIncorrecto);

            InitializeButtons();
            kinectCursorMainWindow.Click += KinectCursorMainWindowClick;
            kinectCursorSettings.Click += KinectCursorSettingsClick;
            kinectCursorDashbd.Click += KinectCursorDashbdClick;


            this.Loaded += (s, e) => DiscoverKinectSensor();
            this.Unloaded += (s, e) => { this.Kinect = null; };

            _timer = new System.Windows.Threading.DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 1) };
            _timer.Tick += TimerTick;
        }

        #region Elements Visibility
        public void IsButtonVisible(Button name)
        {
            name.Visibility = Visibility.Visible;
        }

        public void IsButtonCollapsed(Button name)
        {
            name.Visibility = Visibility.Collapsed;
        }

        public void IsBtnHoverVisible(HoverButton name)
        {
            name.Visibility = Visibility.Visible;
        }

        public void IsBtnHoverCollapsed(HoverButton name)
        {
            name.Visibility = Visibility.Collapsed;
        }
        #endregion

        

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this._sensor = potentialSensor;
                    break;
                }
            }

            if (null != this._sensor)
            {
                try
                {
                    // Start the sensor!
                    this._sensor.Start();
                    // Turn on the color stream to receive color frames
                    this._sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                    // Allocate space to put the pixels we'll receive
                    this.colorPixels = new byte[this._sensor.ColorStream.FramePixelDataLength];

                    // This is the bitmap we'll display on-screen
                    this.colorBitmap = new WriteableBitmap(this._sensor.ColorStream.FrameWidth, this._sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                    // Set the image we display to point to the bitmap where we'll put the image data
                    this.VideoStream.Source = this.colorBitmap;

                    // Add an event handler to be called whenever there is new color frame data
                    this._sensor.ColorFrameReady += this.SensorColorFrameReady;

                }
                catch (IOException)
                {
                    // Some other application is streaming from the same Kinect sensor
                    this._sensor = null;
                }
            }

            
            var imageClockPath = new Image
            {
                Source =
                    new BitmapImage(
                        new Uri(
                            Path.GetDirectoryName(
                                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) +
                            "/Images/imageClock.png"))
            };
            imageClock.Source = imageClockPath.Source;            
            

            //sign up for the event
            kinectSensorChooserSP.KinectSensorChanged += KinectSensorChooserPpKinectSensorChanged;
            //canvasPersonalPreferences.Margin = new Thickness(244, 109, 0, 150);

            var soundPath = new Uri(Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) +
                                    "/Sounds/snd_buttonselect.wav");

            var soundPath2 = new Uri(Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) +
                                    "/Sounds/Vozsumasp.wav");

            _buttonSound = new SoundPlayer(soundPath.ToString());

            
            _vozSuma = new SoundPlayer(soundPath2.ToString());


        }

        /// <summary>
        /// Event handler for Kinect sensor's ColorFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }
        }
        
        public void TimerTick(object sender, EventArgs e)
        {
            TimerCounter += 1;
            Minute = TimerCounter / 60;
            Second = TimerCounter % 60;
            txtBlockTimer.Text = Minute.ToString("00") + ":" + Second.ToString("00");
        }


        #region initialize buttons to be checked
        private void InitializeButtons()
        {
            //_btndashboard = new List<Button> { BtnBack };
            _buttons = new List<Button> { btnPlay, btnStop, btnPause, btnQuit, btnDashboard, btnReset,btnNumber0, btnNumber1, btnNumber2, btnNumber3, btnNumber4, btnNumber5,
                                          btnNumber6, btnNumber7, btnNumber8, btnNumber9, btnNumber0,
                                          btnSuma, btnAceptar , BtnAceptarCorrecto, BtnAceptarIncorrecto};
            //_btnSetConfigurations = new List<Button> { btnAddAngle, btnMinusAngle, btnAddReps, btnMinusReps, 
            //                                           BtnAddTime, BtnReduceTime, btnContinue };
            //_btnKeyPad = new List<Button>{btnNumber0, btnNumber1, btnNumber2, btnNumber3, btnNumber4, btnNumber5,
            //                              btnNumber6, btnNumber7, btnNumber8, btnNumber9, btnNumber0,
            //                              btnSuma};
        }
        #endregion
        
        

        //raise event for Kinect sensor status changed
        private void DiscoverKinectSensor()
        {
            this.Kinect = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
        }
                

        public KinectSensor Kinect
        {
            get { return this._kinect; }
            set
            {
                if (this._kinect != value)
                {
                    if (this._kinect != null)
                    {
                        UninitializeKinectSensor(this._kinect);
                        this._kinect = null;
                    }
                    if (value != null && value.Status == KinectStatus.Connected)
                    {
                        this._kinect = value;
                        InitializeKinectSensor(this._kinect);
                    }
                }
            }
        }


        ///// <summary>
        ///// Execute shutdown tasks and UninitializeKinectSensor
        ///// </summary>
        ///// <param name="kinectSensor">object sending the event</param>

        private void UninitializeKinectSensor(KinectSensor kinectSensor)
        {
            if (kinectSensor != null)
            {
                kinectSensor.Stop();
                kinectSensor.SkeletonFrameReady -= KinectSkeletonFrameReady;
            }
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="kinectSensor">object sending the event</param>

        private void InitializeKinectSensor(KinectSensor kinectSensor)
        {
            if (kinectSensor != null)
            {
                // Turn on the color stream to receive color frames
                ColorImageStream colorStream = kinectSensor.ColorStream;
                colorStream.Enable();                                       

                

                
                //reduces noise on skeletal tracking methods
                var parameters = new TransformSmoothParameters
                {
                    Smoothing = 0.3f,
                    Correction = 0.0f,
                    Prediction = 0.0f,
                    JitterRadius = 1.0f,
                    MaxDeviationRadius = 0.5f
                };

                //Pass TransformSmoothParameters to SkeletonStream
                kinectSensor.SkeletonStream.Enable(parameters);                               

                // Add an event handler to be called whenever there is new skeleton data
                kinectSensor.SkeletonFrameReady += KinectSkeletonFrameReady;

                // Add an event handler to be called whenever there is new color frame data
                //kinectSensor.ColorFrameReady += KinectColorFrameReady;
                kinectSensor.Start();
                this._frameSkeletons = new Skeleton[this.Kinect.SkeletonStream.FrameSkeletonArrayLength];

            }
        }


        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void KinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            try
            {
                using (SkeletonFrame frame = e.OpenSkeletonFrame())
                {
                    if (frame != null)
                    {
                        frame.CopySkeletonDataTo(this._frameSkeletons);
                        Skeleton skeleton = GetPrimarySkeleton(this._frameSkeletons);

                        //Joint variables
                        if (skeleton == null)
                        {
                            return;
                        }

                        #region Joints
                            // Joints to be tracked
                            var hipCenter = skeleton.Joints[JointType.HipCenter];
                            double hipCenterZPosition = hipCenter.Position.Z;
                        #endregion

                   #region Syncing User

                        if (hipCenter.TrackingState == JointTrackingState.NotTracked)
                        {
                            if (TimeSelection == Minute)
                            {
                                _timer.Stop();
                                

                            }
                            if (RepetitionCounter == 0)
                            {
                                _timer.Stop();
                            }
                            
                        }

                        //hipCenterZPosition value is expressed in meters (2.0) is equal to Two meters
                        if (hipCenterZPosition <= 1.90)
                        {
                            UserPositioninglabel.Content = "  MUEVETE HACIA ATRAS";
                            UserPositioninglabel.Foreground = Brushes.LimeGreen;
                            
                            
                            if (TimeSelection == Minute)
                            {
                                _timer.Stop();                                

                            }
                            if (RepetitionCounter == 0)
                            {
                                _timer.Stop();                                

                            }

                        }

                        if (hipCenterZPosition > 2.30)
                        {
                            UserPositioninglabel.Content = "MUEVETE HACIA ADELANTE";
                            UserPositioninglabel.Foreground = Brushes.LimeGreen;
                            
                            
                            if (TimeSelection == Minute)
                            {
                                _timer.Stop();
                                

                            }
                            if (RepetitionCounter == 0)
                            {
                                _timer.Stop();                               

                            }

                        }


                        if (hipCenterZPosition > 1.90 && hipCenterZPosition < 2.30)
                        {
                            UserPositioninglabel.Content = "  EXCELENTE POSICIÓN";
                            UserPositioninglabel.Foreground = Brushes.White;
                            

                            if (TimeSelection == Minute)
                            {
                                _timer.Stop();                               

                            }

                            if (RepetitionCounter == 0)
                            {
                                _timer.Stop();                               

                            }
                        }
                        #endregion                       
                        Joint primaryHand = GetPrimaryHand(skeleton);
                        TrackHand(primaryHand);
                    }
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        // Scale Kinect cursor on window(Resolution), Coding4Fun function
        private void ScalePosition(FrameworkElement element, Joint joints)
        {
            //convert the value to X/Y
            //convert & scale (.3 = means 1/3 of joint distance)
            Joint scaledJoint = joints.ScaleTo(1366, 2000, .9f, .9f);
            Canvas.SetLeft(element, scaledJoint.Position.X);
            Canvas.SetTop(element, scaledJoint.Position.Y);

        }

        //track and display hand
        private void TrackHand(Joint hand)
        {
            if (hand.TrackingState == JointTrackingState.NotTracked)
            {
                kinectCursorMainWindow.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (HideCursor==false)
                {
                    kinectCursorMainWindow.Visibility = Visibility.Visible;

                    //Map a joint location to a point on the depth map
                    //hand
                    DepthImagePoint point = this.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(hand.Position,DepthImageFormat.Resolution640x480Fps30);
                    //DepthImagePoint point2 = this.Kinect.MapSkeletonPointToDepth(hand.Position, DepthImageFormat.Resolution640x480Fps30);
                    
                    _handX = (int)((point.X * GridLayoutRoot.ActualWidth / this.Kinect.DepthStream.FrameWidth) -
                        (kinectCursorMainWindow.ActualWidth / 2.0));
                    _handY = (int)((point.Y * GridLayoutRoot.ActualHeight / this.Kinect.DepthStream.FrameHeight) -
                        (kinectCursorMainWindow.ActualHeight / 2.0));
                    Canvas.SetLeft(kinectCursorMainWindow, _handX);
                    Canvas.SetTop(kinectCursorMainWindow, _handY);
                    ScalePosition(kinectCursorMainWindow, hand);


                    if (isHandOver(kinectCursorMainWindow, _buttons)) kinectCursorMainWindow.Hovering();
                    else kinectCursorMainWindow.Release();

                    if (hand.JointType == JointType.HandRight)
                    {

                        var imageRightHand = new Image
                        {
                            Source =
                                new BitmapImage(
                                    new Uri(
                                        Path.GetDirectoryName(
                                            System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) +
                                        "/Images/hand_r.png"))
                        };                      


                        kinectCursorMainWindow.ImageSource = imageRightHand.Source.ToString();
                        kinectCursorMainWindow.ActiveImageSource = imageRightHand.Source.ToString();

                    }
                    else
                    {

                        var imageLeftHand = new Image
                        {
                            Source =
                                new BitmapImage(
                                    new Uri(
                                        Path.GetDirectoryName(
                                            System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) +
                                        "/Images/hand_l.png"))
                        };

                        kinectCursorMainWindow.ImageSource = imageLeftHand.Source.ToString();
                        kinectCursorMainWindow.ActiveImageSource = imageLeftHand.Source.ToString();
                    } 
                }
            }
        }

        //detect if hand is overlapping over any button
        private bool isHandOver(FrameworkElement hand, IEnumerable<Button> buttonslist)
        {
            var handTopLeft = new Point(Canvas.GetLeft(hand), Canvas.GetTop(hand));
            var handX = handTopLeft.X + hand.ActualWidth / 2;
            var handY = handTopLeft.Y + hand.ActualHeight / 2;

            foreach (Button target in buttonslist)
            {
                var targetTopLeft = new Point(Canvas.GetLeft(target), Canvas.GetTop(target));
                if (handX > targetTopLeft.X && handX < targetTopLeft.X + target.Width && handY > targetTopLeft.Y && handY < targetTopLeft.Y + target.Height)
                {
                    _selected = target;
                    return true;

                }
            }
            return false;
        }

        //get the hand closest to the Kinect sensor
        private static Joint GetPrimaryHand(Skeleton skeleton)
        {
            var primaryHand = new Joint();
            if (skeleton != null)
            {
                primaryHand = skeleton.Joints[JointType.HandLeft];
                Joint rightHand = skeleton.Joints[JointType.HandRight];
                if (rightHand.TrackingState != JointTrackingState.NotTracked)
                {
                    if (primaryHand.TrackingState == JointTrackingState.NotTracked)
                    {
                        primaryHand = rightHand;
                    }
                    else
                    {
                        if (primaryHand.Position.Z > rightHand.Position.Z)
                        {
                            primaryHand = rightHand;
                        }
                    }
                }
            }
            return primaryHand;
        }


        //get the skeleton closest to the Kinect sensor
        private static Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
        {
            Skeleton skeleton = null;
            if (skeletons != null)
            {
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (skeleton == null)
                        {
                            skeleton = skeletons[i];
                        }
                        else
                        {
                            if (skeleton.Position.Z > skeletons[i].Position.Z)
                            {
                                skeleton = skeletons[i];
                            }
                        }
                    }
                }
            }
            return skeleton;
        }    



        void KinectSensorChooserPpKinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            var oldSensor = (KinectSensor)e.OldValue;

            //stop the old sensor
            if (oldSensor != null)
            {
                oldSensor.Stop();
            }

            //get the new sensor
            var newSensor = (KinectSensor)e.NewValue;
            if (newSensor == null)
            {
                return;
            }

            //register for event and enable Kinect sensor features you want
            // Turn on the color stream to receive color frames
                this._sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this._sensor.ColorStream.FramePixelDataLength];

                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(this._sensor.ColorStream.FrameWidth, this._sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                // Set the image we display to point to the bitmap where we'll put the image data
                this.VideoStream.Source = this.colorBitmap;

                // Add an event handler to be called whenever there is new color frame data
                this._sensor.ColorFrameReady += this.SensorColorFrameReady;

                
            //newSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            newSensor.SkeletonStream.Enable();
            //newSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            

            //sign up for events if you want to get at API directly
            //newSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(NewSensorAllFramesReady);


            try
            {
                newSensor.Start();
            }
            catch (System.IO.IOException)
            {
                //this happens if another app is using the Kinect
                kinectSensorChooserSP.AppConflictOccurred();
            }
        }

        //this event fires when Color/Depth/Skeleton are synchronized
        //void NewSensorAllFramesReady(object sender, AllFramesReadyEventArgs e)
        //{

        //    using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
        //    {
        //        if (depthFrame == null)
        //        {
        //            return;
        //        }

        //        byte[] pixels = GenerateColoredBytes(depthFrame);

        //        //number of bytes per row width * 4 (B,G,R,Empty)
        //        int stride = depthFrame.Width * 4;

                

        //    }
        //}

        


        public static byte CalculateIntensityFromDepth(int distance)
        {
            //formula for calculating monochrome intensity for histogram
            return (byte)((Math.Max(distance - MinDepthDistance, 0)
                / (MaxDepthDistanceOffset)));
        }

        #region Kinect Cursor Events

        void KinectCursorMainWindowClick(object sender, RoutedEventArgs e)
        {
            _selected.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, _selected));
            _buttonSound.Play();

        }

        void KinectCursorSettingsClick(object sender, RoutedEventArgs e)
        {
            _selected.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, _selected));
            _buttonSound.Play();
        }

        void KinectCursorDashbdClick(object sender, RoutedEventArgs e)
        {
            _selected.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, _selected));
            _buttonSound.Play();
        }

        #endregion

        #region Interface buttons events
        float _frandomNumber;
        Boolean _hayPause = false;
        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            IsButtonVisible(btnPause);
            IsButtonCollapsed(btnPlay);
            
            if (_hayPause != true)
            {
                _vozSuma.Play();

                _frandomNumber = randomNumber.Next(2, 20);
                lblRandomNumber.Content = _frandomNumber.ToString();

                IsButtonCollapsed(btnReset);
                _timer.Start();

                BoolStartTimer = true;
                BoolStartSquat = true;
                _hayPause = false;
            }
            else
            {
                _timer.Start();

                BoolStartTimer = true;
                BoolStartSquat = true;
            }
            
        }


        private void StopButtonClick(object sender, RoutedEventArgs e)
        {                       
            _buttonSound.Play();
            _timer.Stop();
            TimerCounter = 0;
            BoolStartSquat = false;
            IsButtonVisible(btnPlay);
            IsButtonCollapsed(btnPause);
            //IsButtonVisible(btnReset);
        }

        private void PauseButtonClick(object sender, RoutedEventArgs e)
        {         
            _hayPause = true;
            _buttonSound.Play();
            BoolStartTimer = false;
            BoolStartSquat = false;
            _timer.Stop();
            IsButtonCollapsed(btnPause);
           // IsButtonVisible(btnPlay);
            //IsButtonVisible(btnReset);
        }

        private void QuitButtonClick(object sender, RoutedEventArgs e)
        {
            //_kinect.Stop();
            this.Close();
            //Application.Current.Shutdown();
        }

        private void BtnAddRepsClick(object sender, RoutedEventArgs e)
        {
            _buttonSound.Play();
            if (RepetitionCounter >= 0 && RepetitionCounter <= 48)
            {
                RepetitionCounter += 2;
                RepetitionCounterCopy = RepetitionCounter;
                
            }

            if (RepetitionCounter > 0 && RepetitionCounter < 10)
            {
                lblReps.Content = " " + RepetitionCounter.ToString(CultureInfo.CurrentCulture);
            }
            else
            {
                lblReps.Content = "" + RepetitionCounter.ToString(CultureInfo.CurrentCulture);
            }
        }

        private void BtnMinusRepsClick(object sender, RoutedEventArgs e)
        {
            _buttonSound.Play();
            if (RepetitionCounter >= 5)
            {
                RepetitionCounter -= 2;
                RepetitionCounterCopy = RepetitionCounter;
                
            }
            if (RepetitionCounter > 0 && RepetitionCounter < 10)
            {
                lblReps.Content = " " + RepetitionCounter.ToString(CultureInfo.CurrentCulture);
            }
            else
            {
                lblReps.Content = "" + RepetitionCounter.ToString(CultureInfo.CurrentCulture);
            }
            if (RepetitionCounter <= 0)
            {
                RepetitionCounter = 0;
                lblReps.Content = " " + RepetitionCounter.ToString(CultureInfo.CurrentCulture);
                RepetitionCounter = 0;

            }
        }        

        private void BtnAddTimeClick(object sender, RoutedEventArgs e)
        {

            _buttonSound.Play();
            TimeSelection += 2;
            lblTimer.Content = TimeSelection.ToString(CultureInfo.CurrentCulture) + "'";

            if (TimeSelection >= 4 && TimeSelection < 10)
            {
                lblTimer.Content = " " + TimeSelection.ToString(CultureInfo.CurrentCulture) + "'";

            }
            if (TimeSelection > 10)
            {
                TimeSelection = 10;
                lblTimer.Content = "" + TimeSelection.ToString(CultureInfo.CurrentCulture) + "'";
            }
        }

        private void BtnReduceTimeClick(object sender, RoutedEventArgs e)
        {
            _buttonSound.Play();
            if (TimeSelection >= 4 && TimeSelection <= 10)
            {
                TimeSelection -= 2;

                lblTimer.Content = " " + TimeSelection.ToString(CultureInfo.CurrentCulture) + "'";

            }
            if (TimeSelection <= 4)
            {
                TimeSelection = 4;
                lblTimer.Content = " " + TimeSelection.ToString(CultureInfo.CurrentCulture) + "'";
            }
        }

        void BtnResetClick(object sender, RoutedEventArgs e)
        {
            //_buttonSound.Play();
            //btnDashboard.Foreground = Brushes.White;
            //TimerCounter = 0;

            //_timer.Stop();
            //BoolStartTimer = false;

            //RepetitionCounter = RepetitionCounterCopy;
            //lblReps.Content = RepetitionCounterCopy.ToString(CultureInfo.InvariantCulture);

            //LblUserMessages.Content = "PRESS PLAY TO START AGAIN!";
        }


        private void BtnContinueClick(object sender, RoutedEventArgs e)
        {
            _buttonSound.Play();
                        
            canvasPersonalPreferences.Visibility = Visibility.Collapsed;
            //canvasSpecialLearningInterface.Visibility = Visibility.Visible;
            
            if (TimeSelection < 10)
            {
                lblClock.Content = " " + TimeSelection.ToString(CultureInfo.InvariantCulture) + "'";
            }
            else
            {
                lblClock.Content = TimeSelection.ToString(CultureInfo.InvariantCulture) + "'";
            }
        }


        private void BtnDashboardClick(object sender, RoutedEventArgs e)
        {
            _buttonSound.Play();
            canvasActivityDashboard.Visibility = Visibility.Visible;
            HideCursor = false;
            kinectCursorMainWindow.Visibility = Visibility.Visible;            
            _timer.Stop();
            
            canvasMainDashboard.Visibility = Visibility.Visible;
            BtnBack.Visibility = Visibility.Visible;

            
            if (RepetitionCounterCopy == 0)
            {
                RepetitionCounterCopy = 1;
            }
            const int maximumPercentaje = 100;
            RepsPerformed = RepetitionCounterCopy - RepetitionCounter;
            int ruleThreeResult = ((RepsPerformed * maximumPercentaje) / RepetitionCounterCopy);

            progressBarReps.Minimum = 0;
            progressBarReps.Maximum = 100;
            progressBarReps.Value = ruleThreeResult;
            progressBarReps.Foreground = Brushes.DarkOrange;
            lblRepsPercentage.Content = ruleThreeResult + "% of goal " + RepetitionCounterCopy;

            /////////////////////////////////////////////
            lblShowTimerDashBd.Content = Minute.ToString("00") + ":" + Second.ToString("00");


            int rulethreeResultTime = ((TimerCounter * maximumPercentaje) / (60 * TimeSelection));
            progressBarTime.Minimum = 0;
            progressBarTime.Maximum = 100;
            //progressBarTime.Value = RulethreeResultTime;
            progressBarTime.Foreground = Brushes.DarkOrange;
            progressBarTime.Value = rulethreeResultTime;
            lblTimePercentage.Content = rulethreeResultTime + "% of goal " + TimeSelection + ":00 Minutes";

            ///////////////////////////////////////////////////////////////////////////////////////

            if (HigherAngle == -10 && SmallerAngle == 1000)
            {
                HigherAngle = 0;
                SmallerAngle = 0;
            }
        }

        private void BtnBackClick(object sender, RoutedEventArgs e)
        {
            _buttonSound.Play();
            
            GridLayoutRoot.Visibility = Visibility.Visible;
            canvasMainDashboard.Visibility = Visibility.Collapsed;
            BtnBack.Visibility = Visibility.Collapsed;
        }

        #endregion
        
        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this._sensor)
            {
                this._sensor.Stop();
                this._sensor = null;
            }
        }

        

        List<int> listNumeros = new List<int>();
        int sumatoria;

        public void SumarNum()
        {
            for (int i = 0; i < listNumeros.Count; i++)
            {
                sumatoria += listNumeros[i];
            }
        }

        public void MostrarSuma()
        {
            txtResultado.Text = "";

            for (int i = 0; i < listNumeros.Count; i++)
            {
                txtResultado.Text += listNumeros[i];
                if (i != listNumeros.Count - 1)
                {
                    txtResultado.Text += " + ";
                }
            }
        }
        #region Eventos Botones        
        
        private void BtnNumber1Click(object sender, RoutedEventArgs e)
        {

            listNumeros.Add(1);
            MostrarSuma();
            
        }

        private void BtnNumber2Click(object sender, RoutedEventArgs e)
        {

            listNumeros.Add(2);
            MostrarSuma();
        }

        private void BtnNumber3Click(object sender, RoutedEventArgs e)
        {

            listNumeros.Add(3);
            MostrarSuma();
        }        
        

        private void BtnNumber4Click(object sender, RoutedEventArgs e)
        {
            listNumeros.Add(4);
            MostrarSuma();        
        }

        private void BtnNumber5Click(object sender, RoutedEventArgs e)
        {
            listNumeros.Add(5);
            MostrarSuma();
        } 

        private void BtnNumber6Click(object sender, RoutedEventArgs e)
        {
            listNumeros.Add(6);
            MostrarSuma(); 
        }

        private void BtnNumber7Click(object sender, RoutedEventArgs e)
        {
            listNumeros.Add(7);
            MostrarSuma();
        }

        private void BtnNumber8Click(object sender, RoutedEventArgs e)
        {
            listNumeros.Add(8);
            MostrarSuma();
        }

        private void BtnNumber9Click(object sender, RoutedEventArgs e)
        {
            listNumeros.Add(9);
            MostrarSuma(); 
        }

        private void BtnNumber0Click(object sender, RoutedEventArgs e)
        {
            listNumeros.Add(0);
            MostrarSuma();
        }
        #endregion
        

        private void BtnResultadoClick1(object sender, RoutedEventArgs e)
        {
            SumarNum();

            txtResultado.Text += " = " + sumatoria;

            if (_frandomNumber == sumatoria)
            {
                _buttonSound.Play();
                IsButtonVisible(BtnAceptarCorrecto);
            }
            else
            {
                IsButtonVisible(BtnAceptarIncorrecto);
                _buttonSound.Play();
                sumatoria = 0;
                listNumeros.Clear();
            }   
        }

        private void BtnAceptarClick(object sender, RoutedEventArgs e)
        {
            IsButtonCollapsed(BtnAceptarCorrecto);
            IsButtonCollapsed(BtnAceptarIncorrecto);
            _frandomNumber = randomNumber.Next(2, 20);
            lblRandomNumber.Content = _frandomNumber.ToString();
            sumatoria = 0;
            LblUserMessages.Content = "";
            txtResultado.Text = "";
            listNumeros.Clear();
        }

        private void BtnLimpiarSumaClick1(object sender, RoutedEventArgs e)
        {
            
            LblUserMessages.Content = "";
            txtResultado.Text = "";
            listNumeros.Clear();
            sumatoria = 0;
        }
        
    }
}
#endregion