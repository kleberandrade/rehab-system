#pragma once

#ifndef CAN_FRAME_H
#define	CAN_FRAME_H

#include <windows.h>
#include <stdlib.h>
#include <stdio.h>

#include "GPIOLib.h"
#include "canlib-MCP2515.h"

#define DEVICE					0
#define CAN_MESSAGE_LENGTH      (8)             ///< CAN message length
#define COBID_NMT				(0x000)			/// CAN identifier MESTRE: EPOS -> Escravos
#define CAN_DESCRIPTOR          (0x80000000)
#define CAN_MASK                (0xFFFFFFFF)

#define CAN_MESSAGE canMessageExt

namespace CAN
{
	/*******************************************************************
	*   ESTRUTURA DA CLASSE
	*******************************************************************/

	class CANFrame
	{
	public:
		CANFrame();
		BOOL InitCANFrame(UINT32 canBaudRate);
		BOOL InitCANNetwork();
		void PrintStat();
		void Write(CAN_MESSAGE message);
		void Read(CAN_MESSAGE message);

	private:

		void SetMasterMessage(BYTE byte0);
		void SetSDOMessage();

		DWORD m_CANStatus;

		CAN_MESSAGE m_networkManagement;	// NMT
		CAN_MESSAGE m_serviceDataObject;	// SDO
		CAN_MESSAGE m_processDataObject;	// PDO
	};

	/*******************************************************************
	*   IMPLEMENTAÇÃO DA CLASSE
	*******************************************************************/

	inline CANFrame::CANFrame()
	{
		
	}

	inline BOOL CANFrame::InitCANFrame(UINT32 canBaudRate = 500)
	{
		DWORD successStatus = TRUE;
		DWORD gpioInterruptNumber = 0;
		DWORD procType = 0; 
		PIN_INSTANCE         IntGpio;

		//VARIAVEIS LOCAIS
		DWORD versionMajor, versionMinor, buildNumber;  ///< Major, minor, build number

		//INICIALIZAÇÃO CAN
		CANLibMCP2515GetLibVersion(&versionMajor, &versionMinor, &buildNumber);
		NKDbgPrintfW(L"CAN Lib (CANLib Version %d.%d.%d)\n\r", versionMajor, versionMinor, buildNumber);

		GetGPIOFromPin(73, FALSE, &IntGpio); // CANINT# signal is on SODIMM pin 73
		gpioInterruptNumber = IntGpio.inst1;

		if (successStatus)
		{
			/// Initalizes CAN Chip
			if (0 != CANLibMCP2515Init(DEVICE, gpioInterruptNumber, WATER_MARK, BUFFER_SIZE_CAN, 0))
			{
				NKDbgPrintfW(L"InitMcp2515() failed (See serial debug output for details)\r\n");
				successStatus = FALSE;
			}
		}

		if (successStatus)
			successStatus = CANLibMCP2515WriteDescriptor(DEVICE, CAN_DESCRIPTOR, CAN_MASK);

		if (successStatus)
			successStatus = CANLibMCP2515SetBaudrate(DEVICE, canBaudRate);

		return successStatus;
	}

	inline BOOL CANFrame::InitCANNetwork()
	{
		//Envia para a rede CAN o sinal de Start do NMT
		SetMasterMessage(0x01);
		if (!CANLibMCP2515TransmitMessage(DEVICE, &m_networkManagement, 1000))
		{
			NKDbgPrintfW(L"Could not transmit NMT Start Message\n\r");
			m_CANStatus = CANLibMCP2515GetStatus(DEVICE);
			NKDbgPrintfW(L"CAN Status: 0x%x\n\r", m_CANStatus);
			return 0;
		}

		SetSDOMessage();
		if (!CANLibMCP2515TransmitMessage(DEVICE, &m_serviceDataObject, 1000))
		{
			NKDbgPrintfW(L"Could not transmit NMT Start Message\n\r");
			m_CANStatus = CANLibMCP2515GetStatus(DEVICE);
			NKDbgPrintfW(L"CAN Status: 0x%x\n\r", m_CANStatus);
			return 0;
		}

		return 1; 
	}

	inline void CANFrame::SetMasterMessage(BYTE byte0)
	{
	    /// Configuration for message type (Standard)
		m_networkManagement.frameType = STANDARD;
		m_networkManagement.id = COBID_NMT;

	    m_networkManagement.isRemoteTransmitRequest = FALSE;
	    m_networkManagement.length = CAN_MESSAGE_LENGTH;

	    m_networkManagement.data[0] = byte0;
	    m_networkManagement.data[1] = 0x00;
	    m_networkManagement.data[2] = 0x00;
	    m_networkManagement.data[3] = 0x00;
	    m_networkManagement.data[4] = 0x00;
	    m_networkManagement.data[5] = 0x00;
	    m_networkManagement.data[6] = 0x00;
	    m_networkManagement.data[7] = 0x00;
	}

	inline void CANFrame::SetSDOMessage()
	{
		/// Configuration for message type (Standard)
		m_serviceDataObject.frameType = STANDARD;
		m_serviceDataObject.id = 0x601;

	    m_serviceDataObject.isRemoteTransmitRequest = FALSE;
	    m_serviceDataObject.length = CAN_MESSAGE_LENGTH;

		m_serviceDataObject.data[0] = 0x22; 
		m_serviceDataObject.data[1] = (0x6060 & 0x000000ff);
		m_serviceDataObject.data[2] = (0x6060 & 0x0000ff00)/0x100;
		m_serviceDataObject.data[3] =  0x00;
		m_serviceDataObject.data[4] = (0xfd & 0x000000ff);
		m_serviceDataObject.data[5] = (0xfd & 0x0000ff00)/0x100;
		m_serviceDataObject.data[6] = (0xfd & 0x00ff0000)/0x10000;
		m_serviceDataObject.data[7] = (0xfd & 0xff000000)/0x1000000;  
	}

	inline void CANFrame::Write(CAN_MESSAGE message)
	{

	}

	inline void CANFrame::Read(CAN_MESSAGE message)
	{


	}
}

#endif