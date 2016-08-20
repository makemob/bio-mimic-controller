using UnityEngine;
using System.Collections;
using Modbus;
using System.IO.Ports;

public class SerialComms : MonoBehaviour {

	protected SerialPort m_serial;

	void Start()
	{
		Startup ();
	}

	public virtual void Startup () 
	{
        string portName = "/dev/tty.usbserial-A101OCIF";
        int baudRate = 9600;
        string [] args = System.Environment.GetCommandLineArgs ();
		for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Equals("-portName", System.StringComparison.OrdinalIgnoreCase) && (i + 1) < args.Length)
            {
                portName = args[i+1];
            }
            else if (args[i].Equals("-baudRate", System.StringComparison.OrdinalIgnoreCase) && (i + 1) < args.Length)
            {
                baudRate = System.Convert.ToInt32(args[i + 1]);
            }
		}

        Debug.Log("Attempting to open serial port. PortName: " + portName + " BaudRate: " + baudRate);

		m_serial = new SerialPort(portName, baudRate);
		try {
			
			if (m_serial != null)
				m_serial.Open();
		}
		catch {
			Debug.Log("Failed to open serial port.");
		}

		if (m_serial.IsOpen)
			Debug.Log("Opened serial port.");
	}
	
	public virtual void Shutdown () 
	{
		SerialWrite("s\r");
	}

	public virtual void Move(int speed) 
	{
		string command = string.Empty;
		command += (speed < 0) ? "r" : "f";		//forwards or backwards
		command += Mathf.Abs(speed).ToString();	//send speed value 0-255
		command += "\r";						//return

		SerialWrite(command);
	}

	protected virtual void SerialWrite(string command)
	{
		Debug.Log ("Sending command: " + command);
		if (m_serial != null && m_serial.IsOpen)
		{ 
			m_serial.Write(command);
		}
		else
		{
			Debug.Log("Command not set. Serial port is not open.");
		}
	}
}
