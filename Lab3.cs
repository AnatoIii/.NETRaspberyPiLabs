using System.Device.Gpio;
using System.Device.Pwm;

public class Lab3
{
    // GPIO 17 which is physical pin 11
    private const int _ledPin1 = 17;
    // GPIO 20 which is physical pin 38
    private const int _switchPin1 = 20;
    // GPIO 21 which is physical pin 40
    private const int _switchPin2 = 21;
    // value to indicate PWM step
    private const int _loopSleepMs = 1000;
    // value to indicate chip
    private const int _chip = 0;
    // value to indicate channel
    private const int _channel = 0;
    // value to indicate hertz
    private const int _hertz = 400;
    // value to indicate PWM step
    private const double _pwmStep = 0.2;

    // current PWM
    private static double _currentPwm = 1;

    private static PwmChannel _pwmChannel;

    static Lab3()
    {
        _pwmChannel = PwmChannel.Create(_chip, _channel, _hertz, _currentPwm);
    }

    private static void _OnIncreaseEvent(object sender, PinValueChangedEventArgs args)
    {
        // check if value can be increased
        if (_currentPwm < 1.0)
        {
            _currentPwm = Math.Round(_currentPwm + _pwmStep, 1);
            _pwmChannel.DutyCycle = _currentPwm;

            Console.WriteLine($"PWM value increased. New PWM: {_currentPwm}.");
        }
    }

    private static void _OnDecreaseEvent(object sender, PinValueChangedEventArgs args)
    {
        // check if value can be decreased
        if (_currentPwm > 0)
        {
            _currentPwm = Math.Round(_currentPwm - _pwmStep, 1);
            _pwmChannel.DutyCycle = _currentPwm;

            Console.WriteLine($"PWM value decreased. New PWM: {_currentPwm}.");
        }
    }

    public static void Run()
    {
        using (var gpioController = new GpioController())
        {
            // sets the pin to output mode and turn LED ON
            gpioController.OpenPin(_ledPin1, PinMode.Output);
            gpioController.Write(_ledPin1, PinValue.Low);

            // start PWM channel
            _pwmChannel.Start();

            // Set the pin mode. We are using InputPullDOwn which uses a pulldown resistor to
            // ensure the input reads Low until an external switch sets it high,
            // We could have used InputPullUp if we wanted it to read High until it was pulled down Low.
            // If we just used Input, the value on the pin would wander between High and Low... not very useful in this situation.
            gpioController.OpenPin(_switchPin1, PinMode.InputPullDown);
            gpioController.OpenPin(_switchPin2, PinMode.InputPullDown);

            // register callback for button press
            gpioController.RegisterCallbackForPinValueChangedEvent(_switchPin1, PinEventTypes.Falling, _OnIncreaseEvent);
            gpioController.RegisterCallbackForPinValueChangedEvent(_switchPin2, PinEventTypes.Falling, _OnDecreaseEvent);

            // we'll loop forever now - CTRL + C to exit
            while (true)
            {
                Console.WriteLine($"Loop step. Current PWM: {_currentPwm}.");

                Thread.Sleep(_loopSleepMs);
            }
        }
    }
}
//-_-\\