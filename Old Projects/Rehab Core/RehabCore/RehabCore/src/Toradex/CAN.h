#pragma once

#ifndef CAN_H
#define	CAN_H

#include <windows.h>
#include <stdlib.h>
#include <stdio.h>

#include "canlib-MCP2515.h"

#define canMessageExt 		CAN_FRAME

namespace Toradex
{
	/*******************************************************************
	*   ESTRUTURA DA CLASSE
	*******************************************************************/

	class CAN 
	{
	public:
		static INT32 Init (DWORD device, DWORD interruptGpio, DWORD waterMark, DWORD bufferSize, DWORD processorType);
		static void Close (DWORD device);
		static BOOL WriteDescriptor (DWORD device, DWORD description, DWORD mask);
		static INT32 ReadDescriptor (DWORD device);
		static INT32 ReadMask (DWORD device);
		static BOOL Reset (DWORD device);
		static BOOL SetBaudrate (DWORD device, DWORD baudRate);
		static BOOL TransmitMessage (DWORD device, CAN_FRAME *message, DWORD timeout);
		static DWORD ReceiveMessages (DWORD device, CAN_FRAME *message, DWORD numberOfMessages, DWORD timeout);
		static INT32 GetStatus (DWORD device);
		static void PrintMessage (CAN_FRAME *message);
		static void GetLibVersion (DWORD *verionMajor, DWORD *versionMinor, DWORD *buildNumber);
		static void ResetInterruptFlags (DWORD device);
		static void Dump(DWORD device);
	};

	/*******************************************************************
	*   IMPLEMENTAÇÃO DA CLASSE
	*******************************************************************/

	inline INT32 CAN::Init(DWORD device, DWORD interruptGpio, DWORD waterMark, DWORD bufferSize, DWORD processorType)
	{
		return CANLibMCP2515Init (device, interruptGpio, waterMark, bufferSize, processorType);
	}

	inline void CAN::Close((DWORD device)
	{
		CANLibMCP2515DeInit (device);
	}

	inline BOOL CAN::WriteDescriptor (DWORD device, DWORD description, DWORD mask)
	{
		return CANLibMCP2515WriteDescriptor (device, description, mask);
	}

	inline INT32 CAN::ReadDescriptor (DWORD device)
	{
		return CANLibMCP2515ReadDescriptor (device);
	}

	inline INT32 CAN::ReadMask (DWORD device)
	{
		return CANLibMCP2515ReadMask (device);
	}

	inline BOOL CAN::Reset (DWORD device)
	{
		return CANLibMCP2515Reset (device);
	}

	inline BOOL CAN::SetBaudrate (DWORD device, DWORD baudRate)
	{
		return CANLibMCP2515SetBaudrate (device, baudRate);
	}

	inline BOOL CAN::TransmitMessage (DWORD device, CAN_FRAME *message, DWORD timeout)
	{
		return CANLibMCP2515TransmitMessage (device, &message, timeout);
	}

	inline DWORD CAN::ReceiveMessages (DWORD device, CAN_FRAME *message, DWORD numberOfMessages, DWORD timeout);
	{
		return CANLibMCP2515ReceiveMessages (device, &message, numberOfMessages, timeout);
	}

	inline INT32 CAN::GetStatus (DWORD device)
	{
		return CANLibMCP2515GetStatus (device);
	}

	inline void CAN::PrintMessage (CAN_FRAME *message)
	{
		CANLibMCP2515PrintMessage (&message);
	}

	inline void CAN::GetLibVersion (DWORD *verionMajor, DWORD *versionMinor, DWORD *buildNumber)
	{
		CANLibMCP2515GetLibVersion (&verionMajor, &versionMinor, &buildNumber);
	}

	inline void CAN::ResetInterruptFlags (DWORD device)
	{
		CANLibMCP2515ResetInterruptFlags (device);
	}

	inline void CAN::Dump(DWORD device)
	{
		DumpMcp2515 (device);
	}
}

#endif