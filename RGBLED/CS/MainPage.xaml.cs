﻿/*
    Copyright(c) Microsoft Open Technologies, Inc. All rights reserved.

    The MIT License(MIT)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files(the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions :

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace RGBLED
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();

            Unloaded += MainPage_Unloaded;

            InitGPIO();
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                redpin = null;
                bluepin = null;
                greenpin = null;
                GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

           redpin = gpio.OpenPin(REDLED_PIN);
           bluepin = gpio.OpenPin(BLUELED_PIN);
           greenpin = gpio.OpenPin(GREENLED_PIN);

            // Show an error if the pin wasn't initialized properly
            if (redpin == null || bluepin == null || greenpin == null)
            {
                GpioStatus.Text = "There were problems initializing the GPIO red/blue/green pin.";
                return;
            }

            redpin.Write(GpioPinValue.High);
            redpin.SetDriveMode(GpioPinDriveMode.Output);
            bluepin.Write(GpioPinValue.High);
            bluepin.SetDriveMode(GpioPinDriveMode.Output);
            greenpin.Write(GpioPinValue.High);
            greenpin.SetDriveMode(GpioPinDriveMode.Output);

            GpioStatus.Text = "GPIO blue/red/green pin initialized correctly.";
        }

        private void MainPage_Unloaded(object sender, object args)
        {
            // Cleanup
            redpin.Dispose();
            bluepin.Dispose();
            greenpin.Dispose();
        }

        private void FlipLED()
        {
            if (LEDStatus == 0)
            {
               LEDStatus = 1;
                if (redpin != null && bluepin != null && greenpin != null)
                {
                    //turn on red
                    redpin.Write(GpioPinValue.High);
                    bluepin.Write(GpioPinValue.Low);
                    greenpin.Write(GpioPinValue.Low);
                }
                LED.Fill = redBrush;
            }
            else if (LEDStatus == 1)
            {
                LEDStatus = 2;
                if (redpin != null && bluepin != null && greenpin != null)
                {
                    //turn on blue
                    redpin.Write(GpioPinValue.Low);
                    bluepin.Write(GpioPinValue.High);
                    greenpin.Write(GpioPinValue.Low);
                }
               LED.Fill = blueBrush;
            }

          else
            {
                LEDStatus = 0;
                if (redpin != null && bluepin != null && greenpin != null)
                {
                    //turn on green
                    redpin.Write(GpioPinValue.Low);
                    bluepin.Write(GpioPinValue.Low);
                    greenpin.Write(GpioPinValue.High);
                }
                LED.Fill = greenBrush;
            }
        }

        private void TurnOffLED()
        {
            if (LEDStatus == 1)
            {
                FlipLED();
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            FlipLED();
        }

        private void Delay_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {            
            if (timer == null)
            {
                return;
            }
            if (e.NewValue == Delay.Minimum)
            {
                DelayText.Text = "Stopped";
                timer.Stop();
                TurnOffLED();
            }
            else
            {
                DelayText.Text = e.NewValue + "ms";
                timer.Interval = TimeSpan.FromMilliseconds(e.NewValue);
                timer.Start();
            }
        }

        private int LEDStatus = 0;
       private const int REDLED_PIN = 5;
       private const int BLUELED_PIN = 6;
       private const int GREENLED_PIN = 13;
        private GpioPin redpin;
        private GpioPin bluepin;
        private GpioPin greenpin;
        private DispatcherTimer timer;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush blueBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
    }
}
