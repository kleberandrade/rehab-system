#pragma once

#ifndef EPOS_CONTROLLER_H
#define EPOS_CONTROLLER_H

#include "Toradex/IO.h"
#include "Toradex/CAN.h"


#define PIN_INTERRUPT			73



#define CAN_DEVICE				0
#define CAN_DESCRIPTOR          0x80000000
#define CAN_MASK                0xFFFFFFFF
#define CAN_BUFFER_SIZE         10000
#define CAN_BAUD_RATE			500
#define CAN_WATER_MARK			1
#define CAN_TIMEOUT				1000


#define CAN_MESSAGE_LENGTH      8				///< CAN message length
#define COBID_PDOTx01           0x201			/// CAN identifier: COLIBRI -> EPOS
#define COBID_PDORx01           0x181			/// CAN identifier: EPOS -> COLIBRI
#define COBID_SYNC				0x80			/// CAN identifier SYNC: EPOS -> Escravos
#define COBID_NMT				0x000			/// CAN identifier MESTRE: EPOS -> Escravos

namespace EPOS
{
	class EPOSController
	{
	public:
		BOOL Init();

		void SetMasterMessage(BYTE byte0);
		void SetSyncMessage();
		void SetSDOMessage();
		void SetEposMessage();

	private:
		BOOL SetupCANControler();
		BOOL SetupCANNetwork();

		DWORD m_positionSetPoint;
		DWORD m_current;
		DWORD m_controlWord;

		DWORD m_gpioInterruptNumber;
		CAN_FRAME m_sdoCANFrame;
		CAN_FRAME m_nmtCANFrame;
		CAN_FRAME m_syncCANFrame;
		CAN_FRAME m_pdoTransmitx01CANFrame;
		CAN_FRAME m_pdoReadx01CANFrame;	
	};

	inline BOOL EPOSController::Init()
	{
		if (!InitCANControler())
			return FALSE;

		if (!InitCANNetwork())
			return FALSE;

		return TRUE;
	}

	inline BOOL EPOSController::SetupCANControler()
	{
		// CANINT# signal is on SODIMM pin 73
		PIN_INSTANCE 	gpio;
		Toradex::IO::GetGPIO(PIN_INTERRUPT, FALSE, &gpio); 
		 /// Initalizes CAN Chip
        if (0 != Toradex::CAN::Init(CAN_DEVICE, gpio.inst1, CAN_WATER_MARK, CAN_BUFFER_SIZE, 0))
          	return FALSE;
        
		if (TRUE != Toradex::CAN::WriteDescriptor(CAN_DEVICE, CAN_DESCRIPTOR, CAN_MASK))
			return FALSE;

		if (TRUE != Toradex::CAN::SetBaudrate(CAN_DEVICE, CAN_BAUD_RATE))
			return FALSE;
    
		return TRUE;
	}

	inline BOOL EPOSController::SetupCANNetwork()
	{
		//Envia para a rede CAN o sinal de Start do NMT
		SetMasterMessage(0x01);

		if (!Toradex::CAN::TransmitMessage(CAN_DEVICE, &NMT, CAN_TIMEOUT))
			return FALSE;

		SetSDOMessage();

		if (!Toradex::CAN::TransmitMessage(CAN_DEVICE, &SDO, CAN_TIMEOUT))
			return FALSE;

		return TRUE;  
	}

	inline void EPOSController::SetMasterMessage(BYTE byte0)
	{
	    /// Configuration for message type (Standard)
		m_nmtCANFrame.frameType = STANDARD;
		m_nmtCANFrame.id = COBID_NMT;

	    m_nmtCANFrame.isRemoteTransmitRequest = FALSE;
	    m_nmtCANFrame.length = CAN_MESSAGE_LENGTH;

	    m_nmtCANFrame.data[0] = byte0;
	    m_nmtCANFrame.data[1] = 0x00;
	    m_nmtCANFrame.data[2] = 0x00;
	    m_nmtCANFrame.data[3] = 0x00;
	    m_nmtCANFrame.data[4] = 0x00;
	    m_nmtCANFrame.data[5] = 0x00;
	    m_nmtCANFrame.data[6] = 0x00;
	    m_nmtCANFrame.data[7] = 0x00;
	}

	inline void EPOSController::SetSDOMessage()
	{
	    /// Configuration for message type (Standard)
		m_sdoCANFrame.frameType = STANDARD;
		m_sdoCANFrame.id = 0x601;

	    m_sdoCANFrame.isRemoteTransmitRequest = FALSE;
	    m_sdoCANFrame.length = CAN_MESSAGE_LENGTH;

		m_sdoCANFrame.data[0] = 0x22; 
		m_sdoCANFrame.data[1] = (0x6060 & 0x000000ff);
		m_sdoCANFrame.data[2] = (0x6060 & 0x0000ff00)/0x100;
		m_sdoCANFrame.data[3] =  0x00;
		m_sdoCANFrame.data[4] = (0xfd & 0x000000ff);
		m_sdoCANFrame.data[5] = (0xfd & 0x0000ff00)/0x100;
		m_sdoCANFrame.data[6] = (0xfd & 0x00ff0000)/0x10000;
		m_sdoCANFrame.data[7] = (0xfd & 0xff000000)/0x1000000;
	}

	inline void EPOSController::SetEposMessage()
	{
	    /// Configuration for message type (Standard)
		m_pdoTransmitx01CANFrame.frameType = STANDARD;
		m_pdoTransmitx01CANFrame.id = COBID_PDOTx01;

	    m_pdoTransmitx01CANFrame.isRemoteTransmitRequest = FALSE;
	    m_pdoTransmitx01CANFrame.length = CAN_MESSAGE_LENGTH;

		m_pdoTransmitx01CANFrame.data[0] = (m_positionSetPoint & 0x000000ff);
		m_pdoTransmitx01CANFrame.data[1] = (m_positionSetPoint & 0x0000ff00)/0x100;
		m_pdoTransmitx01CANFrame.data[2] = (m_positionSetPoint & 0x00ff0000)/0x10000;
		m_pdoTransmitx01CANFrame.data[3] = (m_positionSetPoint & 0xff000000)/0x1000000;
		m_pdoTransmitx01CANFrame.data[4] = (m_current & 0x000000ff);
		m_pdoTransmitx01CANFrame.data[5] = (m_current & 0x0000ff00)/0x100; 
		m_pdoTransmitx01CANFrame.data[6] = (m_controlWord & 0x000000ff);
		m_pdoTransmitx01CANFrame.data[7] = (m_controlWord & 0x0000ff00)/0x100;  
	}

	inline void EPOSController::SetSyncMessage()
	{
	    /// Configuration for message type (Standard)
		m_syncCANFrame.frameType = STANDARD;
		m_syncCANFrame.id = COBID_SYNC;

	    m_syncCANFrame.isRemoteTransmitRequest = FALSE;
	    m_syncCANFrame.length = CAN_MESSAGE_LENGTH;

	    m_syncCANFrame.data[0] = 0x00;
	    m_syncCANFrame.data[1] = 0x00;
	    m_syncCANFrame.data[2] = 0x00;
	    m_syncCANFrame.data[3] = 0x00;
	    m_syncCANFrame.data[4] = 0x00;
	    m_syncCANFrame.data[5] = 0x00;
	    m_syncCANFrame.data[6] = 0x00;
	    m_syncCANFrame.data[7] = 0x00;
	}
}

#endif