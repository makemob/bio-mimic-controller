using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandQueue : MonoBehaviour {

	public float m_commandDelay = 0.02f;				//Enforced time between commands
	public Queue<ICommand> m_queue = new Queue<ICommand>();
	private Coroutine m_commandProcessing = null;

	void OnEnable()
	{
		Run ();
	}

	void OnDisable()
	{
		Stop ();
	}
	//Commence command queue processing
	public void Run()
	{
		if (m_commandProcessing == null) 
			m_commandProcessing = StartCoroutine(ProcessCommands());
	}

	//Cease processing commands
	public void Stop()
	{
		if (m_commandProcessing != null) 
		{
			StopCoroutine(m_commandProcessing);
			m_commandProcessing = null;
		}
	}

	//Add a new command
	public void Add(ICommand item) 
	{
		m_queue.Enqueue(item);
	}

	//Clear command queue completely
	public void Clear()
	{
		m_queue.Clear();
	}

	//This runs as a coroutine, processing commands with a given command delay in between
	private IEnumerator ProcessCommands() 
	{
		while (true) 
		{
			yield return new WaitForSeconds(m_commandDelay);
			if (m_queue.Count > 0)
				m_queue.Dequeue().Execute();
		}
	}	
}
