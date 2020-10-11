# Hardware Abstraction Layer

Because the compiled code has to run on different platforms that can differ greatly on how the hardware is interfaced, the HAL is needed as an API layer of sorts, for the compiler to talk against.

The purpose of the HAL is to give the compiler a standardized way to interact with the most basic functionality of the system needed for the compiled code.

The most typical input/output interface would be the (serial) console.

Any text written by the program to the output (like printf in C/C++) should be directed to the console.
The console can also be used by the program to receive text input from the user.

- Display
  - Text
  - Graphics
- Keyboard
- Mouse
- Joystick
- Audio
  - Output (Single/multiple bits)
  - Input

---

As an alternative the system hardware interface could be provided as an external library.
That library would probably be written in assembly to interact with the system hardware directly
-or- be a wrapper around existing bios/kernel routines.
