using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace CarWifiBackground
{
    class Motor
    {
        //LEDS
        private const int LED0 = 10;
        private const int LED1 = 9;
        private const int LED2 = 25;

        //Motor drive
        private const int ENA = 13;
        private const int ENB = 20;
        private const int IN1 = 19;
        private const int IN2 = 16;
        private const int IN3 = 21;
        private const int IN4 = 26;


        //SERVO
        private const int SER1 = 11;
        private const int SER2 = 8;
        private const int SER3 = 7;
        private const int SER4 = 5;
        private const int SER7 = 6;
        private const int SER8 = 12;



        //SPEED
        private const int Left_Speed_Hold = 255;
        private const int Righ_Speed_Hold = 255;

        private GpioController MyGpioController;

        private GpioPin led0;
        private GpioPin led1;
        private GpioPin led2;
        private GpioPinValue led0Value;
        private GpioPinValue led1Value;
        private GpioPinValue led2Value;

        private GpioPin ena;
        private GpioPin in1;
        private GpioPin in2;
        private GpioPin enb;
        private GpioPin in3;
        private GpioPin in4;

        private Stopwatch timerForward;
        private Stopwatch timerBacward;
        private Stopwatch timerLeft;
        private Stopwatch timerRight;

        public Motor()
        {
            MyGpioController = GpioController.GetDefault();

            led0 = MyGpioController.OpenPin(LED0);
            led1 = MyGpioController.OpenPin(LED1);
            led2 = MyGpioController.OpenPin(LED2);

            ena = MyGpioController.OpenPin(ENA);
            in1 = MyGpioController.OpenPin(IN1);
            in2 = MyGpioController.OpenPin(IN2);
            enb = MyGpioController.OpenPin(ENB);
            in3 = MyGpioController.OpenPin(IN3);
            in4 = MyGpioController.OpenPin(IN4);

            led0Value = GpioPinValue.High;
            led1Value = GpioPinValue.High;
            led2Value = GpioPinValue.High;
            led0.SetDriveMode(GpioPinDriveMode.Output);
            led1.SetDriveMode(GpioPinDriveMode.Output);
            led2.SetDriveMode(GpioPinDriveMode.Output);

            ena.SetDriveMode(GpioPinDriveMode.Output);
            in1.SetDriveMode(GpioPinDriveMode.Output);
            in2.SetDriveMode(GpioPinDriveMode.Output);
            enb.SetDriveMode(GpioPinDriveMode.Output);
            in3.SetDriveMode(GpioPinDriveMode.Output);
            in4.SetDriveMode(GpioPinDriveMode.Output);

            timerForward = new Stopwatch();
            timerBacward = new Stopwatch();
            timerLeft = new Stopwatch();
            timerRight = new Stopwatch();
        }

        public void StartLeds()
        {

            led0.Write(led0Value);
            led1.Write(led1Value);
            led2.Write(led2Value);
            //Task.Delay(250).Wait();
            //led0.Write(GpioPinValue.Low);
            //led1.Write(GpioPinValue.Low);
            //led2.Write(GpioPinValue.Low);
            //Task.Delay(250).Wait();
            //led0.Write(GpioPinValue.High);
            //led1.Write(GpioPinValue.High);
            //led2.Write(GpioPinValue.High);
            Debug.WriteLine("Enciende leds");
        }

        public void Forward()
        {
            if (!timerForward.IsRunning)
            {
                timerForward.Restart();
            }
            ena.Write(GpioPinValue.High);
            enb.Write(GpioPinValue.High);
            in1.Write(GpioPinValue.High);
            in2.Write(GpioPinValue.Low);
            in3.Write(GpioPinValue.High);
            in4.Write(GpioPinValue.Low);
        }
        public void Backward()
        {
            if (!timerBacward.IsRunning)
            {
                timerBacward.Restart();
            }
            ena.Write(GpioPinValue.High);
            enb.Write(GpioPinValue.High);
            in1.Write(GpioPinValue.Low);
            in2.Write(GpioPinValue.High);
            in3.Write(GpioPinValue.Low);
            in4.Write(GpioPinValue.High);
        }
        public void TurnLeft()
        {
            if (!timerLeft.IsRunning)
            {
                timerLeft.Restart();
            }
            ena.Write(GpioPinValue.High);
            enb.Write(GpioPinValue.High);
            in1.Write(GpioPinValue.High);
            in2.Write(GpioPinValue.Low);
            in3.Write(GpioPinValue.Low);
            in4.Write(GpioPinValue.High);
        }
        public void TurnRight()
        {
            if (!timerRight.IsRunning)
            {
                timerRight.Restart();
            }
            ena.Write(GpioPinValue.High);
            enb.Write(GpioPinValue.High);
            in1.Write(GpioPinValue.Low);
            in2.Write(GpioPinValue.High);
            in3.Write(GpioPinValue.High);
            in4.Write(GpioPinValue.Low);
        }

        public void Stop()
        {

            ena.Write(GpioPinValue.Low);
            enb.Write(GpioPinValue.Low);
            in1.Write(GpioPinValue.Low);
            in2.Write(GpioPinValue.Low);
            in3.Write(GpioPinValue.Low);
            in4.Write(GpioPinValue.Low);
            if (timerForward.IsRunning)
            {
                Debug.WriteLine("Tiempo hacia delante: " + timerForward.ElapsedMilliseconds);
                timerForward.Stop();
                AzureIoTHub.SendDeviceToCloudMessageAsync((int)timerForward.ElapsedMilliseconds, "forward", true);
            }
            if (timerBacward.IsRunning)
            {
                Debug.WriteLine("Tiempo hacia atrás: " + timerBacward.ElapsedMilliseconds);
                timerBacward.Stop();
                AzureIoTHub.SendDeviceToCloudMessageAsync((int)timerForward.ElapsedMilliseconds, "backward", true);
            }
            if (timerLeft.IsRunning)
            {
                Debug.WriteLine("Tiempo hacia izquierda: " + timerLeft.ElapsedMilliseconds);
                timerLeft.Stop();
                AzureIoTHub.SendDeviceToCloudMessageAsync((int)timerForward.ElapsedMilliseconds, "left",false);
            }
            if (timerRight.IsRunning)
            {
                Debug.WriteLine("Tiempo hacia derecha: " + timerRight.ElapsedMilliseconds);
                timerRight.Stop();
                 AzureIoTHub.SendDeviceToCloudMessageAsync((int)timerForward.ElapsedMilliseconds, "right",false);
            }
        }
    }
}
