using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class CommandQueueThreadPool : MonoBehaviour
{
	private Queue<ICommand> m_queue;
	private object m_lock;
	private bool m_processing;
	private System.DateTime m_startTime;
	private bool m_running = false;

	void OnEnable()
	{
		m_queue = new Queue<ICommand>();
		m_lock = new Object();
		Run ();
	}

	void OnDisable()
	{
		Stop ();
	}

	//Commence command queue processing
	public void Run()
	{
		m_running = true;

		StartClock ();

		Debug.Log ("Command queue thread started.");
	}

	//Cease processing commands
	public void Stop()
	{
		m_running = false;
		m_queue.Clear ();

		Debug.Log ("Command queue thread stopped.");
	}

	//Add a new command
	public void Add(ICommand item) 
	{
		if (m_running)
			m_queue.Enqueue (item);
	}

	//Clear command queue completely
	public void Clear()
	{
		m_queue.Clear ();
	}

	private void Update() 
	{
		if (m_running)
		{
			bool canRun = false;

			lock (m_lock) {
				canRun = !m_processing;
			}

			if (canRun) 
			{
				ICommand currentCommand = null;

				if (m_queue.Count > 0)
					currentCommand = m_queue.Dequeue ();

				if (currentCommand != null) 
				{
					//double timeBefore = GetClock ();
					lock (m_lock) {
						m_processing = true;
					}
					ThreadPool.QueueUserWorkItem (new WaitCallback ((o) => {
						currentCommand.Execute (); 
						lock (m_lock) {
							m_processing = false;
						}
					}));

					//double timeAfter = GetClock ();
					//Debug.Log ("CommandTime: " + (timeAfter - timeBefore).ToString ());
				}
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
