#pragma once

/**
* @file  IO.h
* @brief Wrapper para as funções de entrada e saída (http://developer.toradex.com/knowledge-base/gpio-lib-api) da placa da Toradex
* 	
* @copyright Kleber de Oliveira Andrade 2011-2014, EESC-USP.
*/

#ifndef IO_H
#define IO_H

#include "GPIOLib.h"

namespace Toradex 
{
	/*******************************************************************
	*   ESTRUTURA DA CLASSE
	*******************************************************************/

	/**
	* @brief Wrapper para as funções de entrada e saída da placa da Toradex
	*/
	class IO
	{
	public:
		
		/**
		*	@brief	Inicia a biblioteca GPIO
		*/
		static void Init(void);

		/**
		*	@brief	Encerra a biblioteca GPIO
		*/
		static void Close(void);

		/**
		*	@brief	Configura o pino
		*/
		static void SetPinMode(DWORD pinNumber, BOOL mode);

		/**
		*	@brief	Configura o pino
		*/
		static void SetPinMode (DWORD pinNumber, DWORD alternativeFunction, BOOL mode);

		/**
		*	@brief	Escreve em um pino específico 
		* 	@param 	pinNumber = número do pino
		* 	@param 	value = valor do pino, TRUE = high ou FALSE = low
		*	@return TRUE se foi possível configurar o pino, FALSE se nao foi possível configurar
		*/
		static BOOL Write(DWORD pinNumber, DWORD value);

		/**
		* 	@brief	Configura o pino para TRUE = high
		* 	@param 	pinNumber = número do pino
		*	@return TRUE se foi possível configurar o pino, FALSE se nao foi possível configurar
		*/
		static BOOL SetHigh(DWORD pinNumber);

		/**
		* 	@brief	Configura o pino para FALSE = low
		* 	@param 	pinNumber = número do pino
		*	@return TRUE se foi possível configurar o pino, FALSE se nao foi possível configurar
		*/
		static BOOL SetLow(DWORD pinNumber);

		/**
		*	@brief	Faz leitura do pino
		* 	@param 	pinNumber = número do pino
		*	@return pino ativo (TRUE = high) ou pino desativado (FALSE = low)
		*/
		static BOOL Read(DWORD pinNumber);


		static BOOL GetGPIO (DWORD pinNumber, BOOL extensionConnector, PIN_INSTANCE *gpio);
	};


	/*******************************************************************
	*   IMPLEMENTAÇÃO DA CLASSE
	*******************************************************************/

	inline void IO::Init(void)
	{
		InitGPIOLib();
	}

	inline void IO::Close(void)
	{
		DeInitGPIOLib();
	}

	inline void IO::SetPinMode (DWORD pinNumber, BOOL mode)   
	{
		SetPinMode(pinNumber, -1, mode);
	}

	inline void IO::SetPinMode (DWORD pinNumber, DWORD alternativeFunction, BOOL mode)   
	{
		SetPinAltFn(pinNumber, alternativeFunction, mode);
	}

	inline BOOL IO::Write(DWORD pinNumber, DWORD value)
	{
		return SetPinLevel(pinNumber, value);
	}

	inline BOOL IO::SetHigh(DWORD pinNumber)
	{
		return Write(pinNumber, TRUE);
	}

	inline BOOL IO::SetLow(DWORD pinNumber)
	{
		return Write(pinNumber, FALSE);
	}

	inline BOOL IO::Read(DWORD pinNumber)
	{
		return GetPinLevel(pinNumber);
	}

	inline BOOL IO::GetGPIO (DWORD pinNumber, BOOL extensionConnector, PIN_INSTANCE *gpio)
	{
		RETURN GetGPIOFromPin(pinNumber, extensionConnector, &gpio);
	}
}

#endif