# bio-mimic-controller
Main pc controller for bio-mimic. Robotics simulation. Actuator, lighting and audio coordination. 

Compiled win64 version available in Binaries/Windows/UKI.zip.

Instructions:
- unzip UKI.zip
- edit UKI.bat to set your desired serial port and baud rate.
- run UKI.bat (dont use UKI.exe, it will run with default serial connection and probably not work)
- Click the "Actuators" menu to open and close it. It will let you move actuators up and down.
- Click the buttons in the bottom right to manipulate multiple actuators at once.
- The "Use multiregister" checbox will control whether to use multi-register writing. We had trouble with that in the past so it can be switched off which may help.
- Debug logs can be found in log.txt in the same folder you started the app.
- Note: There is no checking when at extents yet so if the system cannot handle that case, make sure you remember to press Stop
- Note: This app simulates six simple actuators. If the modbus setup cannot handle addresses out of range you may see errors.
- Note: I've not exposed the timing between modbus commands yet but if we see things not working that is something I'll fix very soon!


