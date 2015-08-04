/**
*	The MIT License (MIT)
*
*	Copyright (c) 2011-2014 DreanNet, EESC-USP.
*
*	Permission is hereby granted, free of charge, to any person obtaining a copy
*	of this software and associated documentation files (the "Software"), to deal
*	in the Software without restriction, including without limitation the rights
*	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*	copies of the Software, and to permit persons to whom the Software is
*	furnished to do so, subject to the following conditions:*
*
*	The above copyright notice and this permission notice shall be included in
*	all copies or substantial portions of the Software.
*
*	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
*	THE SOFTWARE.
*/

#include "Encoding.h"


int Encoding::HostToNetworkInt(int iValue)
{
#if __BIG_ENDIAN
	return htonl(iValue);
#else
	return iValue;
#endif
}

int Encoding::NetworkToHostInt(int iValue)
{
#if __BIG_ENDIAN
	return ntohl(iValue);
#else
	return iValue;
#endif
}

float Encoding::HostToNetworkFloat(float fValue)
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

float Encoding::NetworkToHostFloat(float fValue)
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

double Encoding::HostToNetworkDouble(double dValue)
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

double Encoding::NetworkToHostDouble(double dValue)
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

