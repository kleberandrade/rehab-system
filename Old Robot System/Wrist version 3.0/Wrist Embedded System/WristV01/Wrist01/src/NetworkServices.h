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

/**
*	@brief Wrapper para as funções de send e recv do SOCKET
*
*/
class NetworkServices
{
public:
	/**
	*	@brief	Função send encapsulada
	*
	*	@param	socket
	*	@param	message
	*	@param	messageSize
	*
	*	@return
	*/
	static int SendMessage(SOCKET socket, char *message, int messageSize);

	/**
	*	@brief	Função recv encapsulada
	*
	*	@param	socket
	*	@param	buffer
	*	@param	bufferSize
	*
	*	@return
	*/
	static int ReceiveMessage(SOCKET socket, char *buffer, int messageSize);
};

