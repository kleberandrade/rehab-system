#pragma once

#ifndef THREAD_H
#define THREAD_H

#include <stdio.h>     
#include <windows.h>

/*******************************************************************
*   ESTRUTURA DA CLASSE
*******************************************************************/

class Thread
{
private:
    //This is the state of our thread. True means it's running, 
    //otherwise it's false.
    bool m_running;
    //This variable is the signal for our thread to stop execution. 
    //If it's True then thread will gently stop.
    bool m_end;
    //This variable is miliseconds of delay for our join function.
    //I'll explain it later.
    int m_joinTime;
    //This is the identifier of our thread, each one has it's 
    //own unique identifier.
	DWORD m_threadId;
    //This is the handle to our thread system give us when 
    //Thread is created.
	HANDLE m_handle;
    //Static run method, passed to CreateThread()
    static DWORD WINAPI static_run(void *param);
    //Final run method, here goes our code
	DWORD run();

public:

	/**
	* Construtor da classe
	*/
	Thread(void);

	/**
	* Inicializa a thread
	*/
	void Start(void);

    /**
	* Simples método para encerrar a thread
	*/
	void Stop(void);

	void Join(void);
};

/*******************************************************************
*   IMPLEMENTAÇÃO DA CLASSE
*******************************************************************/

inline Thread::Thread(void)
{
    //This means thread is not running
    m_running = false;
    //Our signal for stopping thread, in this case not.
    m_end = false;
    //Delay set up to 100 miliseconds.
	m_joinTime = 100;
    //Set default values for identifier and handle
	m_threadId = 0;
	m_handle = 0;
}

inline void Thread::Start(void)
{
    //Pass static_run function and whole object as parameter.
    //Get handle to our thread and identifier (tid)
	m_handle = CreateThread(NULL, 0, static_run, (void *)this, 0, &m_threadId);
}

inline void Thread::Stop(void)
{
	m_end = true;             //Sets signal to stop.
}

//Safer method to stop
inline void Thread::Join(void)
{
	m_end = true;				//Sets signal to stop.
	do{
		Sleep(m_joinTime);		//Delay.
	}while(m_running);			//Loops until Running is set to false.
}

inline DWORD WINAPI Thread::static_run(void *param)
{
	Thread *thread = (Thread *)param;
	return thread->run();
}


//Final run method.
inline DWORD Thread::run(void)
{
	m_running = true;     //Set state to running.
	do{
        //Some example code
		printf("Thread id %d\n",m_threadId);
		Sleep(1000);			//Delay. You can change it for your needs.
	}while(!m_end);				//Loops until ending signal is true.
	m_running = false;			//Set state to stopped.
	return 0;
}

#endif