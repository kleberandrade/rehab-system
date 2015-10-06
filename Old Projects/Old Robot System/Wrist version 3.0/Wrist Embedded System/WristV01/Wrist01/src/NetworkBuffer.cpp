#include "NetworkBuffer.h"


NetworkBuffer::NetworkBuffer(const char *buffer) :
	m_parchBuffer(buffer)
{

}

double NetworkBuffer::DecodeDouble()
{
	double value = ntohd(*(__int64*)m_parchBuffer);
	m_parchBuffer += sizeof(double);
	return value;
}

float NetworkBuffer::DecodeFloat()
{
	float value = ntohf(*(__int32*)m_parchBuffer);
	m_parchBuffer += sizeof(float);
	return value;
}

int NetworkBuffer::DecodeInt()
{
	int value = ntohl(*(__int32*)m_parchBuffer);
	m_parchBuffer += sizeof(int);
	return value;
}
