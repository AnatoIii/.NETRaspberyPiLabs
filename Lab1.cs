using System.Device.Gpio;

public class Lab1
{
    // GPIO 17 which is physical pin 11
    private const int _ledPin = 17;
    // LED active time
    private const int _activeLedTimeMs = 300;
    // LED sleep time
    private const int _sleepLedTimeMs = 300;

    public static void Run()
    {
        using (var controller = new GpioController())
        {
            // sets the pin to output mode so we can switch something on
            controller.OpenPin(_ledPin, PinMode.Output);
            controller.Write(_ledPin, PinValue.High);

            // we'll loop forever now - CTRL + C to exit
            while (true)
            {
                Console.WriteLine($"LED on for {_activeLedTimeMs}ms");

                // turn on the LED
                controller.Write(_ledPin, PinValue.Low);
                Thread.Sleep(_activeLedTimeMs);
                Console.WriteLine($"LED off for {_sleepLedTimeMs}ms");

                // turn off the LED
                controller.Write(_ledPin, PinValue.High);
                Thread.Sleep(_sleepLedTimeMs);
            }
        }
    }
}
