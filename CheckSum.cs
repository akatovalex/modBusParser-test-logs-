/// <summary>
/// Calculates CRC16 for a specified data array
/// </summary>
/// <param name="data">data array to calculate CRC</param>
/// <param name="data_size">number of bytes (from the beginning) to use in calculation</param>
/// <returns>CRC16</returns>

namespace modBusParse {
    class CheckSum {
        public static uint CRC16(byte[] data, int data_size) {
            const uint MODBUS_CRC_CONST = 0xA001;
            uint CRC = 0xFFFF;

            for (int i = 0; i < data_size; i++) {
                CRC ^= (uint)data[i];    
                for (int k = 0; k < 8; k++) {
                    if ((CRC & 0x01) == 1) {
                        CRC >>= 1;
                        CRC ^= MODBUS_CRC_CONST;
                    }
                    else
                        CRC >>= 1;
                }
            }

            return CRC;
        }
    }
}
