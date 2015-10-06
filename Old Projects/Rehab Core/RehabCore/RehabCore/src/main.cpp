#pragma once

#include "Resources.h"

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nCmdShow)
{	
	// Cria um robô de reabilitação
	RehabRobot robot = RehabRobot();
	// Inicializa as configurações do robô
	robot.Init();



	return EXIT_SUCCESS;
}