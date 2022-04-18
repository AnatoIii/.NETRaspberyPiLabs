using System.Device.Gpio;

public class Lab2
{
    // GPIO 17 which is physical pin 11
    private const int _ledPin = 17;
    // GPIO 22 which is physical pin 15
    private const int _switchPin = 22;
    // LED active time
    private const int _activeLedTimeMs = 300;
    // LED sleep time
    private const int _sleepLedTimeMs = 200;

    // value to multiply lightTimeInMilliseconds when button pressed
    private static int _multiplicator = 1;
    // indicate active LED time (_activeLedTimeMs * _multiplicator)
    private static int _lightTime = _activeLedTimeMs;
    
    private static void _OnSignalPinValueChangedEvent(object sender, PinValueChangedEventArgs args)
    {
        // button pressed
        // here we have 5 modes:
        // 1 - 300ms
        // 2 - 600ms
        // ...
        // 5 - 1500ms
        if (_multiplicator < 5)
            _multiplicator++;
        else
            _multiplicator = 1;

        // update LED active time depend on button
        _lightTime = _activeLedTimeMs * _multiplicator;
        Console.WriteLine($"Value changed to {_lightTime}");
    }

    public static void Run()
    {
        using (var controller = new GpioController())
        {
            // sets the pin to output mode so we can switch something on
            controller.OpenPin(_ledPin, PinMode.Output);
            controller.Write(_ledPin, PinValue.High);

            // Set the pin mode. We are using InputPullDOwn which uses a pulldown resistor to
            // ensure the input reads Low until an external switch sets it high,
            // We could have used InputPullUp if we wanted it to read High until it was pulled down Low.
            // If we just used Input, the value on the pin would wander between High and Low... not very useful in this situation.
            controller.OpenPin(_switchPin, PinMode.InputPullDown);
            controller.RegisterCallbackForPinValueChangedEvent(_switchPin, PinEventTypes.Falling, _OnSignalPinValueChangedEvent);

            // we'll loop forever now - CTRL + C to exit
            while (true)
            {
                Console.WriteLine($"LED on for {_lightTime}ms");

                // turn on the LED
                controller.Write(_ledPin, PinValue.Low);
                Thread.Sleep(_lightTime);
                Console.WriteLine($"LED off for {_sleepLedTimeMs}ms");

                // turn off the LED
                controller.Write(_ledPin, PinValue.High);

                Thread.Sleep(_sleepLedTimeMs);
            }
        }
    }
}
