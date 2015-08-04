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
*	OUT OF OR IN CONNECTION WITH THE SOFTWA
RE OR THE USE OR OTHER DEALINGS IN
*	THE SOFTWARE.
*/

#include "TCPClient.h"
#include "NetworkDispatcherData.h"
#include "NetworkRequestData.h"
#include "GameRequestMessage.h"
#include "RobotDispatcherMessage.h"

#define SERVER_ADDRESS	"169.254.210.131"
#define SERVER_PORT		12322
#define SLEEP_TIME		1000

class ClientNetwork
{
public:
	ClientNetwork();
	ClientNetwork(const char *host, int address);
	~ClientNetwork(void);

	bool Open(void);
	bool IsOpen(void);
	void Close(void);

	void Send(void);
	void Receive(void);

	inline NetworkDispatcherData *GetDispatcherData(void)
	{
		return m_dispatcherData;
	}

	inline NetworkRequestData *GetRequestData(void)
	{
		return m_requestData;
	}

	inline void SetDispatcher(NetworkDispatcherData *dispatcherData)
	{
		m_dispatcherData = dispatcherData;
	}

	inline void SetRequest(NetworkRequestData *requestData)
	{
		m_requestData = requestData;
	}

private:
	TCPClient m_tcpClient;
	BufferDecode m_Decode;
	BufferEncode m_Encode;
	NetworkRequestData *m_requestData;
	NetworkDispatcherData *m_dispatcherData;
};

