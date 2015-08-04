#pragma once

#ifndef EPOS_H
#define EPOS_H

#include "Toradex/IO.h"
#include "Toradex/CAN.h"

#define NUMBER_OF_TEST_MESSAGES (8)
#define COBID_PDOTx01           (0x201)			/// CAN identifier: COLIBRI -> EPOS
#define COBID_PDORx01           (0x181)			/// CAN identifier: EPOS -> COLIBRI
#define COBID_SYNC				(0x80)			/// CAN identifier SYNC: EPOS -> Escravos
#define COBID_NMT				(0x000)			/// CAN identifier MESTRE: EPOS -> Escravos
#define CAN_MESSAGE_LENGTH      (8)             ///< CAN message length
#define CAN_RECEIVE_TIMEOUT     (20)	        ///< Receive timeout in ms
#define WATER_MARK              (1)             ///< Water mark
#define BUFFER_SIZE_CAN         (10000)     	///< Buffer size

#define CAN_MESSAGE_STRUCT canMessageExt

namespace EPOS
{
	class EPOSController
	{
	public:

		void ClearErrors();
		void EnableControl();
		void DisableControl();

	private:
		const bool PDOgetStatusWord_Fault();

		void PDOsetControlWord_FaultReset(const bool state);
		void PDOsetControlWord_SwitchOn(const bool state);
		void PDOsetControlWord_EnableVoltage(const bool state);
		void PDOsetControlWord_QuickStop(const bool state);
		void PDOsetControlWord_EnableOperation(const bool state);

		void SetEposMessage();
		void SetSDOMessage();
		void SetMasterMessage(BYTE byte0);

		void Waiting(const int time);

		int m_current;
		UCHAR m_statusWord;
		UCHAR m_controlWord;
		UCHAR m_positionSetpoint;
		BOOL m_controlActive;
		CAN_MESSAGE_STRUCT m_nmt;
		CAN_MESSAGE_STRUCT m_pdot;
		CAN_MESSAGE_STRUCT m_pdor;
		CAN_MESSAGE_STRUCT m_sdo;
	};

	inline void EPOSController::Waiting(const int time)
	{
		int i = 1;
		while (i < 5)
		{
			printf(".");	
			Sleep(time);
			i++;
		}
	}

	inline void EPOSController::EnableControl()
	{
		printf("Epos - Enabling axis.\n");
		PDOsetControlWord_SwitchOn(false);
		PDOsetControlWord_EnableVoltage(true);
		PDOsetControlWord_QuickStop(true);
		PDOsetControlWord_EnableOperation(false);
		Waiting(500);

		PDOsetControlWord_SwitchOn(true);
		PDOsetControlWord_EnableVoltage(true);
		PDOsetControlWord_QuickStop(true);
		PDOsetControlWord_EnableOperation(false);
		Waiting(500);

		PDOsetControlWord_SwitchOn(true);
		PDOsetControlWord_EnableVoltage(true);
		PDOsetControlWord_QuickStop(true);
		PDOsetControlWord_EnableOperation(true);
		printf("Epos - Axis active.\n");

		printf("Epos - Enabling control.\n");
		m_controlActive = TRUE;
		Waiting(500);
		printf("Epos - Control active.\n");
	}

	inline void EPOSController::DisableControl()
	{
		printf("Epos - Disabling control.\n");
		m_controlActive = FALSE;
		Waiting(500);

		printf("Epos - Disabling Axis.\n");
		PDOsetControlWord_SwitchOn(true);
		PDOsetControlWord_EnableVoltage(true);
		PDOsetControlWord_QuickStop(true);
		PDOsetControlWord_EnableOperation(false);
		Waiting(500);

		PDOsetControlWord_SwitchOn(false);
		PDOsetControlWord_EnableVoltage(true);
		PDOsetControlWord_QuickStop(true);
		PDOsetControlWord_EnableOperation(false);
		Waiting(500);

		PDOsetControlWord_SwitchOn(false);
		PDOsetControlWord_EnableVoltage(false);
		PDOsetControlWord_QuickStop(false);
		PDOsetControlWord_EnableOperation(false);
		printf("Epos - Axis not active.\n");
	}

	inline void EPOSController::ClearErrors()
	{
		printf("EPOS - init reseting errors.\n");
		PDOsetControlWord_FaultReset(TRUE);
		Waiting(500);
		PDOsetControlWord_FaultReset(FALSE);
		printf("EPOS - finish reseting errors.\n");
	}

	inline const bool EPOSController::PDOgetStatusWord_Fault()
	{
		if ((m_statusWord & 8) > 0) 
			return true;
		
		return false;
	}

	inline void EPOSController::PDOsetControlWord_FaultReset(const bool state)
	{
		if (state) 
			m_statusWord = m_statusWord | 128;
		else 
			m_statusWord = m_statusWord & (~128);
	}

	inline void EPOSController::PDOsetControlWord_SwitchOn(const bool state)
	{
		if (state)
			m_statusWord = m_statusWord | 1;
		else 
			m_statusWord = m_statusWord & (~1);
	}

	inline void EPOSController::PDOsetControlWord_EnableVoltage(const bool state)
	{
		if (state) 
			m_statusWord = m_statusWord | 2;
		else
			m_statusWord = m_statusWord & (~2);
	}

	inline void EPOSController::PDOsetControlWord_QuickStop(const bool state)
	{
		if (state) 
			m_statusWord = m_statusWord | 4;
		else 
			m_statusWord = m_statusWord & (~4);
	}

	inline void EPOSController::PDOsetControlWord_EnableOperation(const bool state)
	{
		if (state) 
			m_statusWord = m_statusWord | 8;
		else
			m_statusWord = m_statusWord & (~8);
	}

	inline void EPOSController::SetEposMessage()
	{
		/// Configuration for message type (Standard)
		m_pdot.frameType = STANDARD;
		m_pdot.id = COBID_PDOTx01;

		m_pdot.isRemoteTransmitRequest = FALSE;
		m_pdot.length = CAN_MESSAGE_LENGTH;

		m_pdot.data[0] = (m_positionSetpoint & 0x000000ff);
		m_pdot.data[1] = (m_positionSetpoint & 0x0000ff00)/0x100;
		m_pdot.data[2] = (m_positionSetpoint & 0x00ff0000)/0x10000;
		m_pdot.data[3] = (m_positionSetpoint & 0xff000000)/0x1000000;
		m_pdot.data[4] = (m_current & 0x000000ff);
		m_pdot.data[5] = (m_current & 0x0000ff00)/0x100; 
		m_pdot.data[6] = (m_controlWord & 0x000000ff);
		m_pdot.data[7] = (m_controlWord & 0x0000ff00)/0x100; 
	}

	inline void EPOSController::SetSDOMessage()
	{
		/// Configuration for message type (Standard)
		m_sdo.frameType = STANDARD;
		m_sdo.id = 0x601;

		m_sdo.isRemoteTransmitRequest = FALSE;
		m_sdo.length = CAN_MESSAGE_LENGTH;

		m_sdo.data[0] = 0x22; 
		m_sdo.data[1] = (0x6060 & 0x000000ff);
		m_sdo.data[2] = (0x6060 & 0x0000ff00)/0x100;
		m_sdo.data[3] =  0x00;
		m_sdo.data[4] = (0xfd & 0x000000ff);
		m_sdo.data[5] = (0xfd & 0x0000ff00)/0x100;
		m_sdo.data[6] = (0xfd & 0x00ff0000)/0x10000;
		m_sdo.data[7] = (0xfd & 0xff000000)/0x1000000;  
	}

	inline void EPOSController::SetMasterMessage(BYTE byte0)
	{
		/// Configuration for message type (Standard)
		m_nmt.frameType = STANDARD;
		m_nmt.id = COBID_NMT;

		m_nmt.isRemoteTransmitRequest = FALSE;
		m_nmt.length = CAN_MESSAGE_LENGTH;

		m_nmt.data[0] = byte0;
		m_nmt.data[1] = 0x00;
		m_nmt.data[2] = 0x00;
		m_nmt.data[3] = 0x00;
		m_nmt.data[4] = 0x00;
		m_nmt.data[5] = 0x00;
		m_nmt.data[6] = 0x00;
		m_nmt.data[7] = 0x00;
	}
}

#endif