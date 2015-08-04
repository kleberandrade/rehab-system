#include "NetworkData.h"

NetworkData::NetworkData(void)
{

}

void NetworkData::Serialize(NetworkBuffer buffer)
{
	m_uiPacketType = buffer.DecodeInt();
	OnSerialize(buffer);	
}

void NetworkData::Deserialize(NetworkBuffer buffer)
{
	OnDeserialize(buffer);
}