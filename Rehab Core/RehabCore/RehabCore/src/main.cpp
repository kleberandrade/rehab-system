#pragma once

#include "Resources.h"

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nCmdShow)
{	
	// Cria um rob� de reabilita��o
	RehabRobot robot = RehabRobot();
	// Inicializa as configura��es do rob�
	robot.Init();



	return EXIT_SUCCESS;
}