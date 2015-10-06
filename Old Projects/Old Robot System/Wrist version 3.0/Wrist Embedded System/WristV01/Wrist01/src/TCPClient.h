#pragma once

/**
*	The MIT License (MIT)
*
*	Copyright (c) 2011-2014 DreanNet, EESC-USP.
*
*	Permission is hereby granted, free of charge, to any person obtaining a copy
*	of this software and associated documentation files (the "Software"), to deal
*	in the Software without restriction, including without limitation the rights
*	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*	copies of the Software, and to permit persons to whom the Software is
*	furnished to do so, subject to the following conditions:*
*
*	The above copyright notice and this permission notice shall be included in
*	all copies or substantial portions of the Software.
*
*	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
*	THE SOFTWARE.
*/

#include <winsock2.h>
#include <stdio.h>
#include "NetworkServices.h"

#define LOCAL_HOST		"169.254.210.131"
#define DEFAULT_PORT	12322

class TCPClient
{
public:

	TCPClient();
	TCPClient(const char *address, int port, u_long nonBlock = false, char nagle = 1);
	
	~TCPClient(void);
	void Close(void);
	void ShutdownSocket(void);
	bool InitializeSockets(void);
	bool Open(void);
	bool IsOpen(void) const;
	
	int Send(char *message, int messageSize);
	int Receiver(char *buffer, int bufferSize);
	
private:

	SOCKET m_Socket;
	int m_iResult;
	const char *m_sAddress;
	int m_iPort;
	char m_cNagle;
	u_long m_iNonBlock;
};