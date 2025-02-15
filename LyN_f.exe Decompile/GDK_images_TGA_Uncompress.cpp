/* void __cdecl GDK_images_TGA_Uncompress(unsigned char,unsigned char *,unsigned char *,unsigned long) */

void __cdecl
GDK_images_TGA_Uncompress
          (uchar compressionType, uchar *destination, uchar *compressedData, ulong data_length)
{
  byte currentByte;
  char repeatCount;
  byte highByte;
  undefined4 currentData;
  
  /* Check if there's data to decompress */
  if (0 < (int)data_length) {
    do {
      currentByte = *compressedData;              // Read the first byte from the compressed data
      repeatCount = (currentByte & 0x7f) + 1;     // Calculate how many times to repeat
      compressedData = (uchar *)((int)compressedData + 1); // Move to the next byte
      if ((char)currentByte < '\0') {              // If it's a negative value (indicating RLE compression)
        currentData = *(undefined4 *)compressedData; // Read the next 4 bytes
        compressedData = (uchar *)((int)compressedData + (uint)(compressionType >> 3)); // Adjust pointer
        while (repeatCount != '\0') {
          repeatCount = repeatCount + -1;
          currentByte = (byte)currentData;
          if (compressionType == '\b') {           // For compression type 8
            *destination = currentByte;           // Copy byte to destination
            destination = (uchar *)((int)destination + 1);
            data_length = data_length - 1;
          }
          else {
            highByte = (byte)((uint)currentData >> 8); // Get high byte
            if (compressionType == '\x10') {        // For compression type 16
              *destination = currentByte;
              *(byte *)((int)destination + 1) = highByte;
              destination = (uchar *)((int)destination + 2);
              data_length = data_length - 2;
            }
            else if (compressionType == '\x18') {   // For compression type 24
              *destination = currentByte;
              *(byte *)((int)destination + 1) = highByte;
              *(byte *)((int)destination + 2) = (byte)((uint)currentData >> 0x10);
              destination = (uchar *)((int)destination + 3);
              data_length = data_length - 3;
            }
            else {                                  // For other compression types (32-bit)
              *(undefined4 *)destination = currentData;
              destination = (uchar *)((int)destination + 4);
              data_length = data_length - 4;
            }
          }
        }
      }
      else {                                       // If no RLE compression, just copy bytes
        while (repeatCount != '\0') {
          repeatCount = repeatCount + -1;
          switch(compressionType) {
            case ' ':
            case '\x18':
            case '\x10':
            case '\b':
              *destination = *compressedData;
              destination = (uchar *)((int)destination + 1);
              compressedData = (uchar *)((int)compressedData + 1);
              data_length = data_length - 1;
              break;
          }
        }
      }
    } while (0 < (int)data_length);
  }
  return;
}
