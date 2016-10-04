using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class CommandQueueThreaded : MonoBehaviour
{
	private Queue<ICommand> m_queue;
	private Thread m_commandThread;
	private object m_lock;
	private System.DateTime m_startTime;

	void OnEnable()
	{
		m_queue = new Queue<ICommand>();
		m_commandThread = new Thread(ProcessCommands);
		m_lock = new Object();
	}

	void OnDisable()
	{
		lock (m_lock) 
		{
			m_commandThread.Abort ();
			m_queue.Clear ();
		}
	}

	//Commence command queue processing
	public void Run()
	{
		lock (m_lock) 
		{
			m_commandThread.Start();
		}

		StartClock ();
	}

	//Cease processing commands
	public void Stop()
	{
		lock (m_lock) 
		{
			m_commandThread.Abort ();
		}
	}

	//Add a new command
	public void Add(ICommand item) 
	{
		lock (m_lock) 
		{
			m_queue.Enqueue (item);
		}
	}

	//Clear command queue completely
	public void Clear()
	{
		lock (m_lock) 
		{
			m_queue.Clear ();
		}
	}

	//This runs as a coroutine, processing commands with a given command delay in between
	private void ProcessCommands() 
	{
		while (true) 
		{
			ICommand currentCommand = null;

			lock (m_lock) 
			{
				if (m_queue.Count > 0) 
					currentCommand = m_queue.Dequeue();
			}

			if (currentCommand != null) 
			{
				double timeBefore = GetClock ();
	
				currentCommand.Execute ();

				double timeAfter = GetClock ();

				Debug.Log ("CommandTime: " + (timeAfter - timeBefore).ToString());
			}
		}
	}	


	private void StartClock()
	{
		m_startTime = System.DateTime.UtcNow;
	}

	private double GetClock()
	{
		System.TimeSpan t = System.DateTime.UtcNow - m_startTime;
		return t.TotalMilliseconds;
	}
}
