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

#include "BufferDecode.h"


BufferDecode::BufferDecode(void)
	: Buffer(),
	m_iPosition(0)
{
	Clear();
}

BufferDecode::BufferDecode(char buffer[])
	: Buffer(buffer),
	m_iPosition(0)
{

}

double BufferDecode::DecodeDouble()
{
	double value;
	memcpy(&value, &m_strBuffer[m_iPosition], sizeof(double));
	m_iPosition += sizeof(double);
	return Encoding::NetworkToHostDouble(value);
}

float BufferDecode::DecodeFloat()
{
	float value;
	memcpy(&value, &m_strBuffer[m_iPosition], sizeof(float));
	m_iPosition += sizeof(float);
	return Encoding::NetworkToHostFloat(value);
}

int BufferDecode::DecodeInt()
{
	int value;
	memcpy(&value, &m_strBuffer[m_iPosition], sizeof(int));
	m_iPosition += sizeof(int);
	return Encoding::NetworkToHostInt(value);
}


void BufferDecode::Clear(void)
{
	Buffer::Clear();
	m_iPosition = 0;
}