#pragma once

#ifndef ROBOT_H
#define ROBOT_H

#define PIN_LED_POWER			101
#define PIN_LED_CONTROLLER		102
#define PIN_LED_EPOS_ERROR		103
#define PIN_LED_DEFINE_HOME		104
#define PIN_LED_COMMUNICATION	105

#include "Toradex/IO.h"
#include "Filters/Butter.h"
#include "Control/ImpedanceControl.h"

namespace Robot
{
	/*******************************************************************
	*   ESTRUTURA DA CLASSE
	*******************************************************************/

	class RehabRobot
	{
	public:
		RehabRobot();
		~RehabRobot();
		
		void Init();
		void Close();
		
		void ClearEposErrors();
		void Calibrate();
		void EnableController();
		void DisableController();

	private:
		void SetupIO();
		void SetupControl();
		void SetupCAN();

		Control::ImpedanceControl control;
	};

	/*******************************************************************
	*   IMPLEMENTAÇÃO DA CLASSE
	*******************************************************************/

	inline RehabRobot::RehabRobot()
	{
		printf("*************************************************************************\n");
		printf("*                    GRUPO DE REABILITACAO ROBOTICA                     *\n");
		printf("*                  ESCOLA DE ENGENHARIA DE SAO CARLOS                   *\n");
		printf("*                      UNIVERSIDADE DE SAO PAULO                        *\n");
		printf("*************************************************************************\n");
		printf("*                     ROBO DE REABILITACAO - V1.0                       *\n");
		printf("*************************************************************************\n\n");
	}

	inline RehabRobot::~RehabRobot()
	{
		Close();
	}

	inline RehabRobot::Init()
	{
		SetupIO();
		SetupCAN();
		SetupControl();
	}

	/**
	*	@brief Método para desligar o robô
	*/
	inline RehabRobot::Close()
	{
		printf("Desativando o robô...");
		Toradex::IO::SetLow(PIN_LED_POWER);
		Toradex::IO::Close();
	}

	inline void RehabRobot::SetupIO()
	{
		//Inicilização GPIO
		Toradex::IO::Init();
		// Configura os pinos de saída para os LEDs
		printf("Configurando as portas de saída...");
		Toradex::IO::SetPinMode(PIN_LED_POWER, DIR_OUT);
		Toradex::IO::SetPinMode(PIN_LED_CONTROLLER, DIR_OUT);
		Toradex::IO::SetPinMode(PIN_LED_EPOS_ERROR, DIR_OUT);
		Toradex::IO::SetPinMode(PIN_LED_DEFINE_HOME, DIR_OUT);
		Toradex::IO::SetPinMode(PIN_LED_COMMUNICATION, DIR_OUT);
		printf("Portas configuradas.");
		// Configura o Led de energia
		Toradex::IO::SetHigh(PIN_LED_POWER);
	}

	inline void RehabRobot::SetupControl()
	{
		printf("Configurando o controle de impedancia do robo...");
		// Criando um filtro do tipo ButterWorth
		Filter::Butter butter = Filter::Butter();
		// Criando um controle de impedância com o filtro BUtterWorth
		control = Control::ImpedanceControl(butter);
		// Desabilita o controle
		DisableController();
		printf("Controle de impedancia configurado.");
	}

	inline void RehabRobot::ClearEposErrors()
	{

	}

	inline void RehabRobot::Calibrate()
	{
		// Liga o LED de zerar a posição
		Toradex::IO::SetHigh(PIN_LED_DEFINE_HOME);
		


		// Desliga o LED de zerar a posiação
		Toradex::IO::SetLow(PIN_LED_DEFINE_HOME);
	}
	
	inline void RehabRobot::EnableController()
	{

		// Liga o LED de controle
		Toradex::IO::SetHigh(PIN_LED_CONTROLLER);
	}
	
	inline void RehabRobot::DisableController()
	{

		// Desliga o LED de controle
		Toradex::IO::SetLow(PIN_LED_CONTROLLER);
	}
}

#endif