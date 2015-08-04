#pragma once

/**
* @file  Encoding.h
* @brief Wrapper para as codificações e decodificações de rede
*
* @copyright RehabCore::Net 2011-2014, EESC-USP.
*
*/

#include <winsock2.h>

namespace Net
	{
	/*******************************************************************
	*   ESTRUTURA DA CLASSE
	*******************************************************************/

	/**
	* @brief Wrapper para as codificações e decodificações de rede
	*/
	class Encoding
	{
	public:

		/**
		* @brief Método estático para converter int do host para rede
		* @param iValue - valor int a ser convertido
		* @return valor int convertido
		*/
		static int HostToNetworkInt(int iValue);

		/**
		* @brief Método estático para converter int da rede para o host
		* @param iValue - valor int convertido
		* @return valor int normal
		*/
		static int NetworkToHostInt(int iValue);

		/**
		* @brief Método estático para converter float do host para rede
		* @param fValue - valor float a ser convertido
		* @return valor float convertido
		*/
		static float HostToNetworkFloat(float fValue);

		/**
		* @brief Método estático para converter float da rede para o host
		* @param fValue - valor float convertido
		* @return valor float normal
		*/
		static float NetworkToHostFloat(float fValue);

		/**
		* @brief Método estático para converter double do host para rede
		* @param dValue - valor double a ser convertido
		* @return valor double convertido
		*/
		static double HostToNetworkDouble(double dValue);

		/**
		* @brief Método estático para converter double da rede para o host
		* @param dValue - valor double convertido
		* @return valor double normal
		*/
		static double NetworkToHostDouble(double dValue);
	};


	/*******************************************************************
	*   IMPLEMENTAÇÃO DA CLASSE
	*******************************************************************/

	inline int Encoding::HostToNetworkInt(int iValue)
	{
	#if __BIG_ENDIAN
		return htonl(iValue);
	#else
		return iValue;
	#endif
	}

	inline int Encoding::NetworkToHostInt(int iValue)
	{
	#if __BIG_ENDIAN
		return ntohl(iValue);
	#else
		return iValue;
	#endif
	}

	inline float Encoding::HostToNetworkFloat(float fValue)
	{
	#if __BIG_ENDIAN
		unsigned short i;                            
		unsigned short j;
		unsigned short convert;

		union {
			float val;                       
			unsigned char p[4];
		}zone;                         

		zone.val = fValue;

		for (i = 0, j = 3; i < j; i++, j--)
		{
			convert = (zone.p[i] << 8) + zone.p[j];
			convert = htons(convert);
			zone.p[i] = convert >> 8;
			zone.p[j] = convert & 0x00ff;
		}

		return(zone.val);
	#else
		return fValue;
	#endif
	}

	inline float Encoding::NetworkToHostFloat(float fValue)
	{
	#if __BIG_ENDIAN
		unsigned short i;                          
		unsigned short  j;                            
		unsigned short  convert;                  

		union {
			float val;                      
			unsigned char p[4];                   
		}zone;                        

		zone.val = fValue;

		for (i = 0, j = 3; i < j; i++, j--)
		{
			convert = (zone.p[i] << 8) + zone.p[j];
			convert = ntohs(convert);
			zone.p[i] = convert >> 8;
			zone.p[j] = convert & 0x00ff;
		}

		return(zone.val);
	#else
		return fValue;
	#endif
	}

	inline double Encoding::HostToNetworkDouble(double dValue)
	{
	#if __BIG_ENDIAN
		unsigned short i;           
		unsigned short j;
		unsigned short convert;

		union {
			double val;                    
			unsigned char p[8];                
		}zone;                           

		zone.val = dValue;

		for (i = 0, j = 7; i < j; i++, j--)
		{
			convert = (zone.p[i] << 8) + zone.p[j];
			convert = htons(convert);
			zone.p[i] = convert >> 8;
			zone.p[j] = convert & 0x00ff;
		}

		return(zone.val);
	#else
		return dValue;
	#endif
	}

	inline double Encoding::NetworkToHostDouble(double dValue)
	{
	#if __BIG_ENDIAN
		unsigned short  i;                        
		unsigned short  j;                        
		unsigned short  convert;                     
	 
		union {
			double val;                      
			unsigned char p[8];                    
		}zone;                        
	 
		zone.val = dValue;
	 
		for (i=0, j=7; i < j; i++, j--)
		{
			convert=(zone.p[i] << 8) + zone.p[j];
			convert=ntohs(convert);
			zone.p[i]=convert >> 8;
			zone.p[j]=convert & 0x00ff;
		}
	 
		return(zone.val);      
	#else
		return dValue;
	#endif
	}
}
