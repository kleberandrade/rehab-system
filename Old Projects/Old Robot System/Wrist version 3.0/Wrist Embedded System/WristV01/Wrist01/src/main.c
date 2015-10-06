#include <windows.h>
#include "CoProcLib.h"
#include "PWMLib.h"
#include "clklib.h"
#include "GPIOLib.h"
#include "IntLib.h"
#include "canlib-MCP2515.h"


// Conexao TCP
#include "ClientNetwork.h"
#include "Buffer.h"
#include "BufferDecode.h"
#include "BufferEncode.h"

#if _MSC_VER > 1000
	#pragma once
#endif // _MSC_VER > 1000

#ifdef _WIN32_WCE
	#define _WCE_SECTION
#endif
  
#ifdef _WCE_SECTION
	#pragma comment(lib,"ws2.lib")
#else
	#pragma comment (lib, "ws2_32.lib")
#endif

#define __LITTLE_ENDIAN

/******************************************************************************/

#define NUMBER_OF_TEST_MESSAGES (8)
#define COBID_PDOTx01           (0x201)			/// CAN identifier: COLIBRI -> EPOS
#define COBID_PDORx01           (0x181)			/// CAN identifier: EPOS -> COLIBRI
#define COBID_SYNC				(0x80)			/// CAN identifier SYNC: EPOS -> Escravos
#define COBID_NMT				(0x000)			/// CAN identifier MESTRE: EPOS -> Escravos
#define CAN_MESSAGE_LENGTH      (8)             ///< CAN message length
#define CAN_RECEIVE_TIMEOUT     (20)	            ///< Receive timeout in ms
#define WATER_MARK              (1)             ///< Water mark
#define BUFFER_SIZE_CAN             (10000)         ///< Buffer size

#define	PERIOD					0.002100

#define OSCILOSCOPE_PIN			127
#define DEVICE					0				

#define RATIO					0.08789062

#define CORRENT_MAX 5000
#define CORRENT_MIN -5000

/// All messages
#define CAN_DESCRIPTOR          (0x80000000)
#define CAN_MASK                (0xFFFFFFFF)

#define LP_B 0.0305
#define LP_A -0.9391
#define KIMP 200.0    //$$$$$
#define BIMP 20.0	 //$$$$$$
#define MOTOR_GAIN 0.5  //$$$$$$


// Funções
DWORD WINAPI syncCan(LPVOID lpParameter);

DWORD WINAPI initServer(LPVOID lpParameter);

DWORD WINAPI GetMessages(LPVOID lpParameter);

DWORD WINAPI StatusPrint(LPVOID lpParameter);

static void SetSyncMessage();

static void SetMasterMessage(BYTE byte0);

static void SetEposMessage();

static void SetSDOMessage();

bool PDOgetStatusWord_Fault();

static DWORD freqBase = 0;

BOOL InitCANControler();

double g_position[2], g_velocity[2], g_filteredVelocity[2], g_zero, g_setpointPosition;

BOOL InitCANNetwork();

BOOL GetInterrupt(DWORD *interruptNumber, DWORD *procType); 

BOOL g_controlActive, g_stopControl, gb_Finish;

int i, g_current;

canMessageExt sync, NMT, PDOTx01, PDORx01, SDO;  ///< Keeps message structure contains

DWORD g_statusWord, g_controlWord, g_positionSetPoint;

ClientNetwork gclient;




bool PDOgetStatusWord_Fault(){
	if ((g_statusWord & 8) > 0) return true;
	else return false;
}

void PDOsetControlWord_FaultReset(bool state){
	if (state) g_controlWord = g_controlWord | 128;
	else g_controlWord = g_controlWord & (~128);
}

void PDOsetControlWord_SwitchOn(bool state){
	if (state) g_controlWord = g_controlWord | 1;
	else g_controlWord = g_controlWord & (~1);
}

void PDOsetControlWord_EnableVoltage(bool state){
	if (state) g_controlWord = g_controlWord | 2;
	else g_controlWord = g_controlWord & (~2);
}

void PDOsetControlWord_QuickStop(bool state){
	if (state) g_controlWord = g_controlWord | 4;
	else g_controlWord = g_controlWord & (~4);
}

void PDOsetControlWord_EnableOperation(bool state){
	if (state) g_controlWord = g_controlWord | 8;
	else g_controlWord = g_controlWord & (~8);
}

DWORD WINAPI InitClient(LPVOID lpParameter) {
	
	gclient = ClientNetwork("169.254.210.131",13000);
	gclient.SetDispatcher(new RobotDispatcherMessage);
	gclient.SetRequest(new GameRequestMessage);
	gclient.Open();

	while(!gb_Finish)
	{

		if(gclient.IsOpen())
		{
			//Envia informações para o Jogo
			// Define as informações do robo
			((RobotDispatcherMessage*)gclient.GetDispatcherData())->SetStatus(5);
			((RobotDispatcherMessage*)gclient.GetDispatcherData())->SetPosition(g_position[0]);
			gclient.Send();
			

			gclient.Receive();
			// Recebe os dados nas variáveis
			g_setpointPosition = ((GameRequestMessage*)gclient.GetRequestData())->GetPosition();
			/*double stiffness = ((GameRequestMessage*)gclient.GetRequestData())->GetStiffness();
			double velocity = ((GameRequestMessage*)gclient.GetRequestData())->GetVelocity();
			double acceleration = ((GameRequestMessage*)gclient.GetRequestData())->GetAcceleration();
			int control = ((GameRequestMessage*)gclient.GetRequestData())->GetControl();*/
		}
		else
		{
			Sleep(3000);
			gclient.Open();
			Sleep(1000);
			NKDbgPrintfW(L"Not Connected.....trying to connect!\n");
		}
	}



	return 1;
}

void HabilitaControle(){

	printf("Habilitando Eixos.");
	PDOsetControlWord_SwitchOn(false);
	PDOsetControlWord_EnableVoltage(true);
	PDOsetControlWord_QuickStop(true);
	PDOsetControlWord_EnableOperation(false);
	for(i=1;i<2;i++){
		printf(".");	
		Sleep(500);
	}
	PDOsetControlWord_SwitchOn(true);
	PDOsetControlWord_EnableVoltage(true);
	PDOsetControlWord_QuickStop(true);
	PDOsetControlWord_EnableOperation(false);
	for(i=1;i<2;i++){
		printf(".");	
		Sleep(500);
	}
	PDOsetControlWord_SwitchOn(true);
	PDOsetControlWord_EnableVoltage(true);
	PDOsetControlWord_QuickStop(true);
	PDOsetControlWord_EnableOperation(true);
	printf(".EIXO ATIVO\n");

	printf("Habilitando Controle.");
	g_controlActive = TRUE;
	for(i=1;i<5;i++){
		printf(".");	
		Sleep(500);
	}
	printf(".Controle ATIVO");

}

void DesabilitaControle(){

	printf("Desativando Controle.");
	g_controlActive = FALSE;
	for(i=1;i<5;i++){
		printf(".");	
		Sleep(500);
	}
	printf("\nDesabilibilitando Eixos.");
	PDOsetControlWord_SwitchOn(true);
	PDOsetControlWord_EnableVoltage(true);
	PDOsetControlWord_QuickStop(true);
	PDOsetControlWord_EnableOperation(false);
	for(i=1;i<2;i++){
		printf(".");	
		Sleep(500);
	}
	PDOsetControlWord_SwitchOn(false);
	PDOsetControlWord_EnableVoltage(true);
	PDOsetControlWord_QuickStop(true);
	PDOsetControlWord_EnableOperation(false);
	for(i=1;i<2;i++){
		printf(".");	
		Sleep(500);
	}
	PDOsetControlWord_SwitchOn(false);
	PDOsetControlWord_EnableVoltage(false);
	PDOsetControlWord_QuickStop(false);
	PDOsetControlWord_EnableOperation(false);
	printf("\nEixos Desabilitados.");

}

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nCmdShow)
{	
	
	HANDLE threadClient = CreateThread(NULL, 0, InitClient, NULL, 0, NULL);
	CeSetThreadAffinity(threadClient, 2);
	CeSetThreadPriority(threadClient, 256);

	BOOL isSelectionDone = FALSE; ///< Keeps current state of option selected
    char selectedOption[100];     ///< keeps selected option
    UINT32 stringLength = 0;      ///< Selected option's string length

	printf("  GRUPO DE REABILITACAO ROBOTICA\n");
	printf("ESCOLA DE ENGENHARIA DE SAO CARLOS\n");
	printf("    UNIVERSIDADE DE SAO PAULO\n");
	printf("**********************************\n");
	printf("ROBO DE REABILITACAO: V1.0\n");
	printf("**********************************\n\n\n\n");

	//
	g_zero = 0.0;
	g_controlWord = 0;
	g_position[0] = g_position[1] = g_velocity[0] = g_velocity[1] = 0.0;
	PDOsetControlWord_FaultReset(FALSE);

	
	//Inicilização GPIO
	InitGPIOLib();

	//Prepara e inicializa o base clack
	freqBase = ClkLibGetClockFrequency(PWM_CLK_TEG); // PWM controller input frequency in [Hz]
    SetGPIOAltFn(TEGRA_GPIONUM('l', 5), -1, DIR_IN);    // Set to GPIO Input because multiplexed with GPIO B4 - PWM0
	SetGPIOAltFn(TEGRA_GPIONUM('b', 4),  0, DIR_OUT);   // PWM0 on B.04 in Pin Muxing

	//Inicializa PWM
	 InitPWM(COLIBRI_PWM1,    400, 0); //FREQ ~ 375Hz
	 SetPWMDutyPercentage(COLIBRI_PWM1, 50); // Set to 50% of period


	//Inicialização da rede CAN*******************************************************************
	if (!InitCANControler())
	{
		NKDbgPrintfW(L"FALHA NA INICIALIZACAO DA CAN\n\r");
		return 0;
	}


	//INICIALIZA AS COMUNICAÇÕES COM OS SLAVES ****************************************************

	if (!InitCANNetwork())
	{
		NKDbgPrintfW(L"FALHA NA INICIALIZACAO DA REDE CAN\n\r");
		return 0;
	}

	//*********************************************************************************************

	//Aguarda a estabilização da rede
	Sleep(10);

	//Cria a thread de leitura das mensagens CAN
	HANDLE threadRead = CreateThread(NULL, 0, GetMessages, NULL, 0, NULL);
	CeSetThreadPriority(threadRead, 250);

	//INICIALIZA THREAD THE SYNCRONISMO
	HANDLE threadControl = CreateThread(NULL , 0, syncCan, NULL, 0, NULL);


	//HANDLE	ThreadStatus = CreateThread(NULL, 0, StatusPrint,NULL,0,NULL);

	printf("INICIALIZANDO CONTROLES.");
	Sleep(1000);
	for(i=1;i<10;i++){
		printf(".");	
		Sleep(500);
	}
	

	//Garante desabilitação dos controle
	DesabilitaControle();

	do
    {
		//****************************************************
		//****************************************************
		//****************************************************
		//****************************************************
		//****************************************************
		//****************************************************
		// LOCAL PARA SER MEXIDO!!!!!!
		//****************************************************
		//****************************************************
		//****************************************************
		//****************************************************
		//****************************************************
		printf("\n\n");
		printf("POSICAO ATUAL: %8.2f\n", g_position[0]);
		printf("STATUS ATUAL:");

		if(PDOgetStatusWord_Fault())
		{
			printf(" -- EM FALHA -- ");
		}
		else
		{
			printf(" -- STAND BY -- ");
		}

		printf("\n");
        printf("Selecione a opção:\n");
		printf("****************************\n");
        printf("R. Limpar Erros\n");
		printf("Z. Definir Home Position\n");
		printf("L. Ligar Controle\n");
		printf("D. Desligar Controle\n");
		printf("S. SAIR\n");
		printf("****************************\n");
       
        scanf_s("%s", &selectedOption);
        printf("\n");
        stringLength = strlen(selectedOption);

        /// Enter only if there is only one character entered from key board.
        if (stringLength == 1)
        {
            switch (selectedOption[0])
            {
            case 'r':
            case 'R':
                /// Calls Limpas Erros
				PDOsetControlWord_FaultReset(TRUE);
				printf("Reset de erros.");
				for(i=1;i<5;i++){
					printf(".");	
					Sleep(500);
				}
				PDOsetControlWord_FaultReset(FALSE);
				printf("CONCLUIDO!");
				
                break;

            case 'z':
            case 'Z':
                /// Define a posição Zero
				printf("Mantenha o manipulador parado, definindo posicao zero.");
				g_zero = 0.0;
				for(i=1;i<10;i++){
					printf(".");	
					Sleep(500);
				}
				g_zero = g_position[0];
				printf("CONCLUIDO!");
                break;


			case 'L':
			case 'l':

				HabilitaControle();
				break;

			case 'd':
			case 'D':

				DesabilitaControle();
				break;

			case 's':
            case 'S':
                isSelectionDone = TRUE;
                break;

         

            default:
                printf("Selecao invalida!!\n\n");
                isSelectionDone = FALSE;
            }
        }
        else
        {
            printf("\nTecla invalida!\n\n");
        }

    } while (!isSelectionDone);
	

	//Envia sinal para para o controle
	gb_Finish = TRUE;

	//Aguarda finalizar a thread de leitura
	WaitForSingleObject(threadRead, INFINITE);
	WaitForSingleObject(threadControl, INFINITE);
	

	//Encerra as bibliotecas
	DeInitPWM(0);
	DeInitGPIOLib();
	CANLibMCP2515DeInit(DEVICE);

    return 0;
}



DWORD WINAPI StatusPrint(LPVOID lpParameter){

	while (!gb_Finish){
		
		printf("POSICAO: %8.2f \r", g_position[0]);
		
		
		Sleep(500);
	}


	return 1;
}



DWORD WINAPI GetMessages(LPVOID lpParameter){
	
	while(!gb_Finish){

		//* Recebe a mensagem CAN contendo a posição do motor
		if (0 == CANLibMCP2515ReceiveMessages(DEVICE, &PDORx01, 1, CAN_RECEIVE_TIMEOUT)){
				//NKDbgPrintfW(L"Timeout no recebimento da mensagem CAN\n\r");
				//canStatus = CANLibMCP2515GetStatus(device);
				//NKDbgPrintfW(L"CAN Status: 0x%x\n\r", canStatus);
				//g_controlActive = FALSE;
		}
		else{	
	
			//* Atualiza a posição
			g_position[0] = (RATIO *((PDORx01.data[3]*0x1000000) + (PDORx01.data[2]*0x10000) + (PDORx01.data[1]*0x100) + (PDORx01.data[0]))) - g_zero;
		
			//* ATUALIZA A STATUS WORD
			g_statusWord = (PDORx01.data[7]*0x100) + (PDORx01.data[6]);
		}

	}

	return 0;

}



DWORD WINAPI syncCan(LPVOID lpParameter) 
{

	HANDLE	     hEvent; ///< Handle to the Interrupt Event
	DWORD        dwSysIntr;
	DWORD        dwGpio;
	DWORD	     dwIrq;
	DWORD		 canStatus;

	DOUBLE		 positionError;
	DOUBLE		 velocityError;
	DOUBLE		 actionTorque;
	DOUBLE	     actionCurrent;
    
	PIN_INSTANCE gpio;
    int          returnValue = -1;

	positionError = 0.0;
	velocityError = 0.0;

	//Define o bit de controle
	SetPinAltFn(OSCILOSCOPE_PIN, -1, DIR_OUT);
	SetPinLevel(OSCILOSCOPE_PIN, 0);

	//Prepara mensagem de Sync
	SetSyncMessage();

	//Preparando a interrupção

	//Get GPIO Number for SODIMM Pin 133
	GetGPIOFromPin(133, FALSE, &gpio);
	dwGpio = gpio.inst1;

	//Get IRQ for this particular GPIO
	dwIrq = GetGPIOIrq(dwGpio);
    if (dwIrq)
    {	
	    if (SetGPIOIrqEdge(dwGpio, GPIO_EDGE_RISING))
        {
	        //************************************
	        // General Interrupt Handling Section	

	        // Create an Event to wait on
	        hEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
	        if (hEvent != NULL)
            {
	            // Get the SYSINTR that corresponds to dwIrq (Irqs correspond to the bit numbers of the PXAx Interrupt registers)
	            dwSysIntr = RequestSysInterrupt(dwIrq);
	            if (dwSysIntr)
                {
	                // Link our Event with the SYSINTR
	                if (InterruptInitializeCompat(dwSysIntr, hEvent, NULL, 0))
                    {
	                    
						
						// Increase the priorty of your interrupt Thread
						CeSetThreadPriority(GetCurrentThread(), 250); 
							
						while(!gb_Finish)
								{
									// Wait for Event (Interrupt)
									if (WaitForSingleObject(hEvent, INFINITE) == WAIT_OBJECT_0)
										{
											//sinal de controle
											SetPinLevel(OSCILOSCOPE_PIN, 1);

																					
											////Calcula a velocidade - BUTTER LP 2%
											g_filteredVelocity[1] = g_filteredVelocity[0];
											g_velocity[1] = g_velocity[0];
											g_velocity[0] = (g_position[0]-g_position[1])/PERIOD;

											g_filteredVelocity[0] = (LP_B*g_velocity[0]) + (LP_B*g_velocity[1]) - (LP_A*g_filteredVelocity[1]);
											g_position[1] = g_position[0];


											//Calcula a impedância
											//positionError = g_position[0] - recvPacket.setpoint;
											positionError = g_setpointPosition - g_position[0];
											velocityError = 0.0 - g_filteredVelocity[0];

											//Lei de controle de imedância
											actionTorque = (KIMP*positionError) + (BIMP*velocityError);
											actionCurrent = MOTOR_GAIN*actionTorque;

											//Verifica o Status da rede/motor
											if (PDOgetStatusWord_Fault()) 
											{
												actionCurrent = 0.0;
												g_controlActive = FALSE;
											}
											//Verifica se o controle está ligado
											if (g_controlActive==FALSE) actionCurrent = 0.0;
											
											//LIMITA CORRRENTE
											g_current = (int)actionCurrent;

											if (g_current > CORRENT_MAX) g_current = CORRENT_MAX;
											if (g_current < CORRENT_MIN) g_current = CORRENT_MIN;

											//g_controlWord = recvPacket.command;

											//Envia o comando de corrente
											SetEposMessage();

											if (!CANLibMCP2515TransmitMessage(DEVICE, &PDOTx01, 1000))
												{
													NKDbgPrintfW(L"Could not transmit PDO TX message\n\r");
													canStatus = CANLibMCP2515GetStatus(DEVICE);
													NKDbgPrintfW(L"CAN Status: 0x%x\n\r", canStatus);
													////g_controlActive = FALSE;
												}



											//Envia Sinal de SYNC
											////Envia para a rede CAN o sinal de sync
											if (!CANLibMCP2515TransmitMessage(DEVICE, &sync, 1000))
												{
													NKDbgPrintfW(L"Could not transmit sync message\n\r");
													canStatus = CANLibMCP2515GetStatus(DEVICE);
													NKDbgPrintfW(L"CAN Status: 0x%x\n\r", canStatus);
													//g_controlActive = FALSE;
												}

											//sinal de controle
											SetPinLevel(OSCILOSCOPE_PIN, 0);

					        
											// NOTE: It's possible that we get the same interrupt twice if the signal is not debounced
											InterruptDoneCompat(dwSysIntr);

										}

								}
						//It is very important to deinitalise the interrupt, otherwise it will not work the next time you start the program
						InterruptDisableCompat(dwSysIntr);
						if (ReleaseSysIntr(dwSysIntr))
						{
								 returnValue = 0;
						}

										
                    }
                }
            }
        }
    }

	
	
	return 0;
	
}







BOOL InitCANControler()
{
	DWORD successStatus = TRUE;
	DWORD gpioInterruptNumber = 0;
	DWORD procType = 0; 
	PIN_INSTANCE         IntGpio;
	UINT32 canBaudRate = 500;

	//VARIAVEIS LOCAIS
    DWORD versionMajor, versionMinor, buildNumber;  ///< Major, minor, build number

	//INICIALIZAÇÃO CAN
	CANLibMCP2515GetLibVersion(&versionMajor, &versionMinor, &buildNumber);
	NKDbgPrintfW(L"CAN Lib (CANLib Version %d.%d.%d)\n\r",versionMajor,versionMinor,buildNumber);

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
    {
        successStatus = CANLibMCP2515WriteDescriptor(DEVICE, CAN_DESCRIPTOR, CAN_MASK);
    }

    if (successStatus)
    {

        successStatus = CANLibMCP2515SetBaudrate(DEVICE, canBaudRate);
    }


	return successStatus;
}

BOOL InitCANNetwork()
{

	DWORD canStatus;    ///< Keeps CAN status

	//Envia para a rede CAN o sinal de Start do NMT
	SetMasterMessage(0x01);

	if (!CANLibMCP2515TransmitMessage(DEVICE, &NMT, 1000))
		{
			NKDbgPrintfW(L"Could not transmit NMT Start Message\n\r");
			canStatus = CANLibMCP2515GetStatus(DEVICE);
			NKDbgPrintfW(L"CAN Status: 0x%x\n\r", canStatus);
			return 0;
		}

	SetSDOMessage();

	if (!CANLibMCP2515TransmitMessage(DEVICE, &SDO, 1000))
		{
			NKDbgPrintfW(L"Could not transmit NMT Start Message\n\r");
			canStatus = CANLibMCP2515GetStatus(DEVICE);
			NKDbgPrintfW(L"CAN Status: 0x%x\n\r", canStatus);
			return 0;
		}

 
	return 1;  

}




static void SetSyncMessage()
{
    /// Configuration for message type (Standard)
	sync.frameType = STANDARD;
	sync.id = COBID_SYNC;

    sync.isRemoteTransmitRequest = FALSE;
    sync.length = CAN_MESSAGE_LENGTH;

    sync.data[0] = 0x00;
    sync.data[1] = 0x00;
    sync.data[2] = 0x00;
    sync.data[3] = 0x00;
    sync.data[4] = 0x00;
    sync.data[5] = 0x00;
    sync.data[6] = 0x00;
    sync.data[7] = 0x00;
}



static void SetEposMessage()
{
    /// Configuration for message type (Standard)
	PDOTx01.frameType = STANDARD;
	PDOTx01.id = COBID_PDOTx01;

    PDOTx01.isRemoteTransmitRequest = FALSE;
    PDOTx01.length = CAN_MESSAGE_LENGTH;

	PDOTx01.data[0] = (g_positionSetPoint & 0x000000ff);
	PDOTx01.data[1] = (g_positionSetPoint & 0x0000ff00)/0x100;
	PDOTx01.data[2] = (g_positionSetPoint & 0x00ff0000)/0x10000;
	PDOTx01.data[3] = (g_positionSetPoint & 0xff000000)/0x1000000;
	PDOTx01.data[4] = (g_current & 0x000000ff);
	PDOTx01.data[5] = (g_current & 0x0000ff00)/0x100; 
	PDOTx01.data[6] = (g_controlWord & 0x000000ff);
	PDOTx01.data[7] = (g_controlWord & 0x0000ff00)/0x100; 

  
}

static void SetSDOMessage()
{
    /// Configuration for message type (Standard)
	SDO.frameType = STANDARD;
	SDO.id = 0x601;

    SDO.isRemoteTransmitRequest = FALSE;
    SDO.length = CAN_MESSAGE_LENGTH;

	SDO.data[0] = 0x22; 
	SDO.data[1] = (0x6060 & 0x000000ff);
	SDO.data[2] = (0x6060 & 0x0000ff00)/0x100;
	SDO.data[3] =  0x00;
	SDO.data[4] = (0xfd & 0x000000ff);
	SDO.data[5] = (0xfd & 0x0000ff00)/0x100;
	SDO.data[6] = (0xfd & 0x00ff0000)/0x10000;
	SDO.data[7] = (0xfd & 0xff000000)/0x1000000;
   
}

static void SetMasterMessage(BYTE byte0)
{
    /// Configuration for message type (Standard)
	NMT.frameType = STANDARD;
	NMT.id = COBID_NMT;

    NMT.isRemoteTransmitRequest = FALSE;
    NMT.length = CAN_MESSAGE_LENGTH;

    NMT.data[0] = byte0;
    NMT.data[1] = 0x00;
    NMT.data[2] = 0x00;
    NMT.data[3] = 0x00;
    NMT.data[4] = 0x00;
    NMT.data[5] = 0x00;
    NMT.data[6] = 0x00;
    NMT.data[7] = 0x00;
}


