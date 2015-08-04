#pragma once

#ifndef RESOURCES_H
#define RESOURCES_H

/**
* @file  Resources.h
* @brief recursos e conigurações que serão utilizados no programa princial
* 	
* @copyright Kleber de Oliveira Andrade 2011-2014, EESC-USP.
*/

#include <windows.h>
#include "Robot/RehabRobot.h"

#if _MSC_VER > 1000
	#pragma once
#endif // _MSC_VER > 1000

#ifdef _WIN32_WCE
	#define __WCE_SECTION
#endif

#define __LITTLE_ENDIAN

#endif