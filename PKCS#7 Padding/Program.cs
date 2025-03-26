using System.Text;

namespace PKSC_7_Padding;

internal class Program
{
    static void Main()
    {
        string originalText = "My very secret text that I want to hide from the World!"; // Make sure the text you want to padd is longer than the padding block size.
        int blockSize = 11; // ToDo - Choose the size of the padding block.

        // Convert the string to an array of bytes.
        byte[] data = Encoding.UTF8.GetBytes(originalText);

        // Add padding
        byte[] paddedData = AddPkcs7Padding(data, blockSize);
        string paddedText = BitConverter.ToString(paddedData);

        // Remove padding
        byte[] originalData = RemovePkcs7Padding(paddedData, blockSize);
        string resultText = Encoding.UTF8.GetString(originalData);

        // Display the original data and the padded data.
        Console.WriteLine($"Original:    {originalText}");
        Console.WriteLine($"Padded data: {paddedText}");
        Console.WriteLine($"Reversed:    {resultText}");
        Console.WriteLine($"Same?        {originalText == resultText}");
    }

    /// <summary>
    /// Adding PKCS#7 Padding.
    /// </summary>
    /// <param name="data">Data to be padded.</param>
    /// <param name="blockSize">Size of the padding block.</param>
    /// <returns>Returns an array of bytes containing the padded data.</returns>
    public static byte[] AddPkcs7Padding(byte[] data, int blockSize)
    {
        int paddingSize = blockSize - (data.Length % blockSize);
        byte[] paddedData = new byte[data.Length + paddingSize];
        
        Buffer.BlockCopy(data, 0, paddedData, 0, data.Length);

        for (int i = data.Length; i < paddedData.Length; i++)
        {
            paddedData[i] = (byte)paddingSize;
        }

        return paddedData;
    }

    /// <summary>
    /// Removing PKCS#7 Padding.
    /// </summary>
    /// <param name="paddedData">Data that needs to have its padding removed.</param>
    /// <param name="blockSize">Size of the padding block used during the padding.</param>
    /// <returns>Returns an array of bytes containing the original data.</returns>
    public static byte[] RemovePkcs7Padding(byte[] paddedData, int blockSize)
    {
        int paddingSize = paddedData[paddedData.Length - 1];
        if (paddingSize < 1 || paddingSize > blockSize)
        {
            throw new ArgumentException("Invalid padding size.");
        }

        for (int i = paddedData.Length - paddingSize; i < paddedData.Length; i++)
        {
            if (paddedData[i] != paddingSize)
            {
                throw new ArgumentException("Invalid padding.");
            }
        }

        byte[] data = new byte[paddedData.Length - paddingSize];

        Buffer.BlockCopy(paddedData, 0, data, 0, data.Length);

        return data;
    }
}
