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

#include "NetworkRequestData.h"


class GameRequestMessage : public NetworkRequestData
{
public:
	inline double GetPosition() const
	{
		return m_dPosition;
	}

	inline double GetStiffness() const
	{
		return m_dStiffness;
	}

	inline double GetVelocity() const
	{
		return m_dVelocity;
	}

	inline double GetAcceleration() const
	{
		return m_dAcceleration;
	}

	inline int GetControl() const
	{
		return m_iControl;
	}

protected:
	void OnDeserialize(BufferDecode &decode);

private:
	double m_dPosition;
	double m_dStiffness;
	double m_dVelocity;
	double m_dAcceleration;
	int m_iControl;
};

